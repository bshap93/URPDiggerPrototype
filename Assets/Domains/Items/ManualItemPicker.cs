using System;
using System.Xml;
using Domains.Items;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ManualItemPicker : MonoBehaviour
{
    public string UniqueID;
    public BaseItem Item;
    public int Quantity = 1;
    
    [Header("Feedbacks")]
    [Tooltip("Feedbacks to play when the item is picked up")]
    public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up
    [Tooltip("Feedbacks to play when the item is sold")]
    public MMFeedbacks soldMmFeedbacks; // Feedbacks to play when the item is sold
    public bool NotPickable; // If true, the item cannot be picked up
    
    bool _isInRange;
    
    Inventory _targetInventory;

    void Awake()
    {
        if (string.IsNullOrEmpty(UniqueID))
        {
            UniqueID = Guid.NewGuid().ToString(); // Generate only if unset
            Debug.LogWarning($"Generated new UniqueID for {gameObject.name}: {UniqueID}");
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
        
        if (_targetInventory == null)
        {
            Debug.LogWarning("No inventory found in scene");
        }
        
        if (pickedMmFeedbacks!=null)
        {
            pickedMmFeedbacks.Initialization(gameObject);
        }
        
    }
}
