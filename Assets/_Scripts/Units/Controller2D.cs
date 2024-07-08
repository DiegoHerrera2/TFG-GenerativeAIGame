using System;
using System.Linq;
using UnityEngine;

namespace _Scripts.Units
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Controller2D : MonoBehaviour
    {
        private const float SkinWidth = .015f;

        [SerializeField] 
        private LayerMask collisionMask;
        [SerializeField]
        private int horizontalRayCount = 4;
        [SerializeField]
        private int verticalRayCount = 4;
        [SerializeField] 
        private float maxClimbAngle = 60f;
        [SerializeField] 
        private float maxDescendAngle = 60f;
        
        [SerializeField]
        private float cornerPushForce = 1f;
        
        [SerializeField]
        private float maxCornerPushDistance = 0.5f;

        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;
        
        private BoxCollider2D _collider2D;
        private RaycastOrigins _raycastOrigins;
        public CollisionInfo collisions;

        private void Awake()
        {
            _collider2D = GetComponent<BoxCollider2D>();
            CalculateRaySpacing();
        }

        public void Move(Vector3 velocity)
        {
            UpdateRaycastOrigins();
            collisions.Reset();
            
            if (velocity.y < 0)
                DescendSlope(ref velocity);
            if (velocity.x != 0)
                HorizontalCollisions(ref velocity);
            if (velocity.y != 0)
                VerticalCollisions(ref velocity);
            
            transform.Translate(velocity);
        }
        
        private void HorizontalCollisions(ref Vector3 velocity)
        {
            var directionX = (int)Mathf.Sign(velocity.x);
            var rayLength = Mathf.Abs(velocity.x) + SkinWidth;
            
            for (var i = 0; i < horizontalRayCount; i++)
            {
                var rayOrigin = directionX == -1 ? _raycastOrigins.BottomLeft : _raycastOrigins.BottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
                
                Debug.DrawRay(rayOrigin, Vector2.right * (directionX * rayLength), Color.red);

                if (!hit) continue;
                
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    float distanceToSlopeStart = 0f;
                    if (slopeAngle != collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SkinWidth;
                        //velocity.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart * directionX;
                }
                
                if (!collisions.ClimbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = Mathf.Min(Mathf.Abs(velocity.x), (hit.distance - SkinWidth)) * directionX;
                    rayLength = Mathf.Min(Mathf.Abs(velocity.x) + SkinWidth, hit.distance);

                    if (collisions.ClimbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }
                    
                    collisions.Left = directionX == -1;
                    collisions.Right = directionX == 1;
                }
            }
        }

        private void VerticalCollisions(ref Vector3 velocity)
        {
            var directionY = (int)Mathf.Sign(velocity.y);
            var rayLength = Mathf.Abs(velocity.y) + SkinWidth;
            bool[] hitsAboveLeft = new bool[verticalRayCount/2];
            bool[] hitsAboveRight = new bool[verticalRayCount/2];
            var oldYVelocity = velocity.y;
            var topSideSlopeAngleLeft = 0f;
            var topSideSlopeAngleRight = 0f;

            for (var i = 0; i < verticalRayCount; i++)
            {
                var rayOrigin = directionY == -1 ? _raycastOrigins.BottomLeft : _raycastOrigins.TopLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
                var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
                
                Debug.DrawRay(rayOrigin, Vector2.up * (directionY * rayLength), Color.red);

                if (!hit) continue;
                if (i == 0) topSideSlopeAngleLeft = Vector2.Angle(hit.normal, Vector2.down);
                if (i == verticalRayCount - 1) topSideSlopeAngleRight = Vector2.Angle(hit.normal, Vector2.down);
                
                velocity.y = (hit.distance - SkinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.ClimbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }
                
                collisions.Below = directionY == -1;
                collisions.Above = directionY == 1;
                if (i < verticalRayCount/2) hitsAboveLeft[i] = collisions.Above;
                else hitsAboveRight[i - verticalRayCount/2] = collisions.Above;
                
            }
            
            bool isSlope = topSideSlopeAngleLeft != 0f || topSideSlopeAngleRight != 0f;
            if (isSlope) return;
            float raysCornerLeft = hitsAboveLeft.Count(x => x);
            float raysCornerRight = hitsAboveRight.Count(x => x);
            
            if (raysCornerLeft > 0 || raysCornerRight > 0)
            {
                bool cornerHitLeft = raysCornerRight == 0;
                bool cornerHitRight = raysCornerLeft == 0;
                
                if (cornerHitLeft || cornerHitRight)
                {
                    var distance = _verticalRaySpacing * (verticalRayCount - (cornerHitLeft ? raysCornerLeft : raysCornerRight));
                    if (distance > maxCornerPushDistance) return;
                    float freeRays = (verticalRayCount - raysCornerLeft - raysCornerRight) / verticalRayCount;
                    // If the corner is on the left, we need to apply a force to the right, and viceversa.
                    var pushForce = (cornerHitLeft ? 1/freeRays : -1/freeRays) * cornerPushForce;
                    velocity.x += pushForce;
                    velocity.y = oldYVelocity;
                    collisions.Above = false;
                }
            }
            
        }

        private void ClimbSlope(ref Vector3 velocity, float slopeAngle)
        {
            var moveDistance = Mathf.Abs(velocity.x);
            var climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            if (velocity.y > climbVelocityY) return;
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * velocity.x;
            collisions.Below = true;
            collisions.ClimbingSlope = true;
            collisions.SlopeAngle = slopeAngle;
        }

        private void DescendSlope(ref Vector3 velocity)
        {
            var directionX = (int)Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.BottomRight : _raycastOrigins.BottomLeft;
            var hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

            if (hit)
            {
                var slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
                {
                    if ((int)Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            float moveDistance = Mathf.Abs(velocity.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * velocity.x;
                            velocity.y -= descendVelocityY;

                            collisions.SlopeAngle = slopeAngle;
                            collisions.DescendingSlope = true;
                            collisions.Below = true;
                        }
                    }
                }
            }
        }

        private void UpdateRaycastOrigins()
        {
            var bounds = _collider2D.bounds;
            bounds.Expand(SkinWidth * -2);

            _raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        private void CalculateRaySpacing()
        {
            var bounds = _collider2D.bounds;
            bounds.Expand(SkinWidth * -2);

            horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
            verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

            _horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
        }

        private struct RaycastOrigins
        {
            public Vector2 TopLeft, TopRight;
            public Vector2 BottomLeft, BottomRight;
        }

        [Serializable] public struct CollisionInfo
        {
            public bool Above, Below;
            public bool Left, Right;
            public bool ClimbingSlope, DescendingSlope;
            public float SlopeAngle, SlopeAngleOld;

            public void Reset()
            {
                Above = Below = false;
                Left = Right = false;
                ClimbingSlope = DescendingSlope = false;
                SlopeAngleOld = SlopeAngle;
                SlopeAngle = 0f;
            }
        }
    }
}