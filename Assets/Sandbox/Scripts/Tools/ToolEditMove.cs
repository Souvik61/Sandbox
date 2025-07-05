using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SandboxGame
{
    public class ToolEditMove : ToolBase
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
        private Vector3 currentDragOffset;
        private ObjectBase currentDraggedObject;

        public PNL_Gizmo gizmoPanel;

        private Vector3 offset;

        public ToolEditMove(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            gizmoPanel = editC.gizmoPanel;

        }

        ~ToolEditMove()
        {

        }

        public override void OnToolDeselected()
        {
            //Debug.Log("Edit Move Tool Deselected");

            gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.NONE);
            gizmoPanel.gameObject.SetActive(false);
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            //Debug.Log("Edit Move Tool Selected");

            //Set touch managers gizmo to rect
            //tManager.SetDrawType(ShapeDrawType.RECT);

            editController.gizmoPanel.OnMoveToolDragBegin += OnMoveGizmoDragStartCallback;
            editController.gizmoPanel.OnMoveToolDrag += OnMoveGizmoDragCallback;
            editController.gizmoPanel.OnMoveToolDragEnd += OnMoveGizmoDragEndCallback;

            if (editController.SelectedObject && editController.SelectedObject is ObjectPrimitive)
            {
                gizmoPanel.gameObject.SetActive(true);
                gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.MOVE);
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
                RectTransform moveGizmoTrans = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
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
                gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.MOVE);
                // set the move gizmo transform over object
                RectTransform moveGizmoTrans = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
                Vector3 screenPos = Camera.main.WorldToScreenPoint(editController.SelectedObject.transform.position);
                screenPos.z = 0;
                moveGizmoTrans.position = screenPos;
            }
            else
            {
                gizmoPanel.gameObject.SetActive(false);

            }

        }

        public override bool ShouldBlockOtherEvents()
        {
            return false;
        }

        //------------------------
        // Events from PNL_Gizmo
        //------------------------

        public void OnMoveGizmoDragStartCallback(BaseEventData eventData)
        {
            //Debug.Log("Drag start");

            var rectTransform = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            PointerEventData ptData = (PointerEventData)eventData;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, ptData.position, Camera.main, out offset);
            offset = rectTransform.position - new Vector3(ptData.position.x, ptData.position.y, 0);
        }

        public void OnMoveGizmoDragCallback(BaseEventData eventData)
        {
            //Debug.Log("Drag");

            PointerEventData ptData = (PointerEventData)eventData;

            RectTransform moveGizmoTrans = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            moveGizmoTrans.position = new Vector3(ptData.position.x, ptData.position.y, 0) + offset;

            if (editController.SelectedObject)
            {
                Vector3 targetObjectPos = Camera.main.ScreenToWorldPoint(moveGizmoTrans.position);
                targetObjectPos.z = 0;
                editController.SelectedObject.transform.position = targetObjectPos;

            }
        }

        public void OnMoveGizmoDragEndCallback(BaseEventData eventData)
        {
            //Debug.Log("Drag end");
        }

        //------------------
        //Helper
        //------------------

    }
}