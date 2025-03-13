using System;
using Domains.Mining.Scripts;
using Domains.Player.Scripts;
using Gameplay.Events;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Items
{
    public class ItemPicker : MonoBehaviour, IInteractable
    {
        public string UniqueID;
        public BaseItem Item;
        public int Quantity = 1;

        [Header("Feedbacks")] [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up
        [Tooltip("Feedbacks to play when the item is sold")]
        public MMFeedbacks soldMmFeedbacks; // Feedbacks to play when the item is sold
        public bool NotPickable; // If true, the item cannot be picked up
        bool _isBeingDestroyed;

        bool _isInRange;


        Inventory _targetInventory;

        void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString(); // Generate only if unset
                UnityEngine.Debug.LogWarning($"Generated new UniqueID for {gameObject.name}: {UniqueID}");
            }
        }

        void Start()
        {
            if (PickableManager.IsItemPicked(UniqueID))
            {
                Destroy(gameObject);
                return;

                // _promptManager = FindObjectOfType<PromptManager>();
            }

            _targetInventory = FindFirstObjectByType<Inventory>();

            if (_targetInventory == null) UnityEngine.Debug.LogWarning("No inventory found in scene");

            if (pickedMmFeedbacks != null) pickedMmFeedbacks.Initialization(gameObject);
        }


        void OnDestroy()
        {
            _isBeingDestroyed = true;

            _isInRange = false;
            enabled = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInRange = true;
                // _promptManager?.ShowPickupPrompt("Press F to pick up");
                ItemEvent.Trigger(ItemEventType.PickupRangeEntered, Item, transform);
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isInRange = false;
                // _promptManager?.HidePickupPrompt();
                ItemEvent.Trigger(ItemEventType.PickupRangeExited, Item, transform);
            }
        }
        public void Interact()
        {
            PickItem();
        }

        public void PickItem()
        {
            ItemEvent.Trigger(ItemEventType.Picked, Item, transform);
            Destroy(gameObject);
        }
    }
}
