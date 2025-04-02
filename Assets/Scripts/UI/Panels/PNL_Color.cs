using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    /// <summary>
    /// The color selector panel
    /// </summary>
    public class PNL_Color : MonoBehaviour
    {

        [Header("UI References")]
        public FlexibleColorPicker colorPicker;

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
            //typeText.text = obj.type.ToString();

        }

        /// <summary>
        /// Live link Inspector to show details of that object
        /// </summary>
        /// <param name="obj"></param>
        public void LinkView(ObjectBase obj)
        {
            //if (obj != null)
            //{
            //    typeText.text = obj.type.ToString();
            //}
            //else
            //{
            //    typeText.text = "None";
            //    txtXPosition.text = Constants.TEXTNA;
            //    txtYPosition.text = Constants.TEXTNA;
            //    txtZRotation.text = Constants.TEXTNA;
            //}

        }

        //------------------
        //Setters
        //------------------

        /// <summary>
        /// Set rotation value of the inspector view
        /// Rotation is only in one axis z
        /// </summary>
        /// <param name="position"></param>
        public void SetColorView(Color color)
        {
            colorPicker.color = color;

        }

        //------------------------------
        //Button Events
        //------------------------------

      

        //----------------------
        //Helpers
        //----------------------

        void EnableButtonOutline(GameObject button, bool enabled)
        {
            button.transform.Find("outline").gameObject.SetActive(enabled);
        }

    }
}