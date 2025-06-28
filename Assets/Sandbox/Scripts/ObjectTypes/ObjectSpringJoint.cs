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

        public Vector3 pivotA;
        public Vector3 pivotB;

        private bool isOutlineEnabled;

        private bool invisible;

        int _sortingLayer;

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
            if (objectB)
            {
                joint.connectedBody = objB.GetComponent<Rigidbody2D>();
            }

            joint.anchor = pivotA;
            joint.connectedAnchor = pivotB;

            // Get visual
            //jointVisualRect = Instantiate(Resources.Load<GameObject>("JointVisualRect"), transform);

            var res = Resources.Load<JointVisualSpring>("JointVisualSpring");
            jointVisual = Instantiate(res);

            SetInvisible(false);

            SetLayer(0);

            UpdateProperties();

        }

        private void OnDestroy()
        {
            if (jointVisual == null)
            {
                Debug.Log("Something went wrong...");
            }
            else
            {
                Destroy(jointVisual.gameObject);
                Destroy(joint);
            }
        }

        private void LateUpdate()
        {
            Vector3 a = pivotA;
            Vector3 b = pivotB;

            if (objectA)
            {
                a = objectA.transform.TransformPoint(pivotA);
            }
            if (objectB)
            {
                b = objectB.transform.TransformPoint(pivotB);
            }

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
            _properties["_objA"] = new PropertyItem { id = "_objA", name = "ObjectA", proptype = PropertyType.STRING, getter = () => objectA.name };
            _properties["_objB"] = new PropertyItem { id = "_objB", name = "ObjectB", proptype = PropertyType.STRING, getter = () => { return objectB != null ? objectB.name : ""; } };
            _properties["_invisible"] = new PropertyItem { id = "_invisible", name = "Invisible", proptype = PropertyType.BOOL, getter = () => { return invisible; }, setter = (val) => { SetInvisible((bool)val); } };
            _properties["_layer"] = new PropertyItem { id = "_layer", name = "Layer", proptype = PropertyType.TOGGLE, getter = () => _sortingLayer, setter = (val) => { SetLayer((int)val); } };

        }

        public void SetInvisible(bool value)
        {
            invisible = value;
            jointVisual.SetVisible(!value);

        }

        public override void SetLayer(int layer)
        {
            _sortingLayer = layer;
            jointVisual.SetSortingLayerId(layer);
        }
    }
}