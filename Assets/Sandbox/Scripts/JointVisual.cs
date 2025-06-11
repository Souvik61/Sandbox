using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class JointVisual : MonoBehaviour
    {

        public Transform pivotA;
        public Transform pivotB;
        public Transform line;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void LateUpdate()
        {

            line.transform.position = pivotA.transform.position;

            float dist = Vector3.Distance(pivotA.position, pivotB.position);
            line.localScale = new Vector3(dist, 1, 1);

            Vector3 v = pivotB.position - pivotA.position;
            float ang = Mathf.Atan2(v.y, v.x);

            line.transform.eulerAngles = new Vector3(0, 0, ang * Mathf.Rad2Deg);

            
        }
    }
}
