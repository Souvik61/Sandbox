using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{

    public class ObjectSpringJoint : ObjectJoint
    {

        SpringJoint2D joint;

        // The two joined objects
        public ObjectBase objectA;
        public ObjectBase objectB;

        public JointVisualSpring jointVisual;

        //Pivots in local space

        Vector3 pivotA;
        Vector3 pivotB;

        /// <summary>
        /// Init the joint 
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="pivotA">In local pos</param>
        /// <param name="pivotB">In local pos</param>
        public void Init(ObjectBase objA, ObjectBase objB, Vector3 pivotA, Vector3 pivotB)
        {
            base.Init();

            type = ObjectType.SPRINGJOINT;

            objectA = objA;
            objectB = objB;
            this.pivotA = pivotA;
            this.pivotB = pivotB;

            joint = objA.gameObject.AddComponent<SpringJoint2D>();
            joint.connectedBody = objB.GetComponent<Rigidbody2D>();

            joint.anchor = pivotA;
            joint.connectedAnchor = pivotB;

            // Get visual
            //jointVisualRect = Instantiate(Resources.Load<GameObject>("JointVisualRect"), transform);

            var res = Resources.Load<JointVisualSpring>("JointVisualSpring");
            jointVisual = Instantiate(res);

        }

        private void LateUpdate()
        {
            Vector3 a = objectA.transform.TransformPoint(pivotA);
            Vector3 b = objectB.transform.TransformPoint(pivotB);

            //Vector3 diff = b - a;
            //
            //jointVisualRect.transform.localScale = new Vector3(diff.magnitude, jointVisualRect.transform.localScale.y, jointVisualRect.transform.localScale.z);
            //
            //Vector3 pos = a + new Vector3(diff.x / 2, diff.y / 2, 0);
            //jointVisualRect.transform.position = pos;
            //
            //float angle = Mathf.Atan2(diff.y, diff.x);
            //jointVisualRect.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);

            jointVisual.pivotA.position = a;
            jointVisual.pivotB.position = b;

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