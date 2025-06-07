using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SandboxGame
{
    public class PNL_Gizmo : MonoBehaviour
    {
        public enum GizmoType { NONE, MOVE, ROTATE };

        public Canvas canvasRef;
        public GameObject MoveGizmo;
        public Image MoveGizmoImage;
        public GameObject RotateGizmo;
        public Image RotateGizmoImage;

        public Action<BaseEventData> OnMoveToolDragBegin;
        public Action<BaseEventData> OnMoveToolDrag;
        public Action<BaseEventData> OnMoveToolDragEnd;

        public Action<BaseEventData> OnRotateToolDragBegin;
        public Action<BaseEventData> OnRotateToolDrag;
        public Action<BaseEventData> OnRotateToolDragEnd;

        private void Awake()
        {
            MoveGizmoImage.alphaHitTestMinimumThreshold = 0.01f;
            RotateGizmoImage.alphaHitTestMinimumThreshold = 0.01f;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void EnableGizmoOnly(GizmoType type)
        {
            switch (type)
            {
                case GizmoType.NONE:
                    break;
                case GizmoType.MOVE:
                    MoveGizmo.gameObject.SetActive(true);
                    RotateGizmo.gameObject.SetActive(false);
                    break;
                case GizmoType.ROTATE:
                    MoveGizmo.gameObject.SetActive(false);
                    RotateGizmo.gameObject.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        //--------------
        // Move tool
        //--------------

        public void OnDragBeginMoveCallback(BaseEventData eventData)
        {
            OnMoveToolDragBegin?.Invoke(eventData);
        }

        public void OnDragMoveCallback(BaseEventData eventData)
        {
            OnMoveToolDrag?.Invoke(eventData);
        }

        public void OnDragEndMoveCallback(BaseEventData eventData)
        {
            OnMoveToolDragEnd?.Invoke(eventData);
        }

        //----------------
        // Drag tool
        //----------------

        public void OnRotateToolDragBeginCallback(BaseEventData eventData)
        {
            OnRotateToolDragBegin?.Invoke(eventData);
        }

        public void OnRotateToolDragCallback(BaseEventData eventData)
        {
            OnRotateToolDrag?.Invoke(eventData);
        }

        public void OnRotateToolDragEndCallback(BaseEventData eventData)
        {
            OnRotateToolDragEnd?.Invoke(eventData);
        }
    }
}
