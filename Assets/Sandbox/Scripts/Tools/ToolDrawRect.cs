using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SandboxGame
{
    public class ToolDrawRect : ToolBase
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
        private Vector3 mousePosWorld;

        bool _isDragging;

        public ToolDrawRect(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            //tManager.OnDragStarted += OnStartedDraging;
            //tManager.OnDragEnded += OnEndDraging;
        }

        ~ToolDrawRect()
        {

            //Debug.Log("Draw rect destroyed");

        }

        public override void OnToolDeselected()
        {

            //tManager.OnDragStarted -= OnStartedDraging;
            //tManager.OnDragEnded -= OnEndDraging;

            tManager.SetDrawType(ShapeDrawType.NONE);

            Debug.Log("Rect Draw Tool Deselected");
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            Debug.Log("Rect Draw Tool Selected");

            //Set touch managers gizmo to rect
            tManager.SetDrawType(ShapeDrawType.RECT);
        }

        public override void OnToolUpdate()
        {
            mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorld.z = 0;

            //Debug.Log("Drawing Rect");
            ProcessInputs();

        }

        public override bool ShouldBlockOtherEvents()
        {
            return true;
        }

        //----------
        //Events
        //----------


        //------------------
        //Helper
        //------------------

        void ProcessInputs()
        {
            // verify pointer is not on top of GUI; if it is, return
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0))
            {
                _dragStartPos = mousePosWorld;
                _isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isDragging)
                {
                    _isDragging = false;

                    _dragEndPos = mousePosWorld;

                    //Spawn object
                    CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
                    {
                        Debug.Log("Call next frame");

                        oManager.SpawnRect(_dragStartPos, _dragEndPos, editController.ColorManager.GetRandomColor());

                    }));

                }
            }
        }
    }
}