using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// Images found in the specified folder are displayed in this list.
/// </summary>
public class ImageLoader : MonoBehaviour
{
    [SerializeField]
    private RectTransform _scrollViewContent = null;

    [SerializeField]
    private ImageListItem _imageListItemPrefab = null;

    [SerializeField]
    private InputField _extensionField = null;

    private ListPool<ImageListItem> _imageListItemsPool = new ListPool<ImageListItem>();
    private List<ImageListItem> _currentImageListItems = new List<ImageListItem>();

    private void Start()
    {
        ParseImages();
    }

    /// <summary>
    /// Parses files in a given folder (available through player prefs and/or picked through the FolderBrowser) and generates a list, showing names, creation times, and images, if available.
    /// </summary>
    public void ParseImages()
    {
        string imagesFolderPath = PlayerPrefs.GetString(FolderBrowser.c_SelectedDirectoryKey);
        if (string.IsNullOrEmpty(imagesFolderPath))
        {
            imagesFolderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Images"
            );
            if (!Directory.Exists(imagesFolderPath))
            {
                imagesFolderPath = Application.dataPath;
            }
        }

        try
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(imagesFolderPath);
            //get files of specified extension and order by name
            FileInfo[] filesInfo = directoryInfo.GetFiles(string.Format("*.{0}", _extensionField.text)).OrderBy(x => x.Name).ToArray();

            StopAllCoroutines();
            StartCoroutine(UpdateListCoroutine(filesInfo));
        }
        catch(UnauthorizedAccessException)
        {
            Warning.Show("Unauthorized access");
        }
        catch (Exception e)
        {
            Warning.Show(e.Message);
        }
    }

    private void StashItems()
    {
        for (int i = 0; i < _currentImageListItems.Count; i++)
        {
            _imageListItemsPool.Add(_currentImageListItems[i]);
        }
        _currentImageListItems.Clear();
    }

    private IEnumerator UpdateListCoroutine(FileInfo[] files)
    {
        StashItems();

        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i];

            //try getting an item from the pool first
            ImageListItem imageListItem = _imageListItemsPool.Get();
            if (imageListItem == null)
            {
                imageListItem = Instantiate(_imageListItemPrefab, _scrollViewContent);
            }
            else
            {
                //move to the end of the parent's hierarchy to make sure the order of files is followed
                imageListItem.transform.SetAsLastSibling();
            }
            _currentImageListItems.Add(imageListItem);
            imageListItem.PreInitialize(file.Name, file.CreationTime.ToString());

            StartCoroutine(LoadTextureCoroutine(file.FullName, imageListItem));
        }

        yield return null;
    }

    private IEnumerator LoadTextureCoroutine(string path, ImageListItem imageListItem)
    {
        //start a request for loading the file
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(path))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Warning.Show(request.error);
                Debug.LogError(request.error);
            }
            else
            {
                //get the texture, since the request was successful

                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                try
                {
                    imageListItem.SetImage(texture);
                }
                catch (Exception e)
                {
                    Warning.Show(e.Message);
                    Debug.LogError(e);
                }
                yield return texture;
            }
        }
    }
}
