using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxHandler : MonoBehaviour
{
    public Transform _camera;

    public float relativeMovement = .3f;

    private void Update()
    {
        transform.position = new Vector3(_camera.position.x * relativeMovement, _camera.position.y * relativeMovement, -1);
    }
}
