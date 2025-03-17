using Domains.Input.Scripts;
using Domains.Mining.Scripts;
using Lightbug.CharacterControllerPro.Implementation;
using UnityEngine;

namespace Domains.Gameplay.Mining.Scripts
{
    public class UIInteractionState : CharacterState
    {
        public override void EnterBehaviour(float dt, CharacterState fromState)
        {
            // Freeze character movements
            CharacterActor.Velocity = Vector3.zero;
        }

        public override void UpdateBehaviour(float dt)
        {
            // Keep the player stationary
            CharacterActor.Velocity = Vector3.zero;
        }

        public override void CheckExitTransition()
        {
            // Logic to close UI and return to normal state
            if (CustomInputBindings.IsInteractPressed() || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                // Close UI here
                FindFirstObjectByType<UpgradeUIController>().CloseUpgradeUI();
                CharacterStateController.EnqueueTransition<MyNormalMovement>();
            }
        }
    }
}