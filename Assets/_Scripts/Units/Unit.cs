using System;
using System.Collections.Generic;
using _Scripts.Systems;
using _Scripts.Systems.CameraControl;
using _Scripts.Systems.Particles;
using _Scripts.Units.States;
using Systems;
using Units;
using UnityEngine;
using UnityHFSM;
using StateMachine = UnityHFSM.StateMachine;

namespace _Scripts.Units
{
    [RequireComponent(typeof(Controller2D))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitStats unitStats;
        [SerializeField] private AudioData[] audioData;
        
        private StateMachine _unitStateMachine;
        
        private ParticlePool _particlePool;
        private SpriteRenderer _unitSpriteRenderer;
        public Animator animator;

        private readonly Dictionary<SoundType, AudioCueData> _audios = new Dictionary<SoundType, AudioCueData>();

        public event Action<float> CameraTurn;
        
        private Controller2D _controller;
        private BoxCollider2D _boxCollider2D;

        // Input
        private InputProvider _inputProvider;
        public Vector2 Input => _inputProvider.Movement;
        public bool jumpReleased = false;

        // Movement
        public Vector3 velocity;
        [HideInInspector] public float velocityXSmoothing;
        [HideInInspector] public float yRotation;
        [HideInInspector] public float coyoteTimer;
        [HideInInspector] public float bufferTime;
        [HideInInspector] public float releaseTime;
        public bool CollidingAbove => _controller.collisions.Above;
        public bool CollidingBelow => _controller.collisions.Below;

        private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
        private static readonly int MidAir = Animator.StringToHash("MidAir");
        private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");
        private Vector2 BottomCenter => (Vector2)transform.position + _boxCollider2D.offset - new Vector2(0, _boxCollider2D.size.y / 2);

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            _unitSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
                
            // Get audio data
            foreach (var audioInputs in audioData)
            {
                _audios.Add(audioInputs.soundType, audioInputs.audioCueData);
            }
            
            _particlePool = GetComponent<ParticlePool>();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            unitStats.UpdateParameters();
            _controller = GetComponent<Controller2D>();
            _inputProvider = GetComponent<InputProvider>();
            _inputProvider.JumpRequestedEvent += () => bufferTime = unitStats.bufferTime;
            _inputProvider.JumpReleasedEvent += () => jumpReleased = true;
            
            _unitStateMachine = new StateMachine();

            var airborneStateMachine = new AirborneState(this, unitStats);

            var groundedStateMachine = new HybridStateMachine();
            
            _unitStateMachine.AddState("Airborne", airborneStateMachine);
            _unitStateMachine.AddState("Grounded", groundedStateMachine);
            _unitStateMachine.AddTransition(new Transition(airborneStateMachine.name, groundedStateMachine.name, 
                (t) => CollidingBelow,
                (t) => animator.SetBool(MidAir, false)));
            // Jump transition
            _unitStateMachine.AddTransition(new Transition(groundedStateMachine.name, airborneStateMachine.name, 
                (t) => (bufferTime > 0), 
                (t) => {      
                    animator.SetTrigger(JumpTrigger);
                    animator.SetBool(MidAir, true);
                    _particlePool.PlayEffect(ParticleType.JumpDust, BottomCenter);
                    bufferTime = 0f;
                    coyoteTimer = 0f;
                    velocity.y = releaseTime > 0 ? unitStats.MinJumpVelocity : unitStats.MaxJumpVelocity;
                }));
            // Fall transition
            _unitStateMachine.AddTransition(new Transition(groundedStateMachine.name, airborneStateMachine.name, 
                (t) => !CollidingBelow && coyoteTimer <= 0f, 
                (t) => animator.SetBool(MidAir, true)));
            
            airborneStateMachine.AddState("MovementFalling", new MovementState(this, unitStats));
            airborneStateMachine.AddState("Falling", new IdleState(this));
            airborneStateMachine.AddTransition(new Transition("MovementFalling", "Falling", 
                (t) => Math.Abs(velocity.x) < 0.1f));
            airborneStateMachine.AddTransition(new Transition("Falling", "MovementFalling", 
                (t) => Input.x != 0));
            airborneStateMachine.SetStartState("MovementFalling");

            
            groundedStateMachine.AddState("Movement", new MovementState(this, unitStats));
            groundedStateMachine.AddState("Idle", new IdleState(this));
            groundedStateMachine.AddTransition(new Transition("Movement", "Idle", 
                (t) => Math.Abs(velocity.x) < 0.6f && Input.x == 0));
            groundedStateMachine.AddTransition(new Transition("Idle", "Movement",
                (t) => (Input.x != 0 || velocity.x != 0)));
            
            groundedStateMachine.SetStartState("Movement");

            _unitStateMachine.SetStartState(airborneStateMachine.name);
            
            _unitStateMachine.Init();
            
        }

        private void Update()
        {
            // Jumping variables
            bufferTime = Mathf.Max(0, bufferTime - Time.deltaTime);
            
            coyoteTimer = CollidingBelow ? unitStats.coyoteTime : Mathf.Max(0, coyoteTimer - Time.deltaTime);
            
            // If the unit is grounded, reset y velocity to 0 so it doesn't keep falling
            if (velocity.y < 0 && (CollidingAbove || CollidingBelow)) velocity.y = 0;
            velocity.y += unitStats.Gravity * Time.deltaTime;

            _unitStateMachine.OnLogic();

            _controller.Move(velocity * Time.deltaTime);
            HandleRotation();
            
            // Reset jump variables
            jumpReleased = false;
        }
        
        private void HandleRotation()
        {
            if (_unitSpriteRenderer.flipX && yRotation == 180 || !_unitSpriteRenderer.flipX && yRotation == 0) return;
            _unitSpriteRenderer.flipX = !_unitSpriteRenderer.flipX;
            // Esto debería ir en un script separado que se ocupe de la cámara y asi no tengo que diferenciar si es un jugador o no
            CameraTurn?.Invoke(yRotation);
        }
        
        private void PlayFootstepSound()
        {
            //_particlePool.PlayEffect(ParticleType.FootstepDust, BottomCenter);
            AudioManager.PlayWithPitchRange(_audios[SoundType.Footstep], BottomCenter, new Vector2(0.9f, 1.1f));
        }
        
    }
}
