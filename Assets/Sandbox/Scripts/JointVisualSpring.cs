using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class JointVisualSpring : JointVisual
    {

        List<Transform> segmentList = new List<Transform>();

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void LateUpdate()
        {
            line.transform.position = pivotA.transform.position;
            Vector3 dir = pivotB.position - pivotA.position;
            dir.Normalize();
            float dist = Vector3.Distance(pivotA.position, pivotB.position);

            int segCount = Mathf.CeilToInt(dist);

            //If i not got enough segments 
            if (segmentList.Count != segCount)
            {

                for (int i = 1; i < segmentList.Count; i++)
                {
                    Destroy(segmentList[i].gameObject);
                }
                segmentList.Clear();
                //Create new segments

                segmentList.Add(line);

                for (int i = 0; i < segCount-1; i++)
                {
                    var seg = Instantiate(line, transform);
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
    }
}
