using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a warning as a part of an on-screen dialog window.
/// </summary>
public class Warning : MonoBehaviour
{
    private static Warning _instance;

    [SerializeField]
    private Text _message = null;

    [Tooltip("How long the warning stays on screen.")]
    [SerializeField]
    private float _onScreenTime = 3f;

    private CanvasGroup _panel;
    private bool _isActive;
    private float _timer;

    /// <summary>
    /// Shows the message in a dialog that automatically disappear after set amount of time.
    /// </summary>
    /// <param name="warningText">Text of the message.</param>
    public static void Show(string warningText)
    {
        _instance._message.text = warningText;
        _instance._panel.alpha = 1f;
        _instance._isActive = true;
        _instance._timer = 0f;
    }

    private void Awake()
    {
        _instance = this;
        _panel = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_isActive)
        {
            if(_timer < _onScreenTime)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _isActive = false;
                _panel.alpha = 0f;
            }
        }
    }
}
