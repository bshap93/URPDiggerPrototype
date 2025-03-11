using Digger.Modules.Core.Sources;
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
        private int lastTextureIndex = -1; // Track the last detected texture index

        RuntimeDig _digClass;
        private DiggerMaster _diggerMaster;

        void Start()
        {
            _diggerMaster = FindFirstObjectByType<DiggerMaster>(); 
            _digClass = GetComponent<RuntimeDig>();
        }
        
        void Update()
        {
            // CheckForInteractable();
            CheckForDiggableTerrain();
        
            // if (Input.GetKeyDown(KeyCode.E)) // Press E to interact
            // {
            //     PerformInteraction();
            // }
        }
        
        void CheckForDiggableTerrain()
        {
            if (playerCamera == null)
            {
                Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            RaycastHit hit;
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.TransformDirection(Vector3.forward);

            // Adjust the raycast to check specifically for diggable terrain
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, terrainLayer))
            {
                reticle.color = interactReticleColor;

                // Call texture detection from TextureDetector
                int textureIndex = TextureDetector.GetTextureIndex(hit, out Terrain terrain);
                if (terrain != null && textureIndex != lastTextureIndex)
                {
                    // Log the texture index if it has changed
                    Debug.Log($"Texture index changed to: {textureIndex} on terrain: {terrain.name}");
                    lastTextureIndex = textureIndex; // Update the last texture index
                }
            }
            else
            {
                reticle.color = defaultReticleColor;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(playerCamera.transform.position, 
                playerCamera.transform.TransformDirection(Vector3.forward) * interactionDistance);
        
        }

        void CheckForInteractable()
        {
            if (playerCamera == null)
            {
                Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }
        
            RaycastHit hit;
            Vector3 rayOrigin = playerCamera.transform.position; // Start from the camera
            Vector3 rayDirection = playerCamera.transform.TransformDirection(Vector3.forward); // Cast forward from camera

        
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, interactableLayer))
            {

                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
                if (interactable != null)
                {
                    reticle.color = interactReticleColor;
                    return;
                }
            }
        
            reticle.color = defaultReticleColor;
        } 
        
        void DetectTexture(RaycastHit hit)
        {
            int index = TextureDetector.GetTextureIndex(hit, out Terrain terrain);
            if (terrain != null && index < terrain.terrainData.terrainLayers.Length)
            {
                Debug.Log($"Texture detected: {terrain.terrainData.terrainLayers[index].name}");
            }
            else
            {
                Debug.Log("No texture detected or object is not a terrain.");
            }
        }

        void PerformInteraction()
        {
            if (playerCamera == null)
            {
                Debug.LogError("PlayerInteraction: No camera assigned!");
                return;
            }

            RaycastHit hit;
            Vector3 rayOrigin = playerCamera.transform.position; // Start from the camera
            Vector3 rayDirection = playerCamera.transform.forward; // Cast forward from camera

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, interactableLayer))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
            

        }
    }
}
