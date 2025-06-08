using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SandboxGame

{
    public class PNL_Shapes : MonoBehaviour
    {

        [Header("Button References")]
        public GameObject btnMove;
        public GameObject btnRotate;
        public GameObject btnDrag;
        public GameObject btnCircle;
        public GameObject btnRect;
        public GameObject btnTriangle;
        public GameObject btnWeld;
        public GameObject btnSpring;
        public GameObject btnRope;

        public EditController editController;

        bool _isHidden;

        private void Awake()
        {
            //Set button references

            btnMove.GetComponent<Button>().onClick.AddListener(OnMoveButtonClicked);
            btnRotate.GetComponent<Button>().onClick.AddListener(OnRotateButtonClicked);
            btnDrag.GetComponent<Button>().onClick.AddListener(OnDragButtonClicked);
            btnCircle.GetComponent<Button>().onClick.AddListener(OnCircleBtnClicked);
            btnRect.GetComponent<Button>().onClick.AddListener(OnRectBtnClicked);
            btnTriangle.GetComponent<Button>().onClick.AddListener(OnTriBtnClicked);
            btnWeld.GetComponent<Button>().onClick.AddListener(OnWeldBtnClicked);
            btnSpring.GetComponent<Button>().onClick.AddListener(OnSpringBtnClicked);
            btnRope.GetComponent<Button>().onClick.AddListener(OnRopeBtnClicked);

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

        /// <summary>
        /// Hide or unhide
        /// </summary>
        /// <param name="hide"></param>
        public void Hide(bool hide)
        {
            _isHidden = hide;

            if (_isHidden)
            {
                GetComponent<CanvasGroup>().interactable = false;
                GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            }
            else
            {
                GetComponent<CanvasGroup>().interactable = true;
                GetComponent<CanvasGroup>().DOFade(1, 0.3f);
            }

        }

        //------------------------------
        //Button Events
        //------------------------------

        private void OnRotateButtonClicked()
        {
            //EnableButtonOutlineOnly("ROTATE");
            editController.SetToolWithChecking(ToolType.EDIT_ROTATE);
        }

        void OnMoveButtonClicked()
        {
            //EnableButtonOutlineOnly("MOVE");
            editController.SetToolWithChecking(ToolType.EDIT_MOVE);
        }

        void OnDragButtonClicked()
        {
            //EnableButtonOutlineOnly("DRAG");
            editController.SetToolWithChecking(ToolType.EDIT_DRAG);
        }

        public void OnCircleBtnClicked()
        {
            //EnableButtonOutlineOnly("CIRCLE");
            editController.SetToolWithChecking(ToolType.DRAW_CIRCLE);
        }

        public void OnRectBtnClicked()
        {
            //EnableButtonOutlineOnly("RECT");
            editController.SetToolWithChecking(ToolType.DRAW_RECT);

        }

        public void OnTriBtnClicked()
        {
            //EnableButtonOutlineOnly("TRI");
            editController.SetToolWithChecking(ToolType.DRAW_TRI);
        }

        public void OnWeldBtnClicked()
        {
            //EnableButtonOutlineOnly("TRI");
            editController.SetToolWithChecking(ToolType.WELD);
        }

        public void OnSpringBtnClicked()
        {
            editController.SetToolWithChecking(ToolType.JOINT_SPRING);
        }

        public void OnRopeBtnClicked()
        {
            editController.SetToolWithChecking(ToolType.JOINT_ROPE);
        }

        //----------------------
        //Helpers
        //----------------------

        /// <summary>
        /// Given a group of buttons enable only one within the group
        /// </summary>
        public void EnableButtonOutlineOnly(string btnName)
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
                case "TRI":
                    {
                        DisableAllButtonsInGroup("TOOL");
                        DisableAllButtonsInGroup("SHAPE");
                        EnableButtonOutline(btnTriangle, true);
                    }
                    break;
                case "WELD":
                    {
                        DisableAllButtonsInGroup("TOOL");
                        DisableAllButtonsInGroup("SHAPE");
                        DisableAllButtonsInGroup("JOINTS");
                        EnableButtonOutline(btnWeld, true);
                    }
                    break;
                case "SPRING":
                    {
                        DisableAllButtonsInGroup("TOOL");
                        DisableAllButtonsInGroup("SHAPE");
                        DisableAllButtonsInGroup("JOINTS");
                        EnableButtonOutline(btnSpring, true);
                    }
                    break;
                case "ROPE":
                    {
                        DisableAllButtonsInGroup("TOOL");
                        DisableAllButtonsInGroup("SHAPE");
                        DisableAllButtonsInGroup("JOINTS");
                        EnableButtonOutline(btnRope, true);
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
                        EnableButtonOutline(btnTriangle, false);
                    }
                    break;
                case "TOOL":
                    {
                        EnableButtonOutline(btnDrag, false);
                        EnableButtonOutline(btnRotate, false);
                        EnableButtonOutline(btnMove, false);
                    }
                    break;
                case "JOINTS":
                    {
                        EnableButtonOutline(btnWeld, false);
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
}