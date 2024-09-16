using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        public ToolEditRotate(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            tManager.OnDragStarted += OnStartedDraging;
            tManager.OnDragEnded += OnEndDraging;
        }

        ~ToolEditRotate()
        {

        }

        public override void OnToolDeselected()
        {

            tManager.OnDragStarted -= OnStartedDraging;
            tManager.OnDragEnded -= OnEndDraging;

            Debug.Log("Edit Rotate Tool Deselected");
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Edit Rotate Tool Selected");

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
                currRotationVec = Camera.main.ScreenToWorldPoint(mousePos) - currentDraggedObject.transform.position;
                currRotationVec.z = 0;
                currAngleDelta = Vector3.SignedAngle(startRotationVec, currRotationVec, Vector3.forward);

                currentDraggedObject.transform.eulerAngles = new Vector3(0, 0, startRotationZ + currAngleDelta);
            }

            //Debug
            if (currentDraggedObject)
            {
                Debug.DrawLine(currentDraggedObject.transform.position, currentDraggedObject.transform.position + startRotationVec, Color.red);
                Debug.DrawLine(currentDraggedObject.transform.position, currentDraggedObject.transform.position + currRotationVec, Color.green);
                Debug.Log(startRotationVec.ToString() +" - "+ currRotationVec.ToString() + " Angle: " + currAngleDelta);
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
        //Debug
        //------------------

        


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