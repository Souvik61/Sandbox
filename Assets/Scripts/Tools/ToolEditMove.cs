using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("Edit Move Tool Update");
            ProcessInputs();

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
            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {
                var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if (rB != null)
                {
                    editController.SelectObject(rB.GetComponent<ObjectBase>());
                }

            }
        }
        
    }
}