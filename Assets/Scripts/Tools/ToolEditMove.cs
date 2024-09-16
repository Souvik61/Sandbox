using System.Collections;
using System.Collections.Generic;
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

        public ToolEditMove(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            tManager.OnDragStarted += OnStartedDraging;
            tManager.OnDragEnded += OnEndDraging;
        }

        ~ToolEditMove()
        {

        }

        public override void OnToolDeselected()
        {

            tManager.OnDragStarted -= OnStartedDraging;
            tManager.OnDragEnded -= OnEndDraging;

            Debug.Log("Edit Move Tool Deselected");
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Edit Move Tool Selected");

            //Set touch managers gizmo to rect
            //tManager.SetDrawType(ShapeDrawType.RECT);
        }

        public override void OnToolUpdate()
        {
            mousePos = Input.mousePosition;
            mousePos.z = 0;
            //Debug.Log("Edit Move Tool Update");
            ProcessInputs();


            if (isDragging)
            {
                currentDraggedObject.transform.position = Camera.main.ScreenToWorldPoint(mousePos) + currentDragOffset;
            }

        }

        //----------
        //Events
        //----------

        void OnStartedDraging()
        {


        }

        void OnEndDraging()
        {


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

                if (rB != null)//If clicked on a body
                {
                    editController.SelectObject(rB.GetComponent<ObjectBase>());

                    isDragging = true;
                    currentDragOffset = rB.transform.position - Camera.main.ScreenToWorldPoint(mousePos);
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