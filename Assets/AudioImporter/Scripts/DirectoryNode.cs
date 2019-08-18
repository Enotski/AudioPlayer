using TMPro;
using Unity;
using UnityEngine;
using UnityEngine.UI;
public class DirectoryNode : MonoBehaviour
{
    public delegate void CheckedStateHandler(bool state, string name);

    public event CheckedStateHandler OnStateChanged;
    public TextMeshProUGUI fileName;
    public Toggle toggle;
    private bool _checkState;
    public bool Checked
    {
        get { return _checkState; }
        set
        {
            toggle.isOn = value;
            _checkState = value;           
            OnStateChanged?.Invoke(_checkState, FullName);
        }
    }
    public string FullName { get; set; }
    
    public void OnCheck()
    {
        if (toggle == null)
            return;
        Checked = !Checked;
    }

    public void onBrowserClosed()
    {
        if (toggle == null)
            return;
        Checked = false;
    }
}