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

        public void Init(ObjectBase objA, ObjectBase objB)
        {
            base.Init();

            objectA = objA;
            objectB = objB;

            joint = objA.gameObject.AddComponent<FixedJoint2D>();
            joint.connectedBody = objB.GetComponent<Rigidbody2D>();


        }
    }
}