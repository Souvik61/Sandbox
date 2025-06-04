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
        public Canvas canvasRef;
        public GameObject MoveGizmo;
        public Image MoveGizmoImage;

        public Action<BaseEventData> OnMoveToolDragBegin;
        public Action<BaseEventData> OnMoveToolDrag;
        public Action<BaseEventData> OnMoveToolDragEnd;


        private void Awake()
        {
            MoveGizmoImage.alphaHitTestMinimumThreshold = 0.01f;
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

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
    }
}
