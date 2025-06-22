using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Progress;

namespace SandboxGame
{
    public class JointVisualSpring : JointVisual
    {

        protected List<Transform> segmentList = new List<Transform>();

        int _sortingLayer;

        private void LateUpdate()
        {
            line.transform.position = pivotA.transform.position;
            Vector3 dir = pivotB.position - pivotA.position;
            dir.Normalize();
            float dist = Vector3.Distance(pivotA.position, pivotB.position);

            int segCount = Mathf.CeilToInt(dist);

            SpriteRenderer spRend;

            //If i not got enough segments 
            if (segmentList.Count != segCount)
            {

                for (int i = 1; i < segmentList.Count; i++)
                {
                    Destroy(segmentList[i].gameObject);
                }
                segmentList.Clear();

                // set segment sorting layer
                if (line.TryGetComponent(out spRend))
                {
                    spRend.sortingLayerID = EditController.Instance.GetSortingLayer(_sortingLayer);
                }

                //Create new segments
                segmentList.Add(line);

                for (int i = 0; i < segCount - 1; i++)
                {
                    var seg = Instantiate(line, transform);

                    // set segment sorting layer
                    if (seg.TryGetComponent(out spRend))
                    {
                        spRend.sortingLayerID = EditController.Instance.GetSortingLayer(_sortingLayer);
                    }

                    segmentList.Add(seg);
                }
            }

            float segLength = dist / segCount;

            //line.localScale = new Vector3(dist, 1, 1);

            Vector3 v = pivotB.position - pivotA.position;
            float ang = Mathf.Atan2(v.y, v.x);

            //line.transform.eulerAngles = new Vector3(0, 0, ang * Mathf.Rad2Deg);

            for (int i = 0; i < segCount; i++)
            {
                var seg = segmentList[i];
                seg.position = pivotA.transform.position + (dir * segLength * i);
                seg.transform.eulerAngles = new Vector3(0, 0, ang * Mathf.Rad2Deg);
                seg.localScale = new Vector3(segLength, 1, 1);

            }

        }

        public override void EnableOutline(bool enable)
        {
            isOutlineEnabled = enable;

            if (isOutlineEnabled)
            {
                foreach (var item in segmentList)
                {
                    if (item.TryGetComponent<SpriteRenderer>(out SpriteRenderer spRend))
                    {
                        spRend.material = new Material(GameManager.Instance.ConfigData.outlineMaterial);
                    }
                }
            }
            else
            {
                foreach (var item in segmentList)
                {
                    if (item.TryGetComponent<SpriteRenderer>(out SpriteRenderer spRend))
                    {
                        spRend.material = new Material(GameManager.Instance.ConfigData.spritedefMaterial);
                    }
                }
            }
        }

        public override void SetVisible(bool value)
        {
            invisible = !value;
            var spRend1 = pivotA.GetComponentInChildren<SpriteRenderer>();
            var spRend2 = pivotB.GetComponentInChildren<SpriteRenderer>();

            if (invisible)
            {
                foreach (var item in segmentList)
                {
                    if (item.TryGetComponent(out SpriteRenderer spRend))
                    {
                        spRend.enabled = false;
                    }
                }
            }
            else
            {
                foreach (var item in segmentList)
                {
                    if (item.TryGetComponent(out SpriteRenderer spRend))
                    {
                        spRend.enabled = true;
                    }
                }
            }

        }

        public override void SetSortingLayerId(int layer)
        {
            _sortingLayer = layer;

            var spRend1 = pivotA.GetComponentInChildren<SpriteRenderer>();
            var spRend2 = pivotB.GetComponentInChildren<SpriteRenderer>();

            spRend1.sortingLayerID = EditController.Instance.GetSortingLayer(layer);
            spRend2.sortingLayerID = EditController.Instance.GetSortingLayer(layer);

            foreach (var item in segmentList)
            {
                if (item.TryGetComponent(out SpriteRenderer spRend))
                {
                    spRend.sortingLayerID = EditController.Instance.GetSortingLayer(layer);
                }
            }

        }
    }
}
