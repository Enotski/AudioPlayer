using TMPro;
using UnityEngine;

public class TrackMenu : MonoBehaviour
{
    private PlayListController playListController;
    private UIBehaviourController uiControlller;

    public GameObject HidePanel;

    public TextMeshProUGUI FavoriteTextBlock;
    public TextMeshProUGUI QueueTextBlock;

    public TrackInfo currentTrackInfo;
    public bool Hide
    {
        set
        {
            gameObject.SetActive(value);
            if (value)
            {
                if (currentTrackInfo.IsInQueue)
                    QueueTextBlock.text = "Remove from queue";
                else
                    QueueTextBlock.text = "Add to queue";

                if (currentTrackInfo.IsFavorite)
                    FavoriteTextBlock.text = "Remove from favorite";
                else
                    FavoriteTextBlock.text = "Add to favorite";
            }
        }
    }

    private void Awake()
    {
        uiControlller = FindObjectOfType<UIBehaviourController>();
        playListController = FindObjectOfType<PlayListController>();
    }
    public void OpenTrackMenu(TrackInfo trackInfo)
    {
        currentTrackInfo = trackInfo;
        Hide = true;
        uiControlller.IsActiveHidePanel = true;
    }

    public void AddToFavorite()
    {
        currentTrackInfo.IsFavorite = !currentTrackInfo.IsFavorite;

        if (currentTrackInfo.IsFavorite)
            playListController.favoriteTrackList.Add(currentTrackInfo.currentTrack);
        else
        {
            if (playListController.currPlayList == UtilitsAP.PlayListToUse.Favorite && playListController.currentTrackList.Contains(currentTrackInfo.currentTrack))
            {
                playListController.currentTrackList.Remove(currentTrackInfo.currentTrack);
                uiControlller.RefreshContentPrefab();
            }

            playListController.favoriteTrackList.Remove(currentTrackInfo.currentTrack);
        }

        uiControlller.FavoriteStateChanged();
        Hide = false;
        uiControlller.IsActiveHidePanel = false;
    }
    public void AddToQueue()
    {
        currentTrackInfo.IsInQueue = !currentTrackInfo.IsInQueue;

        if (currentTrackInfo.IsInQueue)
        {
            playListController.queueTrackList.Add(currentTrackInfo.currentTrack);
            currentTrackInfo.queueTextBlock.text = $"[{playListController.queueTrackList.Count}]";
        }
        else
            uiControlller.RemoveFromQueue(currentTrackInfo);

        Hide = false;
        uiControlller.IsActiveHidePanel = false;
    }
}
