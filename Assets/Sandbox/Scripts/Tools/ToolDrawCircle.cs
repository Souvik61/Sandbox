using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

        bool _isDragging;
        bool _validDrag;

        /// <summary>
        /// mouse position with z value of 0
        /// </summary>
        private Vector3 mousePosWorld;

        public ToolDrawCircle(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            //tManager.OnDragStarted += OnStartedDraging;
            //tManager.OnDragEnded += OnEndDraging;

            _validDrag = false;
        }



        ~ToolDrawCircle()
        {

            //Debug.Log("Draw circle destroyed");

        }

        public override void OnToolDeselected()
        {
            //tManager.OnDragStarted -= OnStartedDraging;
            //tManager.OnDragEnded -= OnEndDraging;

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

            mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosWorld.z = 0;

            //Debug.Log("Edit Move Tool Update");
            ProcessInputs();


        }

        public override bool ShouldBlockOtherEvents()
        {
            return true;
        }

        //----------
        //Events
        //----------

        private void OnEndDraging()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                _validDrag = false;
                return;
            }

            if (!_validDrag) return;

            _dragEndPos = Camera.main.ScreenToWorldPoint(tManager.mousePositionScreen);
            _dragEndPos.z = 0;

            var dType = tManager.prevDrawType;

            //Spawn object
            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
            {
                Debug.Log("Call next frame");

                oManager.SpawnCircle(_dragStartPos, _dragEndPos, editController.ColorManager.GetRandomColor());

            }));
        }

        private void OnStartedDraging()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                _validDrag = false;
                return;    
            }

            _dragStartPos = Camera.main.ScreenToWorldPoint(tManager.startMousePositionScreen);
            _dragStartPos.z = 0;
            _validDrag = true;
            Debug.Log("Started dragging");

        }

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

                        oManager.SpawnCircle(_dragStartPos, _dragEndPos, editController.ColorManager.GetRandomColor());

                    }));

                }
            }
        }
    }
}