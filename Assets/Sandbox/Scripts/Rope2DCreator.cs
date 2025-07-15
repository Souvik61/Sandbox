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

        public Transform RopeParent;

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
                var currJoint = Instantiate(segmentPrefab, GetSegmentPosition(i), Quaternion.identity, RopeParent).GetComponent<HingeJoint2D>();
                currJoint.gameObject.SetActive(true);
                SetSegmentLength(currJoint.gameObject, segmentLength);
                segments[i] = currJoint.transform;

                //Set joint rotation accordingly
                Vector3 a = pointA.position;
                Vector3 b = pointB.position;
                Vector3 diff = b - a;

                float angle = Mathf.Atan2(diff.y, diff.x);
                currJoint.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);


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
            if (RopeParent.childCount > 0)
            {
                for (int i = RopeParent.childCount-1; i >= 0; i--)
                {
                    DestroyImmediate(RopeParent.GetChild(i).gameObject);
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

        /// <summary>
        /// Manually create a rope with given params
        /// </summary>
        /// <param name="ropeParent"></param>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <param name="segmentLength"></param>
        public void CreateRope(Transform ropeParent,Transform objectA,Transform objectB,float segmentLength)
        {
            Vector2 GetSegmentPosition(Vector2 pointA,Vector2 pointB, int segmentIndex,int segmentsCount)
            {
                Vector2 posA = pointA;
                Vector2 posB = pointB;

                float fraction = 1f / (float)segmentsCount;
                return Vector2.Lerp(posA, posB, fraction * segmentIndex);
            }


            float dist = Vector3.Distance(objectA.position, objectB.position);
            int segmentsCount = (int)(dist / segmentLength);

            Transform[] segments = new Transform[segmentsCount];

            for (int i = 0; i < segmentsCount; i++)
            {
                var currJoint = Instantiate(segmentPrefab, GetSegmentPosition(objectA.position, objectB.position,i,segmentsCount), Quaternion.identity, ropeParent).GetComponent<HingeJoint2D>();
                currJoint.gameObject.SetActive(true);
                SetSegmentLength(currJoint.gameObject, segmentLength);
                segments[i] = currJoint.transform;

                //Set joint rotation accordingly
                Vector3 a = objectA.position;
                Vector3 b = objectB.position;
                Vector3 diff = b - a;

                float angle = Mathf.Atan2(diff.y, diff.x);
                currJoint.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);

                if (i > 0)
                {
                    currJoint.connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                }
            }

        }

        /// <summary>
        /// Manually create a rope with given params
        /// </summary>
        /// <param name="ropeParent"></param>
        /// <param name="objectA"></param>
        /// <param name="objectB"></param>
        /// <param name="pivotA"></param>
        /// <param name="pivotB"></param>
        /// <param name="segmentLength"></param>
        public void CreateRope(Transform ropeParent, Transform objectA, Transform objectB,Vector3 pivotA,Vector3 pivotB, float segmentLength)
        {
            Vector2 GetSegmentPosition(Vector2 pointA, Vector2 pointB, int segmentIndex, int segmentsCount)
            {
                Vector2 posA = pointA;
                Vector2 posB = pointB;

                float fraction = 1f / (float)segmentsCount;
                return Vector2.Lerp(posA, posB, fraction * segmentIndex);
            }

            Vector3 objBPos = objectB != null ? objectB.TransformPoint(pivotB) : pivotB;

            float dist = Vector3.Distance(objectA.TransformPoint(pivotA), objBPos);
            int segmentsCount = (int)(dist / segmentLength);

            Transform[] segments = new Transform[segmentsCount];

            for (int i = 0; i < segmentsCount; i++)
            {
                var currJoint = Instantiate(segmentPrefab, GetSegmentPosition(objectA.TransformPoint(pivotA), objBPos, i, segmentsCount), Quaternion.identity, ropeParent).GetComponent<HingeJoint2D>();
                currJoint.gameObject.SetActive(true);
                SetSegmentLength(currJoint.gameObject, segmentLength);
                segments[i] = currJoint.transform;

                //Set joint rotation accordingly
                Vector3 a = objectA.TransformPoint(pivotA);
                Vector3 b = objBPos;
                Vector3 diff = b - a;

                float angle = Mathf.Atan2(diff.y, diff.x);
                currJoint.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);

                if (i > 0)
                {
                    currJoint.connectedBody = segments[i - 1].GetComponent<Rigidbody2D>();
                }
            }

        }
    }

}

