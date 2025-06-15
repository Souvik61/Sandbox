using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SandboxGame
{

    public class ObjectRopeJoint : ObjectJoint
    {
        // The two joined objects
        public ObjectBase objectA;
        public ObjectBase objectB;

        public GameObject jointVisualRect;

        public List<Transform> RopeSegments;


        //Pivots in local space

        public Vector3 pivotA;
        public Vector3 pivotB;

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

            type = ObjectType.ROPEJOINT;
            RopeSegments = new List<Transform>();

            objectA = objA;
            objectB = objB;
            this.pivotA = pivotA;
            this.pivotB = pivotB;


            if (transform.childCount > 0)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    RopeSegments.Add(transform.GetChild(i));
                }
            }

            if (objA)
            {
                RopeSegments[0].GetComponent<HingeJoint2D>().connectedBody = objA.GetComponent<Rigidbody2D>();
                RopeSegments[0].GetComponent<HingeJoint2D>().connectedAnchor = pivotA;
            }

            if (objB)
            {

                var joint = RopeSegments[RopeSegments.Count - 1].gameObject.AddComponent<HingeJoint2D>();
                joint.connectedBody = objB.GetComponent<Rigidbody2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = pivotB;
                joint.anchor = new Vector2(1.13f, 0);

                //RopeSegments[RopeSegments.Count - 1].GetComponent<Rigidbody2D>();
            }

            // Get visual
            //jointVisualRect = Instantiate(Resources.Load<GameObject>("JointVisualRect"), transform);

            UpdateProperties();

        }

        private void LateUpdate()
        {
            //Vector3 a = objectA.transform.position;
            //Vector3 b = objectB.transform.position;
            //
            //Vector3 diff = b - a;
            //
            //jointVisualRect.transform.localScale = new Vector3(diff.magnitude, jointVisualRect.transform.localScale.y, jointVisualRect.transform.localScale.z);
            //
            //Vector3 pos = a + new Vector3(diff.x / 2, diff.y / 2, 0);
            //jointVisualRect.transform.position = pos;
            //
            //float angle = Mathf.Atan2(diff.y, diff.x);
            //jointVisualRect.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);

        }

        /// <summary>
        /// Rope will ignore colision with this specific object
        /// </summary>
        /// <param name="obj"></param>
        public void IgnoreCollision(ObjectBase obj)
        {
            if (obj == null)
                return;

            for (int i = 0; i < RopeSegments.Count; i++)
            {
                Physics2D.IgnoreCollision(obj.GetComponentInChildren<Collider2D>(), RopeSegments[i].GetComponent<Collider2D>());
            }
        
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
            //_properties["_type"] = new PropertyItem { id = "_type", name = "Type", proptype = PropertyType.STRING, value = GetType() };
            //_properties["_posX"] = new PropertyItem { id = "_posX", name = "Position X", proptype = PropertyType.FLOAT, value = transform.position.x };
            //_properties["_posY"] = new PropertyItem { id = "_posY", name = "Position Y", proptype = PropertyType.FLOAT, value = transform.position.y };
            //_properties["_rot"] = new PropertyItem { id = "_rot", name = "Rotation", proptype = PropertyType.FLOAT, value = GetZRotation() };
            //_properties["_col"] = new PropertyItem { id = "_col", name = "Color", proptype = PropertyType.COLOR, value = GetColor() };
            //_properties["_radius"] = new PropertyItem { id = "_radius", name = "Radius", proptype = PropertyType.FLOAT, value = radius };
        }

    }
}