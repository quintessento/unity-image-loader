using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Used to represent items in the folder browser.
/// </summary>
public class FolderListItem : MonoBehaviour, IPointerDownHandler
{
    private const float _doubleClickDelay = 0.2f;

    [SerializeField]
    private Text _path = null;

    [SerializeField]
    private Image _background = null;

    private float _doubleClickTimer;
    private bool _doubleClickStarted;

    /// <summary>
    /// Event fired when a folder item is clicked once.
    /// </summary>
    public event EventHandler Clicked;
    /// <summary>
    /// Event fired when a folder item is double-clicked.
    /// </summary>
    public event EventHandler DoubleClicked;

    /// <summary>
    /// Full path, represented by this folder list item.
    /// </summary>
    public string Path { get; private set; }

    public void Initialize(string path, string label, bool backgroundEnabled = false)
    {
        Path = path;
        _path.text = label;
        _background.enabled = backgroundEnabled;
    }

    //from IPointerDownHandler 
    public void OnPointerDown(PointerEventData eventData)
    {
        Clicked?.Invoke(this, null);

        if (_doubleClickStarted && _doubleClickTimer < _doubleClickDelay)
        {
            DoubleClicked?.Invoke(this, null);
        }
        else
        {
            _doubleClickTimer = 0f;
            _doubleClickStarted = true;
        }
    }

    public void Select()
    {
        _background.enabled = true;
    }

    public void Deselect()
    {
        _background.enabled = false;
    }

    private void Update()
    {
        if (_doubleClickStarted && _doubleClickTimer < _doubleClickDelay)
        {
            _doubleClickTimer += Time.deltaTime;
        }
    }
}
