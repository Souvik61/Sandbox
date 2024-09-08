using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PNL_Shapes : MonoBehaviour
{

    [Header("Button References")]
    public GameObject btnMove;
    public GameObject btnRotate;
    public GameObject btnDrag;
    public GameObject btnCircle;
    public GameObject btnRect;

    public EditController editController;

    private void Awake()
    {
        //Set button references

        btnMove.GetComponent<Button>().onClick.AddListener(OnMoveButtonClicked);
        btnRotate.GetComponent<Button>().onClick.AddListener(OnRotateButtonClicked);
        btnDrag.GetComponent<Button>().onClick.AddListener(OnDragButtonClicked);
        btnCircle.GetComponent<Button>().onClick.AddListener(OnCircleBtnClicked);
        btnRect.GetComponent<Button>().onClick.AddListener(OnRectBtnClicked);

    }

    // Start is called before the first frame update
    void Start()
    {
        //!!!!!!!!!!!!!!!!Dirty delay execution by 1 frame
        CoroutineExtensions.NextFrame(this, () => { SetupPanelInitial(); });
        //SetupPanelInitial();
    }

    /// <summary>
    /// Setup panel initial
    /// </summary>
    public void SetupPanelInitial()
    {
        if (TouchManager.Instance.currentDrawType == ShapeDrawType.CIRCLE)
        {
            EnableButtonOutline(btnCircle, true);
            EnableButtonOutline(btnRect, false);
        }
        else if (TouchManager.Instance.currentDrawType == ShapeDrawType.RECT)
        {
            EnableButtonOutline(btnCircle, false);
            EnableButtonOutline(btnRect, true);
        }

    }

    //------------------------------
    //Button Events
    //------------------------------

    private void OnRotateButtonClicked()
    {
        EnableButtonOutlineOnly("ROTATE");
    }

    void OnMoveButtonClicked()
    { 
        EnableButtonOutlineOnly("MOVE");
    }

    void OnDragButtonClicked()
    { 
        EnableButtonOutlineOnly("DRAG");

        editController.SetTool(ToolType.EDIT_DRAG);
    }

    public void OnCircleBtnClicked()
    {
        //EnableButtonOutline(btnCircle, true);
        //EnableButtonOutline(btnRect, false);

        EnableButtonOutlineOnly("CIRCLE");
        //Select circle draw mode
        //TouchManager.Instance.currentDrawType = ShapeDrawType.CIRCLE;

        editController.SetTool(ToolType.DRAW_CIRCLE);

    }

    public void OnRectBtnClicked()
    {
        //EnableButtonOutline(btnCircle, false);
        //EnableButtonOutline(btnRect, true);

        EnableButtonOutlineOnly("RECT");
        //Select rect draw mode
        //TouchManager.Instance.currentDrawType = ShapeDrawType.RECT;

        editController.SetTool(ToolType.DRAW_RECT);


    }

    //----------------------
    //Helpers
    //----------------------

    /// <summary>
    /// Given a group of buttons enable only one within the group
    /// </summary>
    private void EnableButtonOutlineOnly(string btnName)
    {
        switch (btnName)
        {
            case "DRAG":
                {
                    DisableAllButtonsInGroup("TOOL");
                    DisableAllButtonsInGroup("SHAPE");
                    EnableButtonOutline(btnDrag, true);
                }
                break;
            case "MOVE":
                {
                    DisableAllButtonsInGroup("TOOL");
                    DisableAllButtonsInGroup("SHAPE");
                    EnableButtonOutline(btnMove, true);
                }
                break;
            case "ROTATE":
                {
                    DisableAllButtonsInGroup("TOOL");
                    DisableAllButtonsInGroup("SHAPE");
                    EnableButtonOutline(btnRotate, true);
                }
                break;
            case "CIRCLE":
                {
                    DisableAllButtonsInGroup("TOOL");
                    DisableAllButtonsInGroup("SHAPE");
                    EnableButtonOutline(btnCircle, true);
                }
                break;
            case "RECT":
                {
                    DisableAllButtonsInGroup("TOOL");
                    DisableAllButtonsInGroup("SHAPE");
                    EnableButtonOutline(btnRect, true);
                }
                break;
            default:
                break;
        }
    }

    private void DisableAllButtonsInGroup(string groupName)
    {
        switch (groupName)
        {
            case "SHAPE":
                {
                    EnableButtonOutline(btnCircle, false);
                    EnableButtonOutline(btnRect, false);
                }
                break;
            case "TOOL":
                {
                    EnableButtonOutline(btnDrag, false);
                    EnableButtonOutline(btnRotate, false);
                    EnableButtonOutline(btnMove, false);
                }
                break;
            default:
                break;
        }
    }

    void EnableButtonOutline(GameObject button, bool enabled)
    {
        button.transform.Find("outline").gameObject.SetActive(enabled);
    }

}
