using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace SandboxGame
{

    public class ToolFixedJoint : ToolBase
    {
        enum ToolState { NONE, SELECT_A, SELECT_B }

        ToolState toolState;

        EditController editController;
        TouchManager tManager;
        ObjectManager oManager;

        /// <summary>
        /// mouse position with z value of 0
        /// </summary>
        private Vector3 mousePos;

        private bool isDragging;
        private Vector3 currentDragOffset;

        bool isMouseDown;

        ObjectBase objectA;
        ObjectBase objectB;
        Vector3 pivotA;
        Vector3 pivotB;

        public ToolFixedJoint(EditController editC)
        {
            editController = editC;
            tManager = editController.tManager;
            oManager = editC.oManager;

        }

        public override void OnToolDeselected()
        {
            
        }

        public override void OnToolSelected()
        {
            toolState = ToolState.SELECT_A;
            ToastNotification.Show("Click on an object to select it.");
        }

        public override void OnToolUpdate()
        {
            mousePos = Input.mousePosition;
            mousePos.z = 0;

            //Debug.Log("Edit Move Tool Update");
            //ProcessInputs();

            switch (toolState)
            {
                case ToolState.NONE:
                    break;
                case ToolState.SELECT_A:
                    ProcessInputsA();
                    break;
                case ToolState.SELECT_B:
                    ProcessInputsB();
                    break;
                default:
                    break;
            }

        }

        public void SpawnJoint()
        {
            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnFixedJoint(objectA, objectB, pivotA, pivotB);

            }));

        }

        //When in state A 
        void ProcessInputsA()
        {
            //Verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {
                isMouseDown = true;


            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isMouseDown)
                {
                    isMouseDown = false;
                    var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

                    if (rB != null)//If clicked on a body
                    {
                        //Selected objectA
                        objectA = rB.GetComponent<ObjectBase>();
                        toolState = ToolState.SELECT_B;
                        pivotA = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pivotA.z = 0;

                        ToastNotification.Show("Click on second object.");

                        //Debug.Log("Selected object" + objectA.transform.GetInstanceID());
                    }
                }
            }
        }

        //When in state B
        void ProcessInputsB()
        {
            //Verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {
                isMouseDown = true;

            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (isMouseDown)
                {
                    isMouseDown = false;
                    var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

                    if (rB != null)//If clicked on a body
                    {
                        //Selected objectA
                        objectB = rB.GetComponent<ObjectBase>();
                        //Debug.Log("Selected object" + objectB.transform.GetInstanceID());

                        pivotB = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        pivotB.z = 0;

                        SpawnJoint();

                        toolState = ToolState.SELECT_A;

                    }
                }
            }
        }

    }
}