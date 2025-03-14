using Digger.Modules.Core.Sources;
using Digger.Modules.Runtime.Sources;
using Domains.Input.Scripts;
using Domains.Player.Events;
using Domains.Player.Scripts;
using Lightbug.CharacterControllerPro.Implementation;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Mining.Scripts
{
    public class MiningState : CharacterState
    {
        private static readonly int SwingMiningTool = Animator.StringToHash("SwingMiningTool");
        [SerializeField] private float miningRange = 5f;
        public Animator toolAnimator;
        public Transform cameraTransform;

        public float staminaExpense = 2f;

        public MMFeedbacks miningFeedbacks;
        public MMFeedbacks cannotMineFeedbacks;

        // Digger parameters
        public float size;
        public float opacity;
        public BrushType brush = BrushType.Sphere;
        public ActionType action = ActionType.Dig;
        public int textureIndex;

        public bool editAsynchronously = true;
        private DiggerMasterRuntime _diggerMasterRuntime;


        protected override void Start()
        {
            base.Start();
            _diggerMasterRuntime = FindFirstObjectByType<DiggerMasterRuntime>();
            if (!_diggerMasterRuntime)
            {
                UnityEngine.Debug.LogWarning(
                    "DiggerRuntimeUsageExample component requires DiggerMasterRuntime component to be setup in the scene. DiggerRuntimeUsageExample will be disabled.");

                enabled = false;
            }
        }

        // Write your transitions here
        public override bool CheckEnterTransition(CharacterState fromState)
        {
            // if (toolAnimator != null) toolAnimator.SetBool(SwingMiningTool, true);
            if (!PlayerStaminaManager.IsPlayerOutOfStamina()) return PerformMining();

            cannotMineFeedbacks?.PlayFeedbacks();
            return false;
        }

        private bool PerformMining()
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, miningRange))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) interactable.Interact();

                // Perform terrain modification
                if (editAsynchronously)
                    _diggerMasterRuntime.ModifyAsyncBuffured(hit.point, brush, action, textureIndex, opacity, size);
                else
                    _diggerMasterRuntime.Modify(hit.point, brush, action, textureIndex, opacity, size);

                miningFeedbacks?.PlayFeedbacks();

                StaminaEvent.Trigger(StaminaEventType.ConsumeStamina, staminaExpense);
                return true;
            }

            return false;
        }

        // Write your transitions here
        public override void CheckExitTransition()
        {
            if (PlayerStaminaManager.IsPlayerOutOfStamina())
            {
                cannotMineFeedbacks?.PlayFeedbacks();
                CharacterStateController.EnqueueTransition<MyNormalMovement>();
            }

            if (!CustomInputBindings.IsMineMouseButtonPressed())
            {
                UnityEngine.Debug.Log("Mining State will be exited");
                CharacterStateController.EnqueueTransition<MyNormalMovement>();
            }
        }


        public override void UpdateBehaviour(float dt)
        {
            PerformMining();
        }
    }
}