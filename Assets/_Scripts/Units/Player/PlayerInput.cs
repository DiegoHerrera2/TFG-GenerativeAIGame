using UnityEngine;

namespace _Scripts.Units.Player
{
    public class PlayerInput : InputProvider
    {
        private void Update()
        {
            Movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.GetKeyDown(KeyCode.Space)) RequestJump();
            if (Input.GetKeyUp(KeyCode.Space)) ReleaseJump();
        }
    }
}
