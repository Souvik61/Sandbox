using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{

    public class ObjectFixedJoint : ObjectBase
    {
        
        FixedJoint2D joint;

        // The two joined objects
        public ObjectBase objectA;
        public ObjectBase objectB;

        public GameObject jointVisualRect;

        public void Init(ObjectBase objA, ObjectBase objB)
        {
            base.Init();

            type = ObjectType.FIXEDJOINT;

            objectA = objA;
            objectB = objB;

            joint = objA.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = objB.GetComponent<Rigidbody2D>();

            // Get visual
            jointVisualRect = Instantiate(Resources.Load<GameObject>("JointVisualRect"), transform);

        }

        private void LateUpdate()
        {
            Vector3 a = objectA.transform.position;
            Vector3 b = objectB.transform.position;

            Vector3 diff = b - a;

            jointVisualRect.transform.localScale = new Vector3(diff.magnitude, jointVisualRect.transform.localScale.y, jointVisualRect.transform.localScale.z);

            Vector3 pos = a + new Vector3(diff.x / 2, diff.y / 2, 0);
            jointVisualRect.transform.position = pos;

            float angle = Mathf.Atan2(diff.y, diff.x);
            jointVisualRect.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);

        }

    }
}