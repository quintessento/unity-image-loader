using UnityEngine;

/// <summary>
/// Shows a panel with application instructions. 
/// </summary>
public class Instructions : MonoBehaviour
{
    private CanvasGroup _panel;

    public void Open()
    {
        _panel.alpha = 1f;
        _panel.interactable = _panel.blocksRaycasts = true;
    }

    public void Close()
    {
        _panel.alpha = 0f;
        _panel.interactable = _panel.blocksRaycasts = false;
    }

    private void Awake()
    {
        _panel = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        Open();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Open();
        }
    }
}
