using Digger.Modules.Core.Sources;
using Domains.Input;
using ThirdParty.Plugins.Digger.Demo;
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

        RuntimeDig _digClass;
        DiggerMaster _diggerMaster;
        int _lastTextureIndex = -1; // Track the last detected texture index
        bool _pickupPromptActive;

        void Start()
        {
            _diggerMaster = FindFirstObjectByType<DiggerMaster>();
            _digClass = GetComponent<RuntimeDig>();
        }

        void Update()
        {
            PerformRaycastCheck(); // ✅ Single raycast for both interactables and diggable terrain


            if (CustomInputBindings.IsInteractPressed()) // Press E to interact
                PerformInteraction();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(
                playerCamera.transform.position,
                playerCamera.transform.TransformDirection(Vector3.forward) * interactionDistance);
        }

        void PerformRaycastCheck()
        {
            if (playerCamera == null)
            {
                Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            RaycastHit hit;
            var rayOrigin = playerCamera.transform.position;
            var rayDirection = playerCamera.transform.forward;

            // ✅ Use one raycast that detects both interactables and diggable terrain
            if (Physics.Raycast(
                    rayOrigin, rayDirection, out hit, interactionDistance, interactableLayer | terrainLayer))
            {
                // ✅ First, check if the object is interactable
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    reticle.color = interactReticleColor;
                    ShowPickupPrompt(hit.collider.gameObject);
                    return;
                }

                // ✅ If not interactable, check if it's diggable terrain
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("DiggableTerrain"))
                {
                    reticle.color = interactReticleColor;
                    DetectTexture(hit); // Calls the terrain texture detection
                    return;
                }
            }

            // If nothing is hit, reset reticle and hide UI prompts
            reticle.color = defaultReticleColor;
            if (_pickupPromptActive)
                HidePickupPrompt();
        }


        void ShowPickupPrompt(GameObject item)
        {
            _pickupPromptActive = true;
            Debug.Log($"Press E to pick up {item.name}");
        }

        void HidePickupPrompt()
        {
            _pickupPromptActive = false;
            Debug.Log(""); // Clear message
        }

        void DetectTexture(RaycastHit hit)
        {
            var index = TextureDetector.GetTextureIndex(hit, out var terrain);
            if (terrain != null && index < terrain.terrainData.terrainLayers.Length)
                Debug.Log($"Texture detected: {terrain.terrainData.terrainLayers[index].name}");
            else
                Debug.Log("No texture detected or object is not a terrain.");
        }

        void PerformInteraction()
        {
            if (playerCamera == null)
            {
                Debug.LogError("PlayerInteraction: No camera assigned!");
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
