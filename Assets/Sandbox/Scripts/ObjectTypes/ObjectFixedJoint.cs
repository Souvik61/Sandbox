using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SandboxGame
{

    public class ObjectFixedJoint : ObjectJoint
    {
        
        FixedJoint2D joint;

        // The two joined objects
        public ObjectBase objectA;
        public ObjectBase objectB;

        public JointVisual jointVisual;

        public Vector3 pivotA;
        public Vector3 pivotB;


        private bool isOutlineEnabled;

        [SerializeField] private SpriteRenderer outlineSprite;

        /// <summary>
        /// Init the joint 
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="pivotA">In local pos</param>
        /// <param name="pivotB">In local pos</param>
        public void Init(ObjectBase objA, ObjectBase objB,Vector3 pivotA,Vector3 pivotB)
        {
            base.Init();

            type = ObjectType.FIXEDJOINT;

            objectA = objA;
            objectB = objB;
            this.pivotA = pivotA;
            this.pivotB = pivotB;

            joint = objA.gameObject.AddComponent<FixedJoint2D>();
            //joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = objB.GetComponent<Rigidbody2D>();

            //joint.anchor = objectA.transform.InverseTransformPoint(pivotA);
            //joint.connectedAnchor = objectB.transform.InverseTransformPoint(pivotB);

            // Get visual
            //jointVisualRect = Instantiate(Resources.Load<GameObject>("JointVisualRect"), transform);

            var res = Resources.Load<JointVisual>("JointVisual");
            jointVisual = Instantiate(res);

            UpdateProperties();

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

        public override void EnableOutline(bool enable)
        {
            isOutlineEnabled = enable;
            jointVisual.EnableOutline(isOutlineEnabled);
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
            //_properties["_posX"] = new PropertyItem { id = "_posX", name = "Position X", proptype = PropertyType.FLOAT, value = transform.position.x };
            //_properties["_posY"] = new PropertyItem { id = "_posY", name = "Position Y", proptype = PropertyType.FLOAT, value = transform.position.y };
            //_properties["_rot"] = new PropertyItem { id = "_rot", name = "Rotation", proptype = PropertyType.FLOAT, value = GetZRotation() };
            //_properties["_col"] = new PropertyItem { id = "_col", name = "Color", proptype = PropertyType.COLOR, value = GetColor() };
            //_properties["_radius"] = new PropertyItem { id = "_radius", name = "Radius", proptype = PropertyType.FLOAT, value = radius };
        }

    }
}