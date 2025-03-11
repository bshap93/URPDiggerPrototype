using System;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Items
{
    public class ItemSelectableTrigger : MonoBehaviour
    {
        public BaseItem Item;
        
        [SerializeField]
        MMFeedbacks selectionFeedbacks;
        
        public bool NotPickable;
        ManualItemPicker _itemPicker;
        
        // PlayerItemListPreviewManager _playerPreviewManager;
        
        void Awake()
        {
            _itemPicker = GetComponent<ManualItemPicker>();
            
            if (_itemPicker == null)
                _itemPicker = gameObject.AddComponent<ManualItemPicker>();
            
            _itemPicker.Item = Item;
        }

        void OnTriggerEnter(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}
