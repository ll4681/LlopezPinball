using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace PhysicsDemo
{
    // Keyboard helper that works with the new Input System, the old Input Manager, or both.
    public static class DemoInput
    {
        public static bool Pressed(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM
            KeyControl k = Map(key);
            return k != null && k.wasPressedThisFrame;
#else
            return Input.GetKeyDown(key);
#endif
        }

        public static bool Held(KeyCode key)
        {
#if ENABLE_INPUT_SYSTEM
            KeyControl k = Map(key);
            return k != null && k.isPressed;
#else
            return Input.GetKey(key);
#endif
        }

#if ENABLE_INPUT_SYSTEM
        static KeyControl Map(KeyCode key)
        {
            Keyboard kb = Keyboard.current;
            if (kb == null) return null;
            switch (key)
            {
                case KeyCode.Space: return kb.spaceKey;
                case KeyCode.C: return kb.cKey;
                case KeyCode.T: return kb.tKey;
                case KeyCode.R: return kb.rKey;
                case KeyCode.B: return kb.bKey;
                case KeyCode.UpArrow: return kb.upArrowKey;
                case KeyCode.DownArrow: return kb.downArrowKey;
                case KeyCode.LeftArrow: return kb.leftArrowKey;
                case KeyCode.RightArrow: return kb.rightArrowKey;
                default: return null;
            }
        }
#endif
    }
}
