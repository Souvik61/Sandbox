using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace SandboxGame
{
    public class Rope2DCreator : MonoBehaviour
    {
        [ReadOnly]
        public int segmentsCount = 2;

        public Transform pointA;
        public Transform pointB;

        public GameObject segmentPrefab;

        [HideInInspector] public Transform[] segments;

        public float segmentLength = 0.1f;

        Vector2 GetSegmentPosition(int segmentIndex)
        {
            Vector2 posA = pointA.position;
            Vector2 posB = pointB.position;

            float fraction = 1f / (float)segmentsCount;
            return Vector2.Lerp(posA, posB, fraction * segmentIndex);
        }

        [Button]
        void GenerateRope()
        {
            float dist = Vector3.Distance(pointA.position, pointB.position);
            segmentsCount = (int)(dist / segmentLength);

            segments = new Transform[segmentsCount];

            for (int i = 0; i < segmentsCount; i++)
            {
                var currJoint = Instantiate(segmentPrefab, GetSegmentPosition(i), Quaternion.identity, transform).GetComponent<HingeJoint2D>();

                SetSegmentLength(currJoint.gameObject, segmentLength);
                segments[i] = currJoint.transform;

                if (i > 0)
                {
                    int prevIndex = i - 1;
                    currJoint.connectedBody = segments[prevIndex].GetComponent<Rigidbody2D>();
                }
            }
        
        }

        [Button]
        void DeleteSegments()
        {
            if (transform.childCount > 0)
            {
                for (int i = transform.childCount-1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
            segments = null;
        }

        private void OnValidate()
        {
            if (pointA == null || pointB == null)
                return;

            float dist = Vector3.Distance(pointA.position, pointB.position);
            segmentsCount = (int)(dist / segmentLength);
        }

        private void OnDrawGizmos()
        {
            if (pointA == null || pointB == null) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < segmentsCount; i++)
            {
                Vector2 posAtIndex = GetSegmentPosition(i);
                Gizmos.DrawSphere(posAtIndex, 0.1f);
            }
        }

        private void SetSegmentLength(GameObject segment, float length)
        {
            var box = segment.GetComponent<BoxCollider2D>();

            box.size = new Vector2(length, box.size.y);
            box.offset = new Vector2(length / 2, 0);

            GameObject graphic = segment.transform.GetChild(0).gameObject;

            graphic.transform.localPosition = new Vector3(length / 2, 0, 0);
            graphic.transform.localScale = new Vector3(0.17f * length, graphic.transform.localScale.y, 1);

        }
    }

}

