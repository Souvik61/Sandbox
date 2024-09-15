using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{

    public class PNL_Inspector : MonoBehaviour
    {

        [Header("UI References")]
        public TMP_Text typeText;
        [Header("Position Text")]
        public TMP_Text txtXPosition;
        public TMP_Text txtYPosition;
        [Header("Rotation Text")]
        public TMP_Text txtZRotation;

        public EditController editController;

        private void Awake()
        {
            //Set button references    

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Setup panel initial
        /// </summary>
        public void SetupPanelInitial()
        {


        }

        /// <summary>
        /// Update Inspector to show details of that object
        /// </summary>
        /// <param name="obj"></param>
        public void SetView(ObjectBase obj)
        {
            typeText.text = obj.type.ToString();

        }

        /// <summary>
        /// Live link Inspector to show details of that object
        /// </summary>
        /// <param name="obj"></param>
        public void LinkView(ObjectBase obj)
        {
            if (obj != null)
            {
                typeText.text = obj.type.ToString();
            }
            else
            {
                typeText.text = "None";
                txtXPosition.text = Constants.TEXTNA;
                txtYPosition.text = Constants.TEXTNA;
                txtZRotation.text = Constants.TEXTNA;
            }

        }

        //------------------
        //Setters
        //------------------

        /// <summary>
        /// Set position value of the inspector view
        /// </summary>
        /// <param name="position"></param>
        public void SetPositionView(Vector3 position)
        {
            txtXPosition.text = position.x.ToString();
            txtYPosition.text = position.y.ToString();
        }

        /// <summary>
        /// Set rotation value of the inspector view
        /// Rotation is only in one axis z
        /// </summary>
        /// <param name="position"></param>
        public void SetRotationView(float zRotation)
        {
            txtZRotation.text = zRotation.ToString();

        }

        //------------------------------
        //Button Events
        //------------------------------

        private void OnRotateButtonClicked()
        {
            EnableButtonOutlineOnly("ROTATE");
        }

        //----------------------
        //Helpers
        //----------------------

        /// <summary>
        /// Given a group of buttons enable only one within the group
        /// </summary>
        private void EnableButtonOutlineOnly(string btnName)
        {
            //switch (btnName)
            //{
            //    case "DRAG":
            //        {
            //            DisableAllButtonsInGroup("TOOL");
            //            DisableAllButtonsInGroup("SHAPE");
            //            EnableButtonOutline(btnDrag, true);
            //        }
            //        break;
            //    case "MOVE":
            //        {
            //            DisableAllButtonsInGroup("TOOL");
            //            DisableAllButtonsInGroup("SHAPE");
            //            EnableButtonOutline(btnMove, true);
            //        }
            //        break;
            //    case "ROTATE":
            //        {
            //            DisableAllButtonsInGroup("TOOL");
            //            DisableAllButtonsInGroup("SHAPE");
            //            EnableButtonOutline(btnRotate, true);
            //        }
            //        break;
            //    case "CIRCLE":
            //        {
            //            DisableAllButtonsInGroup("TOOL");
            //            DisableAllButtonsInGroup("SHAPE");
            //            EnableButtonOutline(btnCircle, true);
            //        }
            //        break;
            //    case "RECT":
            //        {
            //            DisableAllButtonsInGroup("TOOL");
            //            DisableAllButtonsInGroup("SHAPE");
            //            EnableButtonOutline(btnRect, true);
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        private void DisableAllButtonsInGroup(string groupName)
        {
            //switch (groupName)
            //{
            //    case "SHAPE":
            //        {
            //            EnableButtonOutline(btnCircle, false);
            //            EnableButtonOutline(btnRect, false);
            //        }
            //        break;
            //    case "TOOL":
            //        {
            //            EnableButtonOutline(btnDrag, false);
            //            EnableButtonOutline(btnRotate, false);
            //            EnableButtonOutline(btnMove, false);
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }

        void EnableButtonOutline(GameObject button, bool enabled)
        {
            button.transform.Find("outline").gameObject.SetActive(enabled);
        }

    }
}