using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
