using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SandboxGame
{
    public class ToolDrag : ToolBase
    {
        EditController editController;
        TouchManager tManager;
        ObjectManager oManager;

        //Private

        private Vector3 _dragStartPos;
        private Vector3 _dragEndPos;

        public GameObject jointVisualRect;

        Transform pt;
        Transform pt1;

        public ToolDrag(EditController editC)
        {
            editController = editC;
            this.tManager = editC.tManager;
            this.oManager = editC.oManager;

            pt = Object.Instantiate(Resources.Load<Transform>("DebugPoint"));
            pt1 = Object.Instantiate(Resources.Load<Transform>("DebugPoint"));

        }

        ~ToolDrag()
        {

        }

        public override void OnToolDeselected()
        {
            Debug.Log("Drag Tool Deselected");
            //editController.dragController.SetControlActive(false);
            editController.dragTarget.SetControlActive(false);

            Object.Destroy(jointVisualRect);

            Object.Destroy(pt.gameObject);
            Object.Destroy(pt1.gameObject);

        }

        public override void OnToolSelected()
        {
            Debug.Log("Drag Tool Selected");

            //editController.dragController.SetControlActive(true);
            editController.dragTarget.SetControlActive(true);

            // Get visual
            jointVisualRect = Object.Instantiate(Resources.Load<GameObject>("JointVisualRect"));

        }

        public override void OnToolUpdate()
        {
            //Debug.Log("Drawing Rect");
            ProcessInputs();

            DrawDragLine();

            
        }

        public override bool ShouldBlockOtherEvents()
        {
            return true;
        }

        //----------
        //Events
        //----------

        void DrawDragLine()
        {
            if (editController.dragTarget.m_TargetJoint != null)
            {
                var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPos.z = 0;

                Vector3 a = editController.dragTarget.m_TargetJoint.transform.TransformPoint(editController.dragTarget.m_TargetJoint.anchor);
                Vector3 b = worldPos;

                pt.gameObject.SetActive(true);
                pt1.gameObject.SetActive(true);
                jointVisualRect.SetActive(true);
                pt.position = a;
                pt1.position = b;

                Vector3 diff = b - a;

                jointVisualRect.transform.localScale = new Vector3(diff.magnitude, jointVisualRect.transform.localScale.y, jointVisualRect.transform.localScale.z);

                Vector3 pos = a + new Vector3(diff.x / 2, diff.y / 2, 0);
                jointVisualRect.transform.position = pos;

                float angle = Mathf.Atan2(diff.y, diff.x);
                jointVisualRect.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);

            }
            else
            {
                if (pt.gameObject.activeSelf)
                {
                    pt.gameObject.SetActive(false);
                    pt1.gameObject.SetActive(false);
                    jointVisualRect.SetActive(false);
                }
            }
        }

        void ProcessInputs()
        {

        }
    }
}