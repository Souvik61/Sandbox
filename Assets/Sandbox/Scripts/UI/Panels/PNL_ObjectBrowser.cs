using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace SandboxGame
{
    public class PNL_ObjectBrowser : MonoBehaviour, IObjectObserver
    {
        public GameObject ListingItemPrefab;
        public GameObject ListItemContainer;

        public ObjectManager oManager;

        public void Init(ObjectManager objectManager)
        {
            oManager = objectManager;
            oManager.AddObserver(this);

            // manual update
            OnStateUpdated();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnObjectAdded(ObjectBase obj)
        {
            throw new System.NotImplementedException();
        }

        public void OnObjectRemoved(ObjectBase obj)
        {
            throw new System.NotImplementedException();
        }

        public void OnObjectUpdated(ObjectBase obj)
        {
            throw new System.NotImplementedException();
        }

        public void OnStateUpdated()
        {
            // clear container
            foreach (Transform child in ListItemContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // add list items
            foreach (var item in oManager.objectList)
            {
                GameObject listItem = Instantiate(ListingItemPrefab, ListItemContainer.transform);
                listItem.SetActive(true);
                listItem.transform.FindDeep("Txt_Name").GetComponent<TMP_Text>().text = item.gameObject.name;
            }
        }
    }
}
