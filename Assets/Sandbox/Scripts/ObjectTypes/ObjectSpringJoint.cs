using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{

    public class ObjectSpringJoint : ObjectBase
    {

        SpringJoint2D joint;

        // The two joined objects
        public ObjectBase objectA;
        public ObjectBase objectB;

        public GameObject jointVisualRect;

        public void Init(ObjectBase objA, ObjectBase objB)
        {
            base.Init();

            type = ObjectType.SPRINGJOINT;

            objectA = objA;
            objectB = objB;

            joint = objA.gameObject.AddComponent<SpringJoint2D>();
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

        /// <summary>
        /// Get list of all properties of this object
        /// </summary>
        /// <returns></returns>
        public override List<PropertyItem> GetAllProperties()
        {
            return _properties.Values.ToList();
        }

        /// <summary>
        /// Keep this incase we need to manually update in future
        /// </summary>
        public override void UpdateProperties()
        {
            _properties["_type"] = new PropertyItem { id = "_type", name = "Type", proptype = PropertyType.STRING, getter = () => type.ToString() };
            _properties["_objA"] = new PropertyItem { id = "_objA", name = "ObjectA", proptype = PropertyType.STRING, getter = () => objectA.name };
            _properties["_objB"] = new PropertyItem { id = "_objB", name = "ObjectB", proptype = PropertyType.STRING, getter = () => objectB.name };
        }

    }
}