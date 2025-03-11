using Digger.Modules.Core.Sources;
using Digger.Modules.Runtime.Sources;
using Lightbug.CharacterControllerPro.Implementation;
using UnityEngine;

namespace Domains.Mining.Scripts
{
    public class MiningState : CharacterState 
    {
        [SerializeField] float miningRange = 5f;
        public Animator toolAnimator;
        static readonly int SwingMiningTool = Animator.StringToHash("SwingMiningTool");
        public Transform cameraTransform;
        DiggerMasterRuntime _diggerMasterRuntime;
        
        // Digger parameters
        public float size = 4f;
        public float opacity = 0.5f;
        public BrushType brush = BrushType.Sphere;
        public ActionType action = ActionType.Dig;
        public int textureIndex;
        
        public bool editAsynchronously = true;


        protected override void Start()
        {
            base.Start();
            _diggerMasterRuntime = FindFirstObjectByType<DiggerMasterRuntime>();
            if (!_diggerMasterRuntime) {
                Debug.LogWarning(
                    "DiggerRuntimeUsageExample component requires DiggerMasterRuntime component to be setup in the scene. DiggerRuntimeUsageExample will be disabled.");
                enabled = false;
            }
        }
        // Write your transitions here
        public override bool CheckEnterTransition(CharacterState fromState)
        {
            if (toolAnimator != null)
            {
                toolAnimator.SetBool(SwingMiningTool, true);
            }
            return PerformMining();
        }
        
        private bool PerformMining()
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, miningRange ))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
                
                // Perform terrain modification
                if (editAsynchronously)
                {
                    _diggerMasterRuntime.ModifyAsyncBuffured(hit.point, brush, action, textureIndex, opacity, size);
                }
                else
                {
                    _diggerMasterRuntime.Modify(hit.point, brush, action, textureIndex, opacity, size);
                }

                return true;

            }
            
            return false;
        }

        // Write your transitions here
        public override void CheckExitTransition()
        {
            if (!CharacterActions.mine.value)
            {
                Debug.Log("Mining State will be exited");
                CharacterStateController.EnqueueTransition<MyNormalMovement>();
            }
        }


        
        public override void UpdateBehaviour(float dt)
        {
            
        }



    }
}
