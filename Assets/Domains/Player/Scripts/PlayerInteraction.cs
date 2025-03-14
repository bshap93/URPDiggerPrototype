using Digger.Demo;
using Digger.Modules.Core.Sources;
using Digger.Modules.Runtime.Sources;
using Domains.Input.Scripts;
using Domains.Mining.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Domains.Player.Scripts
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactionDistance = 2f; // How far the player can interact
        public LayerMask interactableLayer; // Only detect objects in this layer
        public LayerMask terrainLayer; // Only detect objects in this layer
        public Camera playerCamera; // Reference to the player’s camera
        public Image reticle;
        public Color defaultReticleColor = Color.white;
        public Color interactReticleColor = Color.green;

        private RuntimeDig _digClass;
        private DiggerMaster _diggerMaster;
        private DiggerMasterRuntime _diggerMasterRuntime;
        private bool _interactablePrompt;

        private void Start()
        {
            _diggerMaster = FindFirstObjectByType<DiggerMaster>();
            _diggerMasterRuntime = FindFirstObjectByType<DiggerMasterRuntime>();
            _digClass = GetComponent<RuntimeDig>();
        }

        private void Update()
        {
            PerformRaycastCheck(); // ✅ Single raycast for both interactables and diggable terrain


            if (CustomInputBindings.IsInteractPressed()) // Press E to interact
                PerformInteraction();

            if (CustomInputBindings.IsPersistanceKeyPressed())
            {
                _diggerMasterRuntime.PersistAll();
#if !UNITY_EDITOR
                Debug.Log("Persisted all modified chunks");
#endif
            }
            else if (CustomInputBindings.IsDeletionKeyPressed())
            {
                _diggerMasterRuntime.DeleteAllPersistedData();
#if !UNITY_EDITOR
                Debug.Log("Deleted all persisted data");
#endif
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(
                playerCamera.transform.position,
                playerCamera.transform.TransformDirection(Vector3.forward) * interactionDistance);
        }


        private void PerformRaycastCheck()
        {
            if (playerCamera == null)
            {
                UnityEngine.Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            RaycastHit hit;
            var rayOrigin = playerCamera.transform.position;
            var rayDirection = playerCamera.transform.forward;

            if (Physics.Raycast(
                    rayOrigin, rayDirection, out hit, interactionDistance, interactableLayer | terrainLayer))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                var button = hit.collider.GetComponent<ButtonActivated>();

                if (interactable != null)
                {
                    reticle.color = interactReticleColor;
                    ShowInteractablePrompt(hit.collider.gameObject);

                    // ✅ Show button prompt if applicable
                    if (button != null) button.ShowPrompt();
                    return;
                }
            }

            // Reset if no interactable is found
            reticle.color = defaultReticleColor;
            if (_interactablePrompt)
                HidePickupPrompt();

            HideAllPrompts(); // Hide button prompts if nothing is targeted
        }

        private void HideAllPrompts()
        {
            foreach (var button in FindObjectsOfType<ButtonActivated>()) button.HidePrompt();
        }


        private void ShowInteractablePrompt(GameObject item)
        {
            _interactablePrompt = true;
        }

        private void HidePickupPrompt()
        {
            _interactablePrompt = false;
            UnityEngine.Debug.Log(""); // Clear message
        }

        private void DetectTexture(RaycastHit hit)
        {
            var index = TextureDetector.GetTextureIndex(hit, out var terrain);
            // if (terrain != null && index < terrain.terrainData.terrainLayers.Length)
            //     Debug.Log($"Texture detected: {terrain.terrainData.terrainLayers[index].name}");
            // else
            //     Debug.Log("No texture detected or object is not a terrain.");
        }

        private void PerformInteraction()
        {
            if (playerCamera == null)
            {
                UnityEngine.Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            RaycastHit hit;
            var rayOrigin = playerCamera.transform.position; // Start from the camera
            var rayDirection = playerCamera.transform.forward; // Cast forward from camera

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, interactableLayer))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) interactable.Interact();
            }
        }
    }
}