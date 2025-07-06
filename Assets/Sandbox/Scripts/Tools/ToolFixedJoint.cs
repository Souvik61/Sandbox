using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
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
        private Vector3 mousePosWorld;

        private bool isDragging;
        private Vector3 currentDragOffset;

        bool isMouseDown;

        ObjectBase objectA;
        ObjectBase objectB;
        Vector3 pivotA;
        Vector3 pivotB;

        private JointVisual _jointVisual;

        private Vector3 pointA;
        private Vector3 pointB;

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
            
            editController.SelectObject(null);
            editController.gizmoPanel.gameObject.SetActive(true);
            editController.gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.NONE);

            ToastNotification.Show("Start dragging from an object and release over another object to join them.");
        }

        public override void OnToolUpdate()
        {
            mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorld.z = 0;

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

        public void SpawnJoint()
        {
            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnFixedJoint(objectA, objectB, pivotA, pivotB);

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
            else
            {
                pivotB = pointB;
            }

            //flip objects for better
            if (objectA == null && objectB != null)
            {
                var objC = objectA;
                objectA = objectB;
                objectB = objC;

                var pivC = pivotA;
                pivotA = pivotB;
                pivotB = pivC;
            }


            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                //Debug.Log("Call next frame");

                oManager.SpawnFixedJoint(objectA, objectB, pivotA, pivotB);

            }));

        }

        void ProcessInputs()
        {
            // verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                var res = Resources.Load<JointVisual>("JointVisual");
                _jointVisual = Object.Instantiate(res);
                _jointVisual.Init(editController);

                pointA = mousePosWorld;
                _jointVisual.pivotA.position = mousePosWorld;

                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    isDragging = false;

                    pointB = mousePosWorld;

                    Object.Destroy(_jointVisual.gameObject);

                    if (IsJointSpawnValid(pointA,pointB))
                    {
                        SpawnJoint(pointA, pointB);
                    }
                }
            }

            if (isDragging)
            {
                _jointVisual.pivotB.position = mousePosWorld;

            }

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

        bool IsJointSpawnValid(Vector3 pointA, Vector3 pointB)
        {
            float jointLength = Vector3.Distance(pointA, pointB);

            objectA = GetObjectAtWorldPosition(pointA);
            objectB = GetObjectAtWorldPosition(pointB);

            if (jointLength >= GameManager.Instance.ConfigData.JointMinLength && objectA != objectB)
            {
                return true;
            }
            else
            {
                return false;
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

            if (rB)
            {
                return rB.GetComponent<ObjectPrimitive>();
            }
            else
            {
                return null;
            }
        }

    }
}