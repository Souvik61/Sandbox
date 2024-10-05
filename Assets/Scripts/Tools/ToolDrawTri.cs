using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class ToolDrawTri : ToolBase
    {
        EditController editController;
        TouchManager tManager;
        ObjectManager oManager;

        //Private

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        public ToolDrawTri(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            tManager.OnDragStarted += OnStartedDraging;
            tManager.OnDragEnded += OnEndDraging;
        }

        ~ToolDrawTri()
        {

            //Debug.Log("Draw rect destroyed");

        }

        public override void OnToolDeselected()
        {

            tManager.OnDragStarted -= OnStartedDraging;
            tManager.OnDragEnded -= OnEndDraging;

            tManager.SetDrawType(ShapeDrawType.NONE);

            Debug.Log("Triangle Draw Tool Deselected");
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Triangle Draw Tool Selected");

            //Set touch managers gizmo to rect
            tManager.SetDrawType(ShapeDrawType.TRI);
        }

        public override void OnToolUpdate()
        {
            //Debug.Log("Drawing Rect");
            ProcessInputs();

        }

        //----------
        //Events
        //----------

        void OnStartedDraging()
        {
            _dragStartPos = Camera.main.ScreenToWorldPoint(tManager.startMousePositionScreen);
            _dragStartPos.z = 0;

        }

        void OnEndDraging()
        {

            _dragEndPos = Camera.main.ScreenToWorldPoint(tManager.mousePositionScreen);
            _dragEndPos.z = 0;

            var dType = tManager.prevDrawType;

            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnTriangle(_dragStartPos, _dragEndPos);

            }));
        }

        //------------------
        //Helper
        //------------------

        void ProcessInputs()
        {

        }
    }
}