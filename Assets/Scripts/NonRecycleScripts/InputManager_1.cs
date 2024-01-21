using UnityEngine;
using UnityEngine.InputSystem;

namespace NonRecycleScripts {
    public class InputManager : MonoBehaviour
    {
        public Vector2 MoveInput;

        public void OnMove(InputValue input) => MoveInput = input.Get<Vector2>();
    }
}
