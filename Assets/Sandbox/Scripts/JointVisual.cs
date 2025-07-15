using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace SandboxGame
{
    public class JointVisual : MonoBehaviour
    {

        public Transform pivotA;
        public Transform pivotB;
        public Transform line;

        protected bool isOutlineEnabled;

        protected bool invisible;

        int _sortingLayer;

        protected EditController editController;

        public void Init(EditController editController)
        {
            this.editController = editController;
        }

        private void Awake()
        {
            SetVisible(true);
        }

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

        public virtual void EnableOutline(bool enable)
        {
            isOutlineEnabled = enable;

            if (isOutlineEnabled)
            {
                var spRend = line.GetComponent<SpriteRenderer>();
                spRend.material = new Material(GameManager.Instance.ConfigData.outlineMaterial);
            }
            else
            {
                var spRend = line.GetComponent<SpriteRenderer>();
                spRend.material = new Material(GameManager.Instance.ConfigData.spritedefMaterial);
            }
        }

        public virtual void SetVisible(bool value)
        {
            invisible = !value;
            var spRend = line.GetComponent<SpriteRenderer>();
            var spRend1 = pivotA.GetComponentInChildren<SpriteRenderer>();
            var spRend2 = pivotB.GetComponentInChildren<SpriteRenderer>();

            spRend.enabled = value;
            spRend1.enabled = value;
            spRend2.enabled = value;

        }

        public virtual void SetSortingLayerId(int layer)
        {
            _sortingLayer = layer;

            var spRend = line.GetComponent<SpriteRenderer>();
            var spRend1 = pivotA.GetComponentInChildren<SpriteRenderer>();
            var spRend2 = pivotB.GetComponentInChildren<SpriteRenderer>();

            spRend.sortingLayerID = editController.GetSortingLayer(layer);
            spRend1.sortingLayerID = editController.GetSortingLayer(layer);
            spRend2.sortingLayerID = editController.GetSortingLayer(layer);

        }
    }
}
