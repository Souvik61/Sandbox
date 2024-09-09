using System.Collections.Generic;
using UnityEngine;


namespace SandboxGame
{

    /// <summary>
    /// Class responsible for linking an object and a inspector view
    /// </summary>
    public class ObjectLinker : MonoBehaviour
    {

        public ObjectBase targetObject;
        public PNL_Inspector viewInspector;

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Link the object and inspector
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="inspector"></param>
        public void Link(ObjectBase obj, PNL_Inspector inspector)
        {
            targetObject = obj;
            viewInspector = inspector;

            inspector.LinkView(obj);
        }
    }
}