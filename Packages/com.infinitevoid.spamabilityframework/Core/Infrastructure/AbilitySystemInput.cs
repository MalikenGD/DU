#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif
    
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure
{
    /// <summary>
    /// Handles player input.
    /// It's a very hacky way of handling input, but it doesnt break your builds or the example game.
    /// </summary>
    public class AbilitySystemInput : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        public static Vector3 MousePosition => Mouse.current.position.ReadValue();

        public float GetHorizontalRaw() => 
            Keyboard.current.aKey.isPressed
            ? -1
            : Keyboard.current.dKey.isPressed 
                ? 1 
                : 0;

        public float GetVerticalRaw() => 
            Keyboard.current.sKey.isPressed
                ? -1
                : Keyboard.current.wKey.isPressed 
                    ? 1 
                    : 0;


        public static bool GetLeftMouseButton() => Mouse.current.leftButton.isPressed;

        public static bool GetRightMouseButtonDown() => Mouse.current.rightButton.wasPressedThisFrame;

        public static bool GetRightMouseButtonUp() => Mouse.current.rightButton.wasReleasedThisFrame;

        public static bool ShouldCastQAbility() => Keyboard.current.qKey.wasPressedThisFrame;
        
#elif ENABLE_LEGACY_INPUT_MANAGER
        
        public static Vector3 MousePosition => Input.mousePosition;

        public float GetHorizontalRaw() => Input.GetAxisRaw("Horizontal");

        public float GetVerticalRaw() => Input.GetAxisRaw("Vertical");

        public static bool GetLeftMouseButton() => Input.GetMouseButton(0);

        public static bool GetRightMouseButtonDown() => Input.GetMouseButtonDown(1);

        public static bool GetRightMouseButtonUp() => Input.GetMouseButtonUp(1);

        public static bool ShouldCastQAbility() => Input.GetKeyDown(KeyCode.Q);
#endif
    }
}