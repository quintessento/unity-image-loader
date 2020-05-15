using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Convenient browser used to navigate the file system and pick directories.
/// </summary>
public class FolderBrowser : MonoBehaviour
{
    public const string c_CurrentPathKey = "key_CurrentPath";
    public const string c_SelectedDirectoryKey = "key_SelectedDirectory";

    [SerializeField]
    private RectTransform _scrollViewContent = null;
    [SerializeField]
    private FolderListItem _folderListItemPrefab = null;
    [SerializeField]
    private Text _currentPathLabel = null;

    private readonly ListPool<FolderListItem> _folderListItemsPool = new ListPool<FolderListItem>();
    private readonly List<FolderListItem> _currentFolderListItems = new List<FolderListItem>();

    private FolderListItem _selectedItem;

    private string _currentPath;

    private void Start()
    {
        SetCurrentPath(PlayerPrefs.GetString(c_CurrentPathKey));
        if (string.IsNullOrEmpty(_currentPath))
        {
            //update current path and save it to preferences for use elsewhere
            SetCurrentPath(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Images"
                )
            );
        }

        UpdateMenuForPath(_currentPath);
    }

    private void SetCurrentPath(string path)
    {
        _currentPath = path;
        _currentPathLabel.text = path;

        //save the setting so that it persists between sessions
        PlayerPrefs.SetString(c_CurrentPathKey, _currentPath);
        PlayerPrefs.Save();
    }

    private void UpdateMenuForPath(string path)
    {
        StashItems();

        //path of the previous directory; for going up a directory
        string prevPath = DirectoryUtils.GetPreviousDirectory(path);

        //restore from preferences
        string selectedDirectory = PlayerPrefs.GetString(c_SelectedDirectoryKey);

        if(!string.IsNullOrEmpty(path))
            AddFolderListItem(prevPath, "..", prevPath == selectedDirectory);

        string[] directories;
        if (string.IsNullOrEmpty(path))
        {
            //no path available -- we are at the root and can navigate the drives
            directories = Directory.GetLogicalDrives();

            for (int i = 0; i < directories.Length; i++)
            {
                string drivePath = directories[i].Trim(Path.DirectorySeparatorChar);
                AddFolderListItem(drivePath, drivePath, drivePath == selectedDirectory);
            }
        }
        else
        {
            //path is available, we are in one of the drives
            directories = Directory.GetDirectories(path + Path.DirectorySeparatorChar);

            for (int i = 0; i < directories.Length; i++)
            {
                AddFolderListItem(
                    directories[i], 
                    DirectoryUtils.GetDirectoryShortName(directories[i]), 
                    directories[i] == selectedDirectory
                );
            }
        }

        //update current path and save it to preferences for use elsewhere
        SetCurrentPath(path);
    }

    private void StashItems()
    {
        for (int i = 0; i < _currentFolderListItems.Count; i++)
        {
            _currentFolderListItems[i].Clicked -= OnFolderItemClicked;
            _currentFolderListItems[i].DoubleClicked -= OnFolderItemDoubleClicked;
            _folderListItemsPool.Add(_currentFolderListItems[i]);
        }
        _currentFolderListItems.Clear();
    }

    private FolderListItem AddFolderListItem(string path, string label, bool isHighlighted)
    {
        //try getting an item from the pool first
        FolderListItem folderListItem = _folderListItemsPool.Get();
        if (folderListItem == null)
        {
            folderListItem = Instantiate(_folderListItemPrefab, _scrollViewContent);
        }
        else
        {
            //move to the end of the parent's hierarchy to make sure the order of files is followed
            folderListItem.transform.SetAsLastSibling();
        }
        folderListItem.Initialize(path, label, isHighlighted);
        if (isHighlighted)
        {
            _selectedItem = folderListItem;
        }

        folderListItem.Clicked += OnFolderItemClicked;
        folderListItem.DoubleClicked += OnFolderItemDoubleClicked;

        _currentFolderListItems.Add(folderListItem);
        return folderListItem;
    }

    private void OnFolderItemClicked(object sender, EventArgs e)
    {
        FolderListItem clickedItem = sender as FolderListItem;
        if(clickedItem != _selectedItem)
        {
            _selectedItem?.Deselect();
            _selectedItem = clickedItem;

            //save the setting so that it persists between sessions
            PlayerPrefs.SetString(c_SelectedDirectoryKey, clickedItem.Path);
            PlayerPrefs.Save();
        }
        clickedItem.Select();
    }

    private void OnFolderItemDoubleClicked(object sender, EventArgs e)
    {
        FolderListItem clickedItem = sender as FolderListItem;
        try
        {
            if(!string.IsNullOrEmpty(clickedItem.Path))
                Directory.GetDirectories(clickedItem.Path);
        }
        catch (UnauthorizedAccessException)
        {
            Warning.Show("Unauthorized access");
            return;
        }
        
        UpdateMenuForPath(clickedItem.Path);
    }
}
