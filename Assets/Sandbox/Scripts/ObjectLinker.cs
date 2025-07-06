using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


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

        private EditController editC;

        private PNL_Gizmo gizmoPanel;
        private Vector3 offset;

        private ObjectBase _colorTargetObject;

        public void Init(EditController editController)
        {
            editC = editController;
            gizmoPanel = editController.gizmoPanel;

            //gizmoPanel.OnMoveToolDragBegin += OnMoveGizmoDragStartCallback;
            //gizmoPanel.OnMoveToolDrag += OnMoveGizmoDragCallback;
            //gizmoPanel.OnMoveToolDragEnd += OnMoveGizmoDragEndCallback;
        }

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
            if (obj == null)
            {
                _colorTargetObject = null;
                viewColor = colorPanel;
                colorPanel.colorPicker.onColorChange.RemoveAllListeners();
            }
            else
            {
                _colorTargetObject = obj;
                viewColor = colorPanel;
                
                colorPanel.colorPicker.onColorChange.RemoveAllListeners();

                colorPanel.colorPicker.color = obj.GetColor();

                colorPanel.colorPicker.onColorChange.AddListener(OnColorChange);
            }
        }

        void OnColorChange(Color color)
        {
            _lastColorChangeValue = color;
            _colorTargetObject.SetColor(_lastColorChangeValue);
        }


        //------------------------
        // Events from PNL_Gizmo
        //------------------------

        public void OnMoveGizmoDragStartCallback(BaseEventData eventData)
        {
            Debug.Log("Drag start");

            var rectTransform = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            PointerEventData ptData = (PointerEventData)eventData;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, ptData.position, Camera.main, out offset);
            offset = rectTransform.position - new Vector3(ptData.position.x, ptData.position.y, 0);
        }

        public void OnMoveGizmoDragCallback(BaseEventData eventData)
        {
            Debug.Log("Drag");

            PointerEventData ptData = (PointerEventData)eventData;

            //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(gizmoPanel.canvasRef.transform as RectTransform, ptData.position, Camera.main, out Vector2 localPoint))
            //{
            //    //rectTransform.anchoredPosition = localPoint + offset;
            //}
            RectTransform moveGizmoTrans = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            moveGizmoTrans.position = new Vector3(ptData.position.x, ptData.position.y, 0) + offset;

            if (targetObject)
            {
                Vector3 targetObjectPos = Camera.main.ScreenToWorldPoint(moveGizmoTrans.position);
                targetObjectPos.z = 0;
                targetObject.transform.position = targetObjectPos;
            
            }
        }

        public void OnMoveGizmoDragEndCallback(BaseEventData eventData)
        {
            Debug.Log("Drag end");
        }

    }
}