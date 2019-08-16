using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A file browser.
/// </summary>
public class Browser : MonoBehaviour
{
    public event Action LoadStarted;
    public event Action LoadFinished;
    public TextMeshProUGUI currentFolderTmp;
    public AudioImporter importer;
    public PlayListController playListController;
    /// <summary>
    /// File extensions that will show up in the browser.
    /// </summary>
    public List<string> extensions;

    public GameObject content;

    public GameObject listItemPrefab;
    public GameObject folderItemPrefab;
    public GameObject upButton;
    public ScrollRect scrollRect;
    public GameObject folderPanel;
    public GameObject filePanel;

    private string _currentDirectory;
    private string CurrentDirectory {
        get => _currentDirectory;
        set {
            _currentDirectory = value;
            currentFolderTmp.text = _currentDirectory;
        }
    }
    private string[] drives;
    private List<string> directories;
    private List<string> files;
    private HashSet<string> checkedFiles;
    private bool selectDrive;
    private bool scrolling;
    private bool checkAll = true;
    public int checkedCount;
    void Awake()
    {
        checkedCount = 0;
        directories = new List<string>();
        files = new List<string>();
        checkedFiles = new HashSet<string>();

        drives = Directory.GetLogicalDrives();
        CurrentDirectory = PlayerPrefs.GetString("currentDirectory", "");

        selectDrive = (string.IsNullOrEmpty(CurrentDirectory) || !Directory.Exists(CurrentDirectory));

        BuildContent();
    }
    /// <summary>
    /// Go to the current directory's parent directory.
    /// </summary>
    public void Up()
    {
        if (string.IsNullOrEmpty(CurrentDirectory))
            return;

        if (CurrentDirectory == Path.GetPathRoot(CurrentDirectory))
        {
            selectDrive = true;
            ClearContent();
            BuildContent();
        }
        else
        {
            CurrentDirectory = Directory.GetParent(CurrentDirectory).FullName;

            ClearContent();
            BuildContent();
        }
    }
    public void onCloseBrowser()
    {
        checkedCount = 0;
        checkAll = false;
        gameObject.SetActive(false);
    }
    private void BuildContent()
    {
        directories.Clear();
        files.Clear();

        if (selectDrive)
        {
            directories.AddRange(drives);
            StopAllCoroutines();
            StartCoroutine(refreshDirectories());
            return;
        }
        try
        {
            directories.AddRange(Directory.GetDirectories(CurrentDirectory));

            foreach (string file in Directory.GetFiles(CurrentDirectory))
            {
                if (extensions.Contains(Path.GetExtension(file)))
                {
                    files.Add(file);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }

        StopAllCoroutines();
        StartCoroutine(refreshFiles());
        StartCoroutine(refreshDirectories());

        //if (directories.Count + files.Count == 0)
        EventSystem.current.SetSelectedGameObject(upButton);
    }
    private void ClearContent()
    {
        var nodes = content.GetComponentsInChildren<DirectoryNode>();

        foreach (var node in nodes)
            Destroy(node.gameObject);
    }
    private void OnDirectorySelected(int index)
    {
        if (selectDrive)
        {
            CurrentDirectory = drives[index];
            selectDrive = false;
        }
        else
        {
            CurrentDirectory = directories[index];
        }

        ClearContent();
        BuildContent();
    }
    IEnumerator refreshFiles()
    {
        for (int i = 0; i < files.Count; i++)
        {
            AddFileItem(i);

            yield return null;
        }
    }
    IEnumerator refreshDirectories()
    {
        for (int i = 0; i < directories.Count; i++)
        {
            AddDirectoryItem(i);

            yield return null;
        }
    }
    IEnumerator Import(string path)
    {
        importer.Import(path);
        while (!importer.isDone)
            yield return null;
    }
    IEnumerator ImportListOfFiles()
    {
        foreach (var node in checkedFiles)
        {
            yield return StartCoroutine(Import(node));
        }
        LoadFinished?.Invoke();
    }
    public void OnAcceptFileLoadClick()
    {
        checkedCount = 0;
        checkedCount = checkedFiles.Count();
        LoadStarted?.Invoke();
        StartCoroutine("ImportListOfFiles");
    }
    private void AddFileItem(int index)
    {
        GameObject listItem = Instantiate(listItemPrefab);

        listItem.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(files[index]);
        listItem.transform.SetParent(filePanel.transform, false);

        var node = listItem.GetComponentInChildren<DirectoryNode>();
        node.FullName = files[index];
        node.OnStateChanged += OnCheckedStateChanged;

        listItem.name = listItem.GetComponentInChildren<TextMeshProUGUI>().text;
    }
    private void AddDirectoryItem(int index)
    {
        GameObject folderItem = Instantiate(folderItemPrefab);

        if (selectDrive)
            folderItem.GetComponentInChildren<TextMeshProUGUI>().text = directories[index];
        else
            folderItem.GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(directories[index]);

        folderItem.GetComponent<Button>().onClick.AddListener(()=>{
            OnDirectorySelected(index);
        });

        folderItem.transform.SetParent(folderPanel.transform, false);
        folderItem.name = folderItem.GetComponentInChildren<TextMeshProUGUI>().text;
    }
    
    public void OnCheckAll()
    {
        var allItems = content.GetComponentsInChildren<DirectoryNode>();
        foreach (var node in allItems)
        {
            node.Checked = checkAll;
        }
        checkAll = !checkAll;
    }
    public void OnCheckedStateChanged(bool state, string fileName)
    {
        if (state && !checkedFiles.Contains(fileName))
            checkedFiles.Add(fileName);
        else if (!state && checkedFiles.Contains(fileName))
            checkedFiles.Remove(fileName);
    }

    private void GetFiles(DirectoryInfo dir, List<FileInfo> files)
    {
        try
        {
            files.AddRange(dir.GetFiles());
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (var d in dirs)
            {
                GetFiles(d, files);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}