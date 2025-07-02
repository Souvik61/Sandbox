using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

        private Image RedDot1;
        private Image RedDot2;

        private GraphicRaycaster raycaster;

        public ToolDrawRect(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            //Subscribe to event functions
            //tManager.OnDragStarted += OnStartedDraging;
            //tManager.OnDragEnded += OnEndDraging;

            raycaster = Object.FindObjectOfType<GraphicRaycaster>();
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

        void ProcessInputs()
        {
            // verify pointer is not on top of GUI; if it is, return
            //if (EventSystem.current.IsPointerOverGameObject()) return;

            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                if (RedDot1 != null && RedDot1.gameObject != null)
                {
                    Object.Destroy(RedDot1.gameObject);
                    Object.Destroy(RedDot2.gameObject);
                }
                
                var mousePos = Input.mousePosition;
                mousePos.z = 0;
                
                RedDot1 = Object.Instantiate(editController.gizmoPanel.RedDotGizmoPrefab, editController.gizmoPanel.transform);
                RedDot2 = Object.Instantiate(editController.gizmoPanel.RedDotGizmoPrefab, editController.gizmoPanel.transform);
                RedDot1.gameObject.SetActive(true);
                RedDot2.gameObject.SetActive(true);
                RedDot1.transform.position = mousePos;
                RedDot2.transform.position = mousePos;

                _dragStartPos = mousePosWorld;
                _isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isDragging)
                {
                    _isDragging = false;

                    _dragEndPos = mousePosWorld;

                    if (UICheck())
                    {
                        //Spawn object
                        CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
                        {
                            Debug.Log("Call next frame");

                            oManager.SpawnRect(_dragStartPos, _dragEndPos, editController.ColorManager.GetRandomColor());

                        }));
                    }
                }
            }

            if (_isDragging)
            {
                var mousePos = Input.mousePosition;
                mousePos.z = 0;
                if (RedDot2 != null)
                {
                    RedDot2.transform.position = mousePos;
                }
            }
        }

        //------------------
        //Helper
        //------------------

        /// <summary>
        /// Returns false if over a UI and should abort
        /// </summary>
        /// <returns></returns>
        bool UICheck()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(pointerData, results);

                if (results.Count > 0)
                {
                    GameObject hovered = results[0].gameObject;

                    if (hovered.CompareTag("graphicexclude"))
                    {
                        return true;
                    }
                }
                return false;
            }

            return true;
        }
    }
}