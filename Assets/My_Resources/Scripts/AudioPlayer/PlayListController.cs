using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;
using System.Linq;

public class PlayListController : MonoBehaviour
{
    public List<AudioClip> currentTrackList = new List<AudioClip>();
    public List<AudioClip> mainTrackList = new List<AudioClip>();
    public List<AudioClip> favoriteTrackList = new List<AudioClip>();

    public UtilitsAP.PlayListToUse currPlayList =  UtilitsAP.PlayListToUse.Main;
    public UtilitsAP.SortCategory currSort = UtilitsAP.SortCategory.TrackName;

    public List<AudioClip> queueTrackList = new List<AudioClip>();

    private void Awake()
    {
        currentTrackList.AddRange(mainTrackList);
    }
    public void RefreshCurrentTrackList()
    {
        currentTrackList.Clear();
        if (currPlayList == UtilitsAP.PlayListToUse.Main)
            currentTrackList.AddRange(mainTrackList);
        else
            currentTrackList.AddRange(favoriteTrackList);
    }
    public void ChangePLayList(UtilitsAP.PlayListToUse value)
    {
        if (currPlayList == value)
            return;

        currentTrackList.Clear();
        switch (value)
        {
            case UtilitsAP.PlayListToUse.Main:
                currentTrackList.AddRange(mainTrackList);
                break;
            case UtilitsAP.PlayListToUse.Favorite:
                currentTrackList.AddRange(favoriteTrackList);
                break;
            default:
                break;
        }
        currPlayList = value;
    }
    public void SortCurrentPlayList(UtilitsAP.SortCategory sortCategory)
    {
        Debug.Log(sortCategory);
        switch (sortCategory)
        {
            case UtilitsAP.SortCategory.Folder:
                break;
            case UtilitsAP.SortCategory.Time:
                currentTrackList = currentTrackList.OrderBy(t => t.length).ToList();
                break;
            case UtilitsAP.SortCategory.TrackName:
                currentTrackList = currentTrackList.OrderBy(t => t.name).ToList();
                break;
            default:
                break;
        }
        currSort = sortCategory;
    }
    public void ClearCurrentPlayList()
    {
        currentTrackList.Clear();
    }
}
