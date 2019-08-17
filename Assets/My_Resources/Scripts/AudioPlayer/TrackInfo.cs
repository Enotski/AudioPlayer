using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrackInfo : MonoBehaviour
{
    public AudioClip currentTrack;
    public TextMeshProUGUI textTrackNum;
    public TextMeshProUGUI textTrackName;
    public TextMeshProUGUI textTrackDuration;
    public TextMeshProUGUI queueTextBlock;
    public TextMeshProUGUI textFolderName;

    public string folderName;

    public GameObject DragImg;
    public GameObject MenuButton;
    public Button RemoveCheckButton;

    public Image checMark;
    public Image bg;
    public Image btn;

    public bool isToRemoveState
    {
        set
        {
            RemoveCheckButton.gameObject.SetActive(value);
            MenuButton.SetActive(!RemoveCheckButton.gameObject.activeSelf);
        }
    }

    public Color selectedTextColor;
    public Color selectedBgColor;
    public Color normalTextColor;
    public Color normalBgColor;
    public Color btnImgColorOnItemSelected;
    public Color btnImgColorOnItemNormal;

    private bool isSelected = false;
    private bool isFavorite = false;
    private bool isInQueue = false;
    public bool isPlayAllowed = true;

    private bool toRemove;
    public bool ToRemove
    {
        get { return toRemove; }
        set
        {
            toRemove = value;
            checMark.gameObject.SetActive(value);
        }
    }
    public bool IsSelected
    {
        get
        {
            return isSelected;
        }
        set
        {
            isSelected = value;
            if (isSelected)
            {
                textTrackNum.color = selectedTextColor;
                textTrackName.color = selectedTextColor;
                textTrackDuration.color = selectedTextColor;
                btn.color = btnImgColorOnItemSelected;
                bg.color = selectedBgColor;
            }
            else
            {
                textTrackNum.color = normalTextColor;
                textTrackName.color = normalTextColor;
                textTrackDuration.color = normalTextColor;
                btn.color = btnImgColorOnItemNormal;
                bg.color = normalBgColor;
            }
        }
    }
    public bool IsFavorite
    {
        get { return isFavorite; }
        set
        {
            isFavorite = value;
        }
    }
    public bool IsInQueue
    {
        get { return isInQueue; }
        set
        {
            isInQueue = value;
        }
    }
    public bool IsRemoved
    {
        set
        {
            if (value)
            {
                Destroy(gameObject);
            }
        }
    }

    private PlayerController playerContrl;
    private PlayListController playListContrl;
    private TrackMenu trackMenu;
    public void OnOpenTrackMenu()
    {
        trackMenu.OpenTrackMenu(this);
    }
    public void PlayCurrentTrack()
    {
        if(isPlayAllowed)
            playerContrl.SetTrackToSource(currentTrack, true);
    }
    public void OnToRemoveClick()
    {
        ToRemove = !ToRemove;
    }

    public void GetInfo(AudioClip track, int num, TrackMenu currentTrackMenu, PlayerController player = null, PlayListController playList = null, string folder ="")
    {
        currentTrack = track;
        trackMenu = currentTrackMenu;
        folderName = folder;
        playerContrl = player;
        playListContrl = playList;

        textTrackName.text = currentTrack.name;
        textFolderName.text = folderName;
        textTrackDuration.text = UtilitsAP.TimeFormat(currentTrack.length, "#00:00");
        textTrackNum.text = num.ToString() + ".";

        IsFavorite = playListContrl.favoriteTrackList.Contains(currentTrack);
        IsInQueue = playListContrl.queueTrackList.Contains(currentTrack);
        if(IsInQueue)
            queueTextBlock.text = $"[{playListContrl.queueTrackList.IndexOf(currentTrack) + 1}]";
    }
}
