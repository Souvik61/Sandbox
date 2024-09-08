using UnityEngine;
using UnityEngine.UI;

public class GenericPopupItem : MonoBehaviour
{
    [SerializeField] private Button btnOk;
    [SerializeField] private Button btnCancel;
    //[SerializeField] private UILocLabelBase buttonText;
    //[SerializeField] private UILocLabelBase messageText;

    private void Awake()
    {
        PopupWindowUI popupWindowUI = GetComponentInParent<PopupWindowUI>();
        btnOk.onClick.AddListener(popupWindowUI.PopupConfirmed);
        btnCancel.onClick.AddListener(popupWindowUI.PopupCancelled);
    }

    public void Initialize(GenericPopup genericPopup)
    {       
        if (genericPopup != null)
        {
            //genericPopup.confirmButtonText = new UILocLabelBase[] { buttonText };
            //genericPopup.messageText = messageText;
        }
    }
}
