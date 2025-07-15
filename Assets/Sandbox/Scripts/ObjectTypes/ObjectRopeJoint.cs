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

        private bool isOutlineEnabled;

        private bool invisible;

        int _sortingLayer;

        public Transform pivotAVisual;
        public Transform pivotBVisual;

        public Transform pivotPrefab;

        /// <summary>
        /// Have a dummy rigidbody incase objectB is null
        /// </summary>
        public Rigidbody2D dummyRigidbody;

        /// <summary>
        /// Distance joint to keep rope joint stable
        /// </summary>
        private DistanceJoint2D _distanceJoint;

        /// <summary>
        /// Init the joint 
        /// </summary>
        /// <param name="objA"></param>
        /// <param name="objB"></param>
        /// <param name="pivotA">In local pos</param>
        /// <param name="pivotB">In local pos</param>
        public void Init(ObjectManager objManager, ObjectBase objA, ObjectBase objB, Vector3 pivotA, Vector3 pivotB)
        {
            base.Init(objManager);

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

            //If i have objectB
            if (objB)
            {

                var joint = RopeSegments[RopeSegments.Count - 1].gameObject.AddComponent<HingeJoint2D>();
                joint.connectedBody = objB.GetComponent<Rigidbody2D>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = pivotB;
                joint.anchor = new Vector2(1.13f, 0);
            }
            else //else
            {
                GameObject dummyRigidbody = new GameObject();
                dummyRigidbody.transform.position = pivotB;
                var rb = dummyRigidbody.AddComponent<Rigidbody2D>();
                rb.isKinematic = true;
                this.dummyRigidbody = rb;

                var joint = RopeSegments[RopeSegments.Count - 1].gameObject.AddComponent<HingeJoint2D>();
                joint.connectedBody = rb;
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = Vector2.zero;
                joint.anchor = new Vector2(1.13f, 0);

            }

            // setup distance joint for better stability
            _distanceJoint = objA.gameObject.AddComponent<DistanceJoint2D>();
            _distanceJoint.maxDistanceOnly = true;
            _distanceJoint.enableCollision = true;
            _distanceJoint.connectedBody = objB != null ? objB.GetComponent<Rigidbody2D>() : dummyRigidbody;
            _distanceJoint.autoConfigureConnectedAnchor = false;
            _distanceJoint.autoConfigureDistance = true;
            _distanceJoint.anchor = pivotA;
            _distanceJoint.connectedAnchor = objectB ? pivotB : Vector2.zero;


            pivotAVisual = Instantiate(pivotPrefab);
            pivotBVisual = Instantiate(pivotPrefab);

            SetInvisible(false);

            UpdateProperties();

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

            pivotAVisual.position = a;
            pivotBVisual.position = b;
        }

        private void OnDestroy()
        {
            if (pivotAVisual != null)
            {
                Destroy(pivotAVisual.gameObject);
            }

            if (pivotBVisual != null)
            {
                Destroy(pivotBVisual.gameObject);
            }

            Destroy(_distanceJoint);
            if (dummyRigidbody)
            {
                Destroy(dummyRigidbody.gameObject);
            }
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

        public override void EnableOutline(bool enable)
        {
            isOutlineEnabled = enable;
            if (isOutlineEnabled)
            {
                foreach (var item in RopeSegments)
                {
                    if (item.GetChild(0).TryGetComponent<SpriteRenderer>(out SpriteRenderer spRend))
                    {
                        spRend.material = new Material(GameManager.Instance.ConfigData.outlineMaterial);
                    }
                }
            }
            else
            {
                foreach (var item in RopeSegments)
                {
                    if (item.GetChild(0).TryGetComponent<SpriteRenderer>(out SpriteRenderer spRend))
                    {
                        spRend.material = new Material(GameManager.Instance.ConfigData.spritedefMaterial);
                    }
                }
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
            _properties["_objA"] = new PropertyItem { id = "_objA", name = "ObjectA", proptype = PropertyType.STRING, getter = () => objectA.name };
            _properties["_objB"] = new PropertyItem { id = "_objB", name = "ObjectB", proptype = PropertyType.STRING, getter = () => { return objectB != null ? objectB.name : ""; } };
            _properties["_invisible"] = new PropertyItem { id = "_invisible", name = "Invisible", proptype = PropertyType.BOOL, getter = () => { return invisible; }, setter = (val) => { SetInvisible((bool)val); } };
            _properties["_layer"] = new PropertyItem { id = "_layer", name = "Layer", proptype = PropertyType.TOGGLE, getter = () => _sortingLayer, setter = (val) => { SetLayer((int)val); } };
            //_properties["_type"] = new PropertyItem { id = "_type", name = "Type", proptype = PropertyType.STRING, value = GetType() };
            //_properties["_posX"] = new PropertyItem { id = "_posX", name = "Position X", proptype = PropertyType.FLOAT, value = transform.position.x };
            //_properties["_posY"] = new PropertyItem { id = "_posY", name = "Position Y", proptype = PropertyType.FLOAT, value = transform.position.y };
            //_properties["_rot"] = new PropertyItem { id = "_rot", name = "Rotation", proptype = PropertyType.FLOAT, value = GetZRotation() };
            //_properties["_col"] = new PropertyItem { id = "_col", name = "Color", proptype = PropertyType.COLOR, value = GetColor() };
            //_properties["_radius"] = new PropertyItem { id = "_radius", name = "Radius", proptype = PropertyType.FLOAT, value = radius };
        }

        public void SetInvisible(bool value)
        {
            SetVisible(!value);
        }

        void SetVisible(bool value)
        {
            invisible = !value;

            if (invisible)
            {
                foreach (var item in RopeSegments)
                {
                    if (item.GetChild(0).TryGetComponent(out SpriteRenderer spRend))
                    {
                        spRend.enabled = false;
                    }
                }
            }
            else
            {
                foreach (var item in RopeSegments)
                {
                    if (item.GetChild(0).TryGetComponent(out SpriteRenderer spRend))
                    {
                        spRend.enabled = true;
                    }
                }
            }

        }

        public override void SetLayer(int layer)
        {
            _sortingLayer = layer;

            var spRend1 = pivotAVisual.GetComponentInChildren<SpriteRenderer>();
            var spRend2 = pivotBVisual.GetComponentInChildren<SpriteRenderer>();
            spRend1.sortingLayerID = objectManager.EditController.GetSortingLayer(layer);
            spRend2.sortingLayerID = objectManager.EditController.GetSortingLayer(layer);

            foreach (var item in RopeSegments)
            {
                if (item.GetChild(0).TryGetComponent(out SpriteRenderer spRend))
                {
                    spRend.sortingLayerID = objectManager.EditController.GetSortingLayer(layer);
                }
            }
        }

    }
}