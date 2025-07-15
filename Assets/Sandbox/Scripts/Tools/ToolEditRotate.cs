using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI.Table;

namespace SandboxGame
{
    public class ToolEditRotate : ToolBase
    {
        EditController editController;
        TouchManager tManager;
        ObjectManager oManager;

        //Private

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        /// <summary>
        /// mouse position with z value of 0
        /// </summary>
        private Vector3 mousePos;

        private bool isDragging;
        private Vector3 startRotationVec;
        private Vector3 currRotationVec;
        private float currAngleDelta;
        private float startRotationZ;
        private ObjectBase currentDraggedObject;

        public PNL_Gizmo gizmoPanel;


        public ToolEditRotate(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            gizmoPanel = editC.gizmoPanel;

        }

        ~ToolEditRotate()
        {

        }

        //----------
        //Events
        //----------

        public override void OnToolDeselected()
        {
            //Debug.Log("Edit Rotate Tool Deselected");

            gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.NONE);
            gizmoPanel.gameObject.SetActive(false);
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            //Debug.Log("Edit Rotate Tool Selected");

            //Set touch managers gizmo to rect
            //tManager.SetDrawType(ShapeDrawType.RECT);

            editController.gizmoPanel.OnRotateToolDragBegin += OnRotateGizmoDragStartCallback;
            editController.gizmoPanel.OnRotateToolDrag += OnRotateGizmoDragCallback;
            editController.gizmoPanel.OnRotateToolDragEnd += OnRotateGizmoDragEndCallback;

            if (editController.SelectedObject && editController.SelectedObject is ObjectPrimitive)
            {
                gizmoPanel.gameObject.SetActive(true);
                gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.ROTATE);

                Vector3 rot = editController.SelectedObject.transform.eulerAngles;

                gizmoPanel.RotateGizmo.transform.eulerAngles = rot;
            }
            else
            {
                gizmoPanel.gameObject.SetActive(false);

            }

        }

        public override void OnToolUpdate()
        {
            mousePos = Input.mousePosition;
            mousePos.z = 0;

            // update the gizmo
            if (editController.SelectedObject && editController.SelectedObject is ObjectPrimitive)
            {
                // set the move gizmo transform over object
                RectTransform moveGizmoTrans = gizmoPanel.RotateGizmo.GetComponent<RectTransform>();
                Vector3 screenPos = Camera.main.WorldToScreenPoint(editController.SelectedObject.transform.position);
                screenPos.z = 0;
                moveGizmoTrans.position = screenPos;
            }
        }

        public override void OnObjectSelected()
        {
            if (editController.SelectedObject && editController.SelectedObject is ObjectPrimitive)
            {
                gizmoPanel.gameObject.SetActive(true);
                gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.ROTATE);
                // set the move gizmo transform over object
                RectTransform rotateGizmoTrans = gizmoPanel.RotateGizmo.GetComponent<RectTransform>();
                Vector3 screenPos = Camera.main.WorldToScreenPoint(editController.SelectedObject.transform.position);
                screenPos.z = 0;
                rotateGizmoTrans.position = screenPos;
                rotateGizmoTrans.transform.eulerAngles = editController.SelectedObject.transform.eulerAngles;
            }
            else
            {
                gizmoPanel.gameObject.SetActive(false);

            }

        }

        public override bool ShouldBlockOtherEvents()
        {
            return isDragging;
        }

        //------------------------
        // Events from PNL_Gizmo
        //------------------------

        public void OnRotateGizmoDragStartCallback(BaseEventData eventData)
        {
            //Debug.Log("Drag start");

            var rectTransform = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            PointerEventData ptData = (PointerEventData)eventData;
            //
            //offset = rectTransform.position - new Vector3(ptData.position.x, ptData.position.y, 0);

            if (editController.SelectedObject && editController.SelectedObject is ObjectPrimitive)
            {
                startRotationZ = editController.SelectedObject.transform.eulerAngles.z;

                startRotationVec = new Vector3(ptData.position.x, ptData.position.y, 0) - gizmoPanel.RotateGizmo.transform.position;

            }
            isDragging = true;

        }

        public void OnRotateGizmoDragCallback(BaseEventData eventData)
        {
            //Debug.Log("Drag");

            PointerEventData ptData = (PointerEventData)eventData;

            if (editController.SelectedObject && editController.SelectedObject is ObjectPrimitive)
            {

                Vector3 ptrPos = ptData.position;

                currRotationVec = ptrPos - gizmoPanel.RotateGizmo.transform.position;
                currRotationVec.z = 0;
                currAngleDelta = Vector3.SignedAngle(startRotationVec, currRotationVec, Vector3.forward);

                editController.SelectedObject.transform.eulerAngles = new Vector3(0, 0, startRotationZ + currAngleDelta);

                gizmoPanel.RotateGizmo.transform.eulerAngles = new Vector3(0, 0, startRotationZ + currAngleDelta);

            }
        }

        public void OnRotateGizmoDragEndCallback(BaseEventData eventData)
        {
            //Debug.Log("Drag end");
            CoroutineExtensions.NextFrame(editController, () => { isDragging = false; });
        }

        //------------------
        //Helper
        //------------------


    }
}