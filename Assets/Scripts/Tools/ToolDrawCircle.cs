using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{

    public class ToolDrawCircle : ToolBase
    {
        EditController editController;
        TouchManager tManager;
        ObjectManager oManager;

        //Private

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        public ToolDrawCircle(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            tManager.OnDragStarted += OnStartedDraging;
            tManager.OnDragEnded += OnEndDraging;
        }



        ~ToolDrawCircle()
        {

            //Debug.Log("Draw circle destroyed");

        }

        public override void OnToolDeselected()
        {
            tManager.OnDragStarted -= OnStartedDraging;
            tManager.OnDragEnded -= OnEndDraging;

            tManager.SetDrawType(ShapeDrawType.NONE);


            Debug.Log("Circle Tool Deselected");
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Circle Tool Selected");

            // Set touch managers gizmo to rect
            tManager.SetDrawType(ShapeDrawType.CIRCLE);
        }

        public override void OnToolUpdate()
        {
            //Debug.Log("Drawing Circle");


        }

        //----------
        //Events
        //----------

        private void OnEndDraging()
        {
            _dragEndPos = Camera.main.ScreenToWorldPoint(tManager.mousePositionScreen);
            _dragEndPos.z = 0;

            var dType = tManager.prevDrawType;

            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnCircle(_dragStartPos, _dragEndPos);

            }));
        }

        private void OnStartedDraging()
        {
            _dragStartPos = Camera.main.ScreenToWorldPoint(tManager.startMousePositionScreen);
            _dragStartPos.z = 0;
        }

        //------------------
        //Helper
        //------------------

        void ProcessInputs()
        {

        }
    }
}