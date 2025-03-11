using System;
using System.Xml;
using Domains.Items;
using MoreMountains.Feedbacks;



namespace Domains.Input
{
    using UnityEngine;
    public class CustomInputBindings
    {
        // Define keybindings in one place
        private static KeyCode interactKey = KeyCode.E;
        private static KeyCode jumpKey = KeyCode.Space;
        private static KeyCode crouchKey = KeyCode.LeftControl;
        private static KeyCode runKey = KeyCode.LeftShift;
        private static KeyCode alpha1Key = KeyCode.Alpha1;
        private static KeyCode keypad1Key = KeyCode.Keypad1;
        static KeyCode changePerspectiveKey = KeyCode.V;

        // Methods to check input (abstraction layer)
        public static bool IsInteractPressed() => Input.GetKeyDown(interactKey);
        public static bool IsJumpPressed() => Input.GetKeyDown(jumpKey);
        public static bool IsCrouchPressed() => Input.GetKey(crouchKey);
        public static bool IsRunHeld() => Input.GetKey(runKey); 
        public static bool IsChangePerspectivePressed() => Input.GetKeyDown(changePerspectiveKey);
        
        public static int GetPressedNumberKey()
        {
            for (int i = 0; i < 9; i++) // Checks keys 1-9
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i) || Input.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    return i; // Returns the number key that was pressed (0 = "1", 1 = "2", etc.)
                }
            }
            return -1; // No number key was pressed
        }
        
    }
}
