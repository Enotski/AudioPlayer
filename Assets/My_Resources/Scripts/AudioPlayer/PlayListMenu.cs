using UnityEngine;
using UnityEngine.UI;

public class PlayListMenu : MonoBehaviour
{
    public UIBehaviourController uiController;

    public GameObject DropDownSortCategory;
    public GameObject DropDownPlayList;

    private UtilitsAP.PlayListToUse _currPlayList;
    private UtilitsAP.SortCategory _currSort;

    private bool _someChanged;

    public int CurrPlayList
    { set {
            if (_currPlayList != (UtilitsAP.PlayListToUse)value)
                _currPlayList = (UtilitsAP.PlayListToUse)value;
        }
    }
    public int CurrSort
    {
        set
        {
            if (_currSort != (UtilitsAP.SortCategory)value)
                _currSort = (UtilitsAP.SortCategory)value;
        }
    }

    private bool hide;
    public bool Hide
    {
        set
        {
            hide = value;
            if (hide == false)
            {
                CheckChanges();
                DropDownSortCategory.SetActive(hide);
                DropDownPlayList.SetActive(hide);
            }
            this.gameObject.SetActive(hide);
        }
        get
        {
            return hide;
        }
    }

    public void OnSortCategoryClick()
    {
        DropDownSortCategory.SetActive(!DropDownSortCategory.activeSelf);
        DropDownPlayList.SetActive(false);
    }
    public void OnChangePlaylistClick()
    {
        DropDownPlayList.SetActive(!DropDownPlayList.activeSelf);
        DropDownSortCategory.SetActive(false);
    }
    public void OnClearClick()
    {
        uiController.playListController.ClearCurrentPlayList();

        uiController.ClearContentPrefab();
        uiController.IsActiveHidePanel = false;
    }

    public void CheckChanges()
    {
        if (_currPlayList == uiController.playListController.currPlayList && _currSort == uiController.playListController.currSort)
            return;

        uiController.playListController.SortCurrentPlayList(_currSort);
        uiController.playListController.ChangePLayList(_currPlayList);

        uiController.ClearContentPrefab();
        uiController.TrackListInitialization();
        uiController.RefreshContentPrefab();

    }
}
