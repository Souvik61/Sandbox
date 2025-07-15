using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        private Image RedDot1;
        private Image RedDot2;

        private GraphicRaycaster raycaster;

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
            raycaster = editController.UICanvas.GetComponent<GraphicRaycaster>();

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
        }

        public override void OnToolSelected()
        {
            //throw new System.NotImplementedException();
            //Debug.Log("Circle Tool Selected");

            editController.SelectObject(null);

            editController.gizmoPanel.gameObject.SetActive(true);
            editController.gizmoPanel.EnableGizmoOnly(PNL_Gizmo.GizmoType.NONE);

            // Set touch managers gizmo to rect
            tManager.SetDrawType(ShapeDrawType.CIRCLE);
            tManager.HideGizmoType(ShapeDrawType.CIRCLE, true);
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
                tManager.HideGizmoType(ShapeDrawType.CIRCLE, false);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (_isDragging)
                {
                    _isDragging = false;

                    _dragEndPos = mousePosWorld;

                    tManager.HideGizmoType(ShapeDrawType.CIRCLE, true);


                    if (UICheck())
                    {
                        if (SpawnCheck(_dragStartPos, _dragEndPos))
                        {
                            //Spawn object
                            CoroutineExtensions.StartGlobalCoroutine(CoroutineExtensions.NextFrameRoutine(() =>
                            {
                                //Debug.Log("Call next frame");
                                oManager.SpawnCircle(_dragStartPos, _dragEndPos, editController.ColorManager.GetRandomColor());
                            }));
                        }
                    }

                    if (RedDot1.gameObject != null)
                    {
                        Object.Destroy(RedDot1.gameObject);
                    }
                    if (RedDot2.gameObject != null)
                    {
                        Object.Destroy(RedDot2.gameObject);
                    }
                }
            }

            if (_isDragging && UICheck())
            {
                var mousePos = Input.mousePosition;
                mousePos.z = 0;
                tManager.SetCircleGizmoInput(_dragStartPos, mousePosWorld);
                if (RedDot2 != null)
                {
                    RedDot2.transform.position = mousePos;
                }
            }
        }

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

        /// <summary>
        /// Check if spawn is valid
        /// Call after UICheck
        /// </summary>
        /// <returns></returns>
        bool SpawnCheck(Vector3 dragStartPos, Vector3 dragEndPos)
        {
            var config = GameManager.Instance.ConfigData;
            float dist = (dragEndPos - dragStartPos).magnitude;

            return dist >= config.MinCircleRadius;
        }
    }
}