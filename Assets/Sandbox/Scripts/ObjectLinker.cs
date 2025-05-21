using System.Collections.Generic;
using UnityEngine;


namespace SandboxGame
{

    /// <summary>
    /// Class responsible for linking an object and a inspector view
    /// </summary>
    public class ObjectLinker : MonoBehaviour
    {

        public ObjectBase targetObject;
        public PNL_Inspector viewInspector;
        public PNL_Color viewColor;


        private Color _lastColorChangeValue;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void Update()
        {
            //If a target object is present
            if (targetObject != null)
            {
                //viewInspector.SetPositionView(targetObject.transform.position);
                //viewInspector.SetRotationView(targetObject.transform.eulerAngles.z);

                viewInspector.UpdatePropertyValues();

            }
        }

        private void LateUpdate()
        {
            //if (targetObject && viewColor)
            //{
            //    targetObject.SetColor(_lastColorChangeValue);
            //}
        }

        /// <summary>
        /// Link the object and inspector
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="inspector"></param>
        public void Link(ObjectBase obj, PNL_Inspector inspector)
        {
            targetObject = obj;
            viewInspector = inspector;

            inspector.Link(obj);
        }

        /// <summary>
        /// Link the object and color picker
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="inspector"></param>
        public void Link(ObjectBase obj, PNL_Color colorPanel)
        {
            targetObject = obj;
            viewColor = colorPanel;

            colorPanel.Link(obj);

            colorPanel.colorPicker.onColorChange.AddListener(OnColorChange);

        }

        void OnColorChange(Color color)
        {
            _lastColorChangeValue = color;
            targetObject.SetColor(_lastColorChangeValue);
        }
    }
}