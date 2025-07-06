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

            editController.SelectObject(null);
            editController.gizmoPanel.gameObject.SetActive(true);
            editController.gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.NONE);

            ToastNotification.Show("Start dragging from an object and release over another object to join them.");
        }

        public override void OnToolUpdate()
        {
            _mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosWorld.z = 0;

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
                var res = Resources.Load<JointVisualRope>("JointVisualRope");
                _jointVisual = Object.Instantiate(res);
                _jointVisual.Init(editController);
                _jointVisual.SetSortingLayerId(4);


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

                    if (IsJointSpawnValid(_dragStartPos, _dragEndPos))
                    {
                        SpawnJoint(_dragStartPos, _dragEndPos);
                    }
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
            else
            {
                pivotB = pointB;
            }

            //flip objects
            if (objectA == null && objectB != null)
            {
                Utilities.Swap(ref objectA, ref objectB);
                Utilities.Swap(ref pivotA, ref pivotB);
            }

            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnRopeJoint(objectA, objectB, pivotA, pivotB);

            }));

        }

        bool IsJointSpawnValid(Vector3 pointA, Vector3 pointB)
        {
            float jointLength = Vector3.Distance(pointA, pointB);

            objectA = GetObjectAtWorldPosition(pointA);
            objectB = GetObjectAtWorldPosition(pointB);

            if (jointLength >= GameManager.Instance.ConfigData.RopeJointMinLength && objectA != objectB)
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
