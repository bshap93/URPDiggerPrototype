using MoreMountains.Tools;
using UnityEngine;

public class UpgradeUIController : MonoBehaviour, MMEventListener<UIEvent>
{
    private CanvasGroup _canvasGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        CloseUpgradeUI();
    }


    public void CloseUpgradeUI()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenUpgradeUI()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}