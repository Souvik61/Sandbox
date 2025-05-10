using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SandboxGame
{
    public class PNL_ObjectBrowser : MonoBehaviour, IObjectObserver
    {
        public GameObject ListingItemPrefab;
        public GameObject ListItemContainer;

        public ObjectManager oManager;
        public EditController editController;

        public void Init(ObjectManager objectManager,EditController editController)
        {
            oManager = objectManager;
            oManager.AddObserver(this);
            this.editController = editController;

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
            //Find a better way
            OnStateUpdated();
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

            //find required height for list
            {
                float h = ListingItemPrefab.GetComponent<RectTransform>().rect.height * oManager.objectList.Count + 30f;

                RectTransform rectTransform = ListItemContainer.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, h);
            }

            // add list items
            foreach (var item in oManager.objectList)
            {
                GameObject listItem = Instantiate(ListingItemPrefab, ListItemContainer.transform);
                listItem.SetActive(true);
                listItem.transform.FindDeep("Txt_Name").GetComponent<TMP_Text>().text = item.gameObject.name;
                listItem.GetComponent<UIObjectBrowserListing>().TargetObject = item;
                //EventTrigger trigger = listItem.GetComponent<EventTrigger>();
                //EventTrigger.Entry entry = new EventTrigger.Entry
                //{
                //    eventID = EventTriggerType.PointerClick
                //};
                //entry.callback.AddListener((data) => { OnAListingClicked((PointerEventData)data); });
                //trigger.triggers.Add(entry);

                listItem.GetComponent<Button>().onClick.AddListener(() => OnAListingClicked(listItem));
                listItem.transform.FindDeep("Btn_Delete").GetComponent<Button>().onClick.AddListener(() => OnListingDeleteClicked(listItem));

            }
        }

        void OnAListingClicked(GameObject item)
        {
            //GameObject item = data.pointerClick;
            if (item.TryGetComponent<UIObjectBrowserListing>(out UIObjectBrowserListing comp))
            {
                //Debug.Log("comp: " + comp.TargetObject.name);
                editController.SelectObject(comp.TargetObject);
            }                   
        }

        void OnListingDeleteClicked(GameObject item)
        {
            //GameObject item = data.pointerClick;
            if (item.TryGetComponent<UIObjectBrowserListing>(out UIObjectBrowserListing comp))
            {
                Debug.Log("Delete: " + comp.TargetObject.name);
                editController.DeleteObject(comp.TargetObject);
            }
        }


    }
}
