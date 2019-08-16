using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Variables")]
    public float volume = 1.0f;
    public float lerpVolume = 8.0f;
    public float prevVolume;
    public bool isTrackRepeat;
    public bool isShuffleTrackList;
    private UtilitsAP.RepeatType _repeatType = UtilitsAP.RepeatType.None;
    private AudioSource audioSource;
    private UIBehaviourController uiBehaviourController;
    private PlayListController playListController;
    private TrackInfo currTrackInfo;
    private int _currTrackIndex = 0;
    private int _prevTrackIndex = -1;
    private int tracksCount;

    [Header("UI Elements")]
    public Slider progressSlider;
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI currentTrackNameText;
    public TextMeshProUGUI currentDurationText;
    public TextMeshProUGUI currentTrackNumber;

    public static readonly string playerName = "PlayerManager";

    public delegate void TrackIsPlayHandler(bool isPlay);
    public event TrackIsPlayHandler playEvent;

    public delegate void RepeatHandler(UtilitsAP.RepeatType repeatType);
    public event RepeatHandler repeatEvent;

    public delegate void ShuffleHandler(bool isShuffle);
    public event ShuffleHandler shuffleEvent;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        uiBehaviourController = GetComponent<UIBehaviourController>();
        playListController = GetComponent<PlayListController>();
        tracksCount = playListController.currentTrackList.Count();
    }
    private void Update()
    {
        if (audioSource.clip == null)
            return;

        if (audioSource.isPlaying)
            OnUpdateTrackInfoPanel();

        if (playListController.currentTrackList.Count > 1 && (audioSource.time >= audioSource.clip.length))
            AutoChangeTrack();
    }

    private void AutoChangeTrack()
    {
        if (_currTrackIndex == (playListController.currentTrackList.Count - 1) && _repeatType == UtilitsAP.RepeatType.None)
        {
            audioSource.time = 0.0f;
            audioSource.Pause();
            playEvent?.Invoke(false);
            return;
        }
        else if (_repeatType == UtilitsAP.RepeatType.Track)
        {
            audioSource.time = 0.0f;
            audioSource.PlayDelayed(0.1f);
            playEvent?.Invoke(true);
            return;
        }

        if (isShuffleTrackList)
            _currTrackIndex = new System.Random().Next(playListController.currentTrackList.Count);

        ChangeTrack(true);
    }
    public void ChangeTrack(bool toNext)
    {
        _prevTrackIndex = _currTrackIndex;
        if (playListController.currentTrackList.Count > 1)
        {
            if (toNext)
            {
                _currTrackIndex++;
                _currTrackIndex = _currTrackIndex == playListController.currentTrackList.Count ? 0 : _currTrackIndex;
            }
            else
            {
                _currTrackIndex--;
                _currTrackIndex = _currTrackIndex < 0 ? (playListController.currentTrackList.Count - 1) : _currTrackIndex;
            }

            SetTrackToSource(playListController.currentTrackList[_currTrackIndex], true);
        }
    }
    public void SetTrackToSource(AudioClip track, bool play = false)
    {
        _prevTrackIndex = playListController.currentTrackList.IndexOf(audioSource.clip);
        _currTrackIndex = playListController.currentTrackList.IndexOf(track);

        audioSource.clip = track;
        audioSource.time = 0.0f;

        OnSetTrack(_currTrackIndex, _prevTrackIndex);

        if (play)
        {
            audioSource.Play();
            playEvent?.Invoke(true);
        }          
    }
    public void PlayPause()
    {
        if (playListController.currentTrackList.Count <= 0)
            return;

        if (audioSource.clip == null)
        {
            _currTrackIndex = 0;
            SetTrackToSource(playListController.currentTrackList[_currTrackIndex]);
        }

        if (audioSource.isPlaying)
            audioSource.Pause();
        else
            audioSource.Play();

        playEvent?.Invoke(audioSource.isPlaying);
    }
    public void ChangeRepeat()
    {
        if (_repeatType == UtilitsAP.RepeatType.PlayList)
            _repeatType = UtilitsAP.RepeatType.None;
        else
            _repeatType++;

        repeatEvent?.Invoke(_repeatType);
    }
    public void ChangeShuffle()
    {
        isShuffleTrackList = !isShuffleTrackList;
        shuffleEvent?.Invoke(isShuffleTrackList);
    }
    public void OnUpdateTrackInfoPanel()
    {
        var _currTrackTime = audioSource.time;
        progressSlider.value = _currTrackTime;

        currentTimeText.text = UtilitsAP.TimeFormat(_currTrackTime, "#00:00");

        progressSlider.maxValue = audioSource.clip != null ? audioSource.clip.length : 0.0f;
    }
    public void OnSetTrack(int currIndx, int prevIndx)
    {
        currentDurationText.text = UtilitsAP.TimeFormat(audioSource.clip.length, "#00:00");
        currentTimeText.text = UtilitsAP.TimeFormat(0, "#00:00");
        progressSlider.maxValue = audioSource.clip.length;
        progressSlider.value = audioSource.time;
        currentTrackNameText.text = audioSource.clip.name;

        currTrackInfo = uiBehaviourController.PlayListContent.GetChild(currIndx).GetComponent<TrackInfo>();
        currTrackInfo.IsSelected = true;

        uiBehaviourController.currTrackInfo = currTrackInfo;

        if (currTrackInfo.IsFavorite)
            uiBehaviourController.FavoriteImg.overrideSprite = uiBehaviourController.FillFavoriteSprite;
        else
            uiBehaviourController.FavoriteImg.overrideSprite = uiBehaviourController.EmptyFavoriteSprite;

        tracksCount = playListController.currentTrackList.Count();
        currentTrackNumber.text = $"{currIndx + 1}/{tracksCount}";

        if (prevIndx != -1 && prevIndx != currIndx)
            uiBehaviourController.PlayListContent.GetChild(prevIndx).GetComponent<TrackInfo>().IsSelected = false;
    }
    public void OnChangeTrackProgress(float val)
    {
        audioSource.time = val;
        currentTimeText.text = UtilitsAP.TimeFormat(audioSource.time, "#00:00");
    }
}
