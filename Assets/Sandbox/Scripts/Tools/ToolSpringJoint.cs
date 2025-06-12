using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SandboxGame
{
    public class ToolSpringJoint : ToolBase
    {

        EditController editController;
        TouchManager tManager;
        ObjectManager oManager;

        ObjectBase objectA;
        ObjectBase objectB;

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        Vector3 pivotA;
        Vector3 pivotB;

        private JointVisual _jointVisual;

        /// <summary>
        /// mouse position with z value of 0
        /// </summary>
        private Vector3 mousePosWorld;

        private bool isDragging;


        public ToolSpringJoint(EditController editC)
        {
            editController = editC;
            tManager = editController.tManager;
            oManager = editC.oManager;

            //Subscribe to event functions
            tManager.OnDragStarted += OnStartedDraging;
            tManager.OnDragEnded += OnEndDraging;

        }

        public override void OnToolDeselected()
        {
            //throw new System.NotImplementedException();
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            ToastNotification.Show("Start dragging from an object and release over another object to join them.");
        }

        public override void OnToolUpdate()
        {
            //throw new System.NotImplementedException();

            mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorld.z = 0;

            //Debug.Log("Edit Move Tool Update");
            ProcessInputs();

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
                var res = Resources.Load<JointVisual>("JointVisual");
                _jointVisual = Object.Instantiate(res);

                _dragStartPos = mousePosWorld;
                _jointVisual.pivotA.position = mousePosWorld;

                isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    isDragging = false;

                    _dragEndPos = mousePosWorld;

                    Object.Destroy(_jointVisual.gameObject);

                    SpawnJoint(_dragStartPos, _dragEndPos);
                }
            }

            if (isDragging)
            {
                _jointVisual.pivotB.position = mousePosWorld;

            }

        }

        //----------
        //Events
        //----------

        private void OnEndDraging()
        {
            _dragEndPos = Camera.main.ScreenToWorldPoint(tManager.mousePositionScreen);
            _dragEndPos.z = 0;

            var dType = tManager.prevDrawType;

            var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));


            if (rB != null)//If clicked on a body
            {
                //Selected objectB
                objectB = rB.GetComponent<ObjectBase>();
                //Debug.Log("Selected object" + objectB.transform.GetInstanceID());

                pivotB = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pivotB.z = 0;

                SpawnJoint();

            }


        }

        private void OnStartedDraging()
        {
            var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

            if (rB != null)
            {
                objectA = rB.GetComponent<ObjectBase>();

                _dragStartPos = Camera.main.ScreenToWorldPoint(tManager.startMousePositionScreen);
                _dragStartPos.z = 0;
            }
            Debug.Log("Started dragging");
        }

        public void SpawnJoint()
        {
            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnSpringJoint(objectA, objectB, pivotA, pivotB);

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

                oManager.SpawnSpringJoint(objectA, objectB, pivotA, pivotB);

            }));

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
