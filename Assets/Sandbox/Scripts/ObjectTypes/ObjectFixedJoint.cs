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

        private bool invisible;

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

            var res = Resources.Load<JointVisual>("JointVisual");
            jointVisual = Instantiate(res);

            SetInvisible(false);

            UpdateProperties();

        }

        private void OnDestroy()
        {
            Destroy(jointVisual.gameObject);
        }

        private void LateUpdate()
        {
            Vector3 a = objectA.transform.TransformPoint(pivotA);
            Vector3 b = objectB.transform.TransformPoint(pivotB);

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
            _properties["_invisible"] = new PropertyItem { id = "_invisible", name = "Invisible", proptype = PropertyType.BOOL, getter = () => { return invisible; } ,setter=(val)=> { SetInvisible((bool)val); } };
        }

        public void SetInvisible(bool value)
        {
            invisible = value;
            jointVisual.SetVisible(!value);

        }

    }
}