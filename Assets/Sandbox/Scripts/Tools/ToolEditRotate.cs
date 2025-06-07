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
            Debug.Log("Edit Rotate Tool Deselected");
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Edit Rotate Tool Selected");

            //Set touch managers gizmo to rect
            //tManager.SetDrawType(ShapeDrawType.RECT);

            editController.gizmoPanel.OnRotateToolDragBegin += OnRotateGizmoDragStartCallback;
            editController.gizmoPanel.OnRotateToolDrag += OnRotateGizmoDragCallback;
            editController.gizmoPanel.OnRotateToolDragEnd += OnRotateGizmoDragEndCallback;

            if (editController.SelectedObject)
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

            //ProcessInputs();


            //if (isDragging)
            //{
            //    currRotationVec = Camera.main.ScreenToWorldPoint(mousePos) - currentDraggedObject.transform.position;
            //    currRotationVec.z = 0;
            //    currAngleDelta = Vector3.SignedAngle(startRotationVec, currRotationVec, Vector3.forward);
            //
            //    currentDraggedObject.transform.eulerAngles = new Vector3(0, 0, startRotationZ + currAngleDelta);
            //}
            //
            ////Debug
            //if (currentDraggedObject)
            //{
            //    Debug.DrawLine(currentDraggedObject.transform.position, currentDraggedObject.transform.position + startRotationVec, Color.red);
            //    Debug.DrawLine(currentDraggedObject.transform.position, currentDraggedObject.transform.position + currRotationVec, Color.green);
            //    //Debug.Log(startRotationVec.ToString() +" - "+ currRotationVec.ToString() + " Angle: " + currAngleDelta);
            //}

            // update the gizmo
            if (editController.SelectedObject)
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
            if (editController.SelectedObject)
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
            Debug.Log("Drag start");

            var rectTransform = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            PointerEventData ptData = (PointerEventData)eventData;
            //
            //offset = rectTransform.position - new Vector3(ptData.position.x, ptData.position.y, 0);

            if (editController.SelectedObject)
            {
                startRotationZ = editController.SelectedObject.transform.eulerAngles.z;

                startRotationVec = new Vector3(ptData.position.x, ptData.position.y, 0) - gizmoPanel.RotateGizmo.transform.position;

            }
            isDragging = true;

        }

        public void OnRotateGizmoDragCallback(BaseEventData eventData)
        {
            Debug.Log("Drag");

            PointerEventData ptData = (PointerEventData)eventData;

            //RectTransform moveGizmoTrans = gizmoPanel.MoveGizmo.GetComponent<RectTransform>();
            //moveGizmoTrans.position = new Vector3(ptData.position.x, ptData.position.y, 0) + offset;
            //
            //if (editController.SelectedObject)
            //{
            //    Vector3 targetObjectPos = Camera.main.ScreenToWorldPoint(moveGizmoTrans.position);
            //    targetObjectPos.z = 0;
            //    editController.SelectedObject.transform.position = targetObjectPos;
            //
            //}

            if (editController.SelectedObject)
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
            Debug.Log("Drag end");
            CoroutineExtensions.NextFrame(editController, () => { isDragging = false; });
        }

        //------------------
        //Helper
        //------------------

        void ProcessInputs()
        {
            // Verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {
                var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

                if (rB != null)//If clicked over a body
                {
                    editController.SelectObject(rB.GetComponent<ObjectBase>());

                    isDragging = true;
                    startRotationZ = rB.transform.eulerAngles.z;
                    startRotationVec = Camera.main.ScreenToWorldPoint(mousePos) - rB.transform.position;
                    startRotationVec.z = 0;
                    currentDraggedObject = rB.GetComponent<ObjectBase>();
                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                currentDraggedObject = null;
            }
        }

    }
}