using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to represent items in the images list.
/// </summary>
public class ImageListItem : MonoBehaviour
{
    [SerializeField]
    private Image _image = null;
    [SerializeField]
    private Image _spinner = null;
    [SerializeField]
    private Text _fileName = null;
    [SerializeField]
    private Text _creationTime = null;

    public void PreInitialize(string fileName, string creationTime)
    {
        _fileName.text = fileName;
        _creationTime.text = creationTime;

        _image.gameObject.SetActive(false);
        _spinner.gameObject.SetActive(true);
        _spinner.fillAmount = 0f;
    }

    public void SetImage(Texture2D image)
    {
        Rect rect = new Rect(
            0f,
            0f,
            image.width,
            image.height
        );

        _image.sprite = Sprite.Create(image, rect, Vector2.zero);
        _image.preserveAspect = true;

        _spinner.gameObject.SetActive(false);
        _image.gameObject.SetActive(true);
    }

    private void Update()
    {
        //simple spinner (progress pie) filling
        if (_spinner.gameObject.activeSelf)
        {
            if (_spinner.fillAmount < 1f)
            {
                _spinner.fillAmount += Time.deltaTime;
            }
            else
            {
                _spinner.fillAmount = 0f;
            }
        }
    }
}
