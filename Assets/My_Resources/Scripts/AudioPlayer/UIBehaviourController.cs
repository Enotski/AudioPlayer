using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBehaviourController : MonoBehaviour
{
    public GameObject TrackItem;
    public GameObject HidePanel;
    public PlayListMenu playListMenu;
    public Transform PlayListContent;
    public CustomReorderableList reorderableList;
    public Browser fileBrowser;
    public List<TrackInfo> allTrackInfosList = new List<TrackInfo>();
    public TrackMenu trackMenu;
    public Button checkAllButton, addButton, acceptButton, removeButton, dragButton;
    public List<GameObject> scrollingElements = new List<GameObject>();
    public TrackInfo currTrackInfo;

    public PlayListController playListController { get; set; }
    public PlayerController playerController { get; set; }

    private bool isDragActive = false;
    private int tracksCount = 0;
    private int _prevIndx = 0;

    [Header("Loading panel")]
    public GameObject LoadingPanel;
    public Slider LoadingSlider;
    public TextMeshProUGUI LoadInfoTMP;
    public TextMeshProUGUI LoadAllInfoTMP;
    private int counter;

    [Header("Play/Pause button")]
    public Image PlauPauseImg;
    public Sprite playSprite;
    public Sprite pauseSprite;

    [Header("Favorite button")]
    public Image FavoriteImg;
    public Sprite FillFavoriteSprite;
    public Sprite EmptyFavoriteSprite;

    [Header("Repeat button")]
    public Image RepeatImg;
    public Sprite RepeatOneSprite;
    public Sprite RepeatAllSprite;

    [Header("Shuffle button")]
    public Image ShuffleImg;

    private bool startToRemove;
    private bool checkAll = true;
    private bool _isActiveHidePanel;
    public bool IsActiveHidePanel
    {
        get
        {
            return _isActiveHidePanel;
        }
        set
        {
            _isActiveHidePanel = value;
            HidePanel.SetActive(_isActiveHidePanel);

            if (!_isActiveHidePanel)
            {
                trackMenu.Hide = _isActiveHidePanel;
                playListMenu.Hide = _isActiveHidePanel;
            }
        }
    }

    private void Start()
    {
        playListController = GetComponent<PlayListController>();
        playerController = GetComponent<PlayerController>();

        playerController.playEvent += OnTrackPlayChanged;
        playerController.shuffleEvent += OnShuffleChanged;
        playerController.repeatEvent += OnRepeatChanged;

        fileBrowser.LoadStarted += OnLoadStarted;
        fileBrowser.importer.Loaded += OnAudioClipLoaded;
        fileBrowser.LoadFinished += OnLoadFinished;

        reorderableList.OnElementDropped.AddListener(ElementDropped);

        onAddNewTrack += InstanceInTrackList;
        TrackListInitialization();
    }

   /// <summary>
   /// Transition between panels
   /// </summary>
   /// <param name="indx">index of panel</param>
    public void GoToScrollingElement(int indx)
    {
        if (indx >= scrollingElements.Count())
            return;

        scrollingElements[_prevIndx].gameObject.SetActive(false);
        scrollingElements[indx].gameObject.SetActive(true);
        _prevIndx = indx;
    }

    #region BrowserImporter Behaviour
    public void OnLoadStarted()
    {
        counter = 0;
        LoadingSlider.maxValue = fileBrowser.checkedCount;
        LoadingPanel.SetActive(true);
    }
    /// <summary>
    /// Loaded track add to main trackList
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="uri"></param>
    public void OnAudioClipLoaded(AudioClip clip, string uri)
    {
        counter++;

        LoadAllInfoTMP.text += $"{uri}\n";
        LoadInfoTMP.text = $"Files loaded: {counter}/{fileBrowser.checkedCount}";
        LoadingSlider.value = counter;

        playListController.mainTrackList.Add(clip);
    }
    public void OnLoadFinished()
    {
        fileBrowser.gameObject.SetActive(false);

        if (playListController.currPlayList == UtilitsAP.PlayListToUse.Main)
        {
            playListController.RefreshCurrentTrackList();
            ClearContentPrefab();
            TrackListInitialization();
            RefreshContentPrefab();
        }
        new WaitForSeconds(2f);
        LoadingPanel.SetActive(false);
    }
    public void OnOpenBrowserClick()
    {
        fileBrowser.gameObject.SetActive(true);

        if (isDragActive)
            OnActiveDrag();
        if (startToRemove)
            OnStartRemoveClick();
    }
    #endregion

    #region PlayList Behaviour
    public delegate void AddNewTrackHandler(AudioClip track);
    public static AddNewTrackHandler onAddNewTrack;

    public void TrackListInitialization()
    {
        tracksCount = 0;
        foreach (var t in playListController.currentTrackList)
        {
            InstanceInTrackList(t);
        }
    }
    /// <summary>
    /// Initialize every trackInfo obj
    /// </summary>
    /// <param name="track"></param>
    public void InstanceInTrackList(AudioClip track)
    {
        tracksCount++;
        GameObject o = Instantiate(TrackItem) as GameObject;
        o.name = track.name;
        o.transform.SetParent(PlayListContent, false);
        TrackInfo info = o.GetComponent<TrackInfo>();
        CustomReorderableElement rdElem = o.GetComponent<CustomReorderableElement>();
        rdElem.InitializeReorderableItem(reorderableList);
        info.GetInfo(track, tracksCount, trackMenu, playerController, playListController);
        allTrackInfosList.Add(info);
    }
    public static void AddNewTrack(AudioClip track)
    {
        onAddNewTrack?.Invoke(track);
    }
    public void OnActiveDrag()
    {
        isDragActive = !isDragActive;
        foreach (var t in allTrackInfosList)
        {
            if (t != null)
            {
                t.DragImg.SetActive(isDragActive);
                t.MenuButton.SetActive(!isDragActive);

                t.isPlayAllowed = !isDragActive;
            }
        }
    }
    /// <summary>
    /// Occurs when dragging element is dropped
    /// </summary>
    /// <param name="droppedStruct"></param>
    private void ElementDropped(CustomReorderableList.ReorderableListEventStruct droppedStruct)
    {
        new WaitForSeconds(1);
        Debug.Log(droppedStruct.DroppedObject + " from " + droppedStruct.FromIndex + " to " + droppedStruct.ToIndex);

        var currTrack = playListController.currentTrackList.Find(t => t.name == droppedStruct.DroppedObject.name);
        playListController.currentTrackList.Remove(currTrack);
        playListController.currentTrackList.Insert(droppedStruct.ToIndex, currTrack);
        RefreshContentPrefab();
    }
    /// <summary>
    /// Refresh content list by changing numbers of existing tracks and removing none existed 
    /// </summary>
    public void RefreshContentPrefab()
    {
        var tracks = FindObjectsOfType<TrackInfo>().ToList();
        int indx;
        foreach (var t in tracks)
        {
            indx = playListController.currentTrackList.IndexOf(t.currentTrack);
            if (indx == -1)
            {
                allTrackInfosList.Remove(t);
                t.IsRemoved = true;
            }
            else
                t.textTrackNum.text = (playListController.currentTrackList.IndexOf(t.currentTrack) + 1).ToString() + ".";
        }
    }
    public void ClearContentPrefab()
    {
        foreach (var t in allTrackInfosList)
            t.IsRemoved = true;
        allTrackInfosList.Clear();
    }

    public void OnStartRemoveClick()
    {
        startToRemove = !startToRemove;

        checkAllButton.gameObject.SetActive(startToRemove);
        acceptButton.gameObject.SetActive(startToRemove);
        addButton.gameObject.SetActive(!startToRemove);
        dragButton.gameObject.SetActive(!startToRemove);

        foreach (var t in allTrackInfosList)
        {
            t.isToRemoveState = startToRemove;

            if (startToRemove == false)
                t.ToRemove = startToRemove;
        }
    }
    public void OnOpenPlayListMenu()
    {
        playListMenu.Hide = true;
        IsActiveHidePanel = true;
    }
    public void OnCheckToRemoveAllClick()
    {
        foreach (var t in allTrackInfosList)
            t.ToRemove = checkAll;
        checkAll = !checkAll;
    }
    public void OnAcceptRemoveClick()
    {
        RemoveFromPlayList();
        OnStartRemoveClick();
        RefreshContentPrefab();
    }
    public void RemoveFromPlayList()
    {
        foreach (var t in allTrackInfosList)
        {
            if (t.ToRemove)
            {
                if (playListController.queueTrackList.Contains(t.currentTrack))
                    playListController.queueTrackList.Remove(t.currentTrack);

                playListController.currentTrackList.Remove(t.currentTrack);
                t.IsRemoved = true;

                RemoveFromQueue(t);
            }
        }
        allTrackInfosList.RemoveAll(track => track.ToRemove);
    }
    public void RemoveFromQueue(TrackInfo trackInfo)
    {
        playListController.queueTrackList.Remove(trackInfo.currentTrack);
        trackInfo.queueTextBlock.text = "";
        foreach (var tInfo in allTrackInfosList)
        {
            int indx = playListController.queueTrackList.IndexOf(tInfo.currentTrack);
            tInfo.queueTextBlock.text = indx == -1 ? "" : $"[{indx + 1}]";
        }
    }
    #endregion

    #region TrackFace Behaviour
    public void OnSetFavorite()
    {
        if (currTrackInfo != null)
        {
            currTrackInfo.IsFavorite = !currTrackInfo.IsFavorite;

            if (currTrackInfo.IsFavorite)
                playListController.favoriteTrackList.Add(currTrackInfo.currentTrack);
            else
            {
                if (playListController.currPlayList == UtilitsAP.PlayListToUse.Favorite && playListController.currentTrackList.Contains(currTrackInfo.currentTrack))
                {
                    playListController.currentTrackList.Remove(currTrackInfo.currentTrack);
                    RefreshContentPrefab();
                }

                playListController.favoriteTrackList.Remove(currTrackInfo.currentTrack);
            }
            FavoriteStateChanged();
        }
    }
    public void FavoriteStateChanged()
    {
        if (currTrackInfo == null)
            return;

        if (currTrackInfo.IsFavorite)
            FavoriteImg.overrideSprite = FillFavoriteSprite;
        else
            FavoriteImg.overrideSprite = EmptyFavoriteSprite;
    }
    public void OnTrackPlayChanged(bool isTrackPlay)
    {
        if (isTrackPlay)
            PlauPauseImg.sprite = pauseSprite;
        else
            PlauPauseImg.sprite = playSprite;
    }
    private void OnShuffleChanged(bool isShuffle)
    {
        var prevColor = ShuffleImg.color;
        if (isShuffle)
            ShuffleImg.color = new Color(prevColor.r, prevColor.g, prevColor.b, 1f);
        else
            ShuffleImg.color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.5f);
    }
    private void OnRepeatChanged(UtilitsAP.RepeatType repeatType)
    {
        var prevColor = RepeatImg.color;

        if (repeatType == UtilitsAP.RepeatType.Track)
        {
            RepeatImg.color = new Color(prevColor.r, prevColor.g, prevColor.b, 1f);
            RepeatImg.overrideSprite = RepeatOneSprite;
        }
        else if (repeatType == UtilitsAP.RepeatType.PlayList)
        {
            RepeatImg.color = new Color(prevColor.r, prevColor.g, prevColor.b, 1f);
            RepeatImg.overrideSprite = RepeatAllSprite;
        }
        else
        {
            RepeatImg.color = new Color(prevColor.r, prevColor.g, prevColor.b, 0.5f);
            RepeatImg.overrideSprite = RepeatAllSprite;
        }
    }
    #endregion

}
