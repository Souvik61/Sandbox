using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SandboxGame
{
    public class ToolRopeJoint : ToolBase
    {

        enum ToolState { NONE, SELECT_A, SELECT_B }

        ToolState toolState;

        EditController editController;
        ObjectManager oManager;

        /// <summary>
        /// mouse position with z value of 0
        /// </summary>
        private Vector3 _mousePosWorld;

        private bool isDragging;

        bool isMouseDown;

        ObjectBase objectA;
        ObjectBase objectB;
        Vector3 pivotA;
        Vector3 pivotB;

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        private JointVisualRope _jointVisual;

        public ToolRopeJoint(EditController editC)
        {
            editController = editC;
            oManager = editC.oManager;

        }

        public override void OnToolDeselected()
        {

        }

        public override void OnToolSelected()
        {
            toolState = ToolState.SELECT_A;
            ToastNotification.Show("Start dragging from an object and release over another object to join them.");
        }

        public override void OnToolUpdate()
        {
            _mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosWorld.z = 0;

            //Debug.Log("Edit Move Tool Update");
            ProcessInputs();

            //switch (toolState)
            //{
            //    case ToolState.NONE:
            //        break;
            //    case ToolState.SELECT_A:
            //        ProcessInputsA();
            //        break;
            //    case ToolState.SELECT_B:
            //        ProcessInputsB();
            //        break;
            //    default:
            //        break;
            //}

        }

        public override bool ShouldBlockOtherEvents()
        {
            return true;
        }

        void ProcessInputs()
        {
            // verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                var res = Resources.Load<JointVisualRope>("JointVisualRope");
                _jointVisual = Object.Instantiate(res);

                _dragStartPos = _mousePosWorld;
                _jointVisual.pivotA.position = _mousePosWorld;

                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    isDragging = false;

                    _dragEndPos = _mousePosWorld;

                    Object.Destroy(_jointVisual.gameObject);

                    SpawnJoint(_dragStartPos, _dragEndPos);
                }
            }

            if (isDragging)
            {
                _jointVisual.pivotB.position = _mousePosWorld;

            }

        }

        public void SpawnJoint()
        {
            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnRopeJoint(objectA, objectB, pivotA, pivotB);

            }));

        }

        public void SpawnJoint(Vector3 pointA, Vector3 pointB)
        {
            objectA = GetObjectAtWorldPosition(pointA);
            objectB = GetObjectAtWorldPosition(pointB);

            if (objectA == null && objectB == null)
            {
                return;
            }

            if (objectA)
            {
                pivotA = objectA.transform.InverseTransformPoint(pointA);
            }

            if (objectB)
            {
                pivotB = objectB.transform.InverseTransformPoint(pointB);
            }

            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnRopeJoint(objectA, objectB, pivotA, pivotB);

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

        /// <summary>
        /// Given a position find underlying object
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        ObjectPrimitive GetObjectAtWorldPosition(Vector3 pos)
        {
            var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(pos, 1 << LayerMask.NameToLayer("Object"));

            return rB.GetComponent<ObjectPrimitive>();

        }
    }
}
