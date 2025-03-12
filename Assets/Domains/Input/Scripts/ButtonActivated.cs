using Domains.Mining.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace Domains.Input.Scripts
{
    public class ButtonActivated : MonoBehaviour, IInteractable
    {
        public UnityEvent OnActivation;
        public ButtonPrompt ButtonPromptPrefab;

        public Vector3 promptTransformOffset;
        public Vector3 promptRotationOffset;
        ButtonPrompt _buttonPrompt;

        void Start()
        {
            if (ButtonPromptPrefab != null)
            {
                var promptPosition = transform.position + promptTransformOffset;
                var promptRotation = Quaternion.Euler(promptRotationOffset);
                _buttonPrompt = Instantiate(ButtonPromptPrefab, promptPosition, promptRotation, transform);
                _buttonPrompt.Initialization();
                _buttonPrompt.Hide();
            }
        }

        public void Interact()
        {
            ActivateButton();
        }

        void ActivateButton()
        {
            if (OnActivation != null)
            {
                OnActivation.Invoke();
                UnityEngine.Debug.Log("Button Activated!");
            }
        }

        public void ShowPrompt()
        {
            if (_buttonPrompt != null) _buttonPrompt.Show();
        }

        public void HidePrompt()
        {
            if (_buttonPrompt != null) _buttonPrompt.Hide();
        }
    }
}
