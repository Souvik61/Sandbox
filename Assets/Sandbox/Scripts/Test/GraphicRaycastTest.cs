using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SandboxGame
{
    public class GraphicRaycastTest : MonoBehaviour
    {
        public GraphicRaycaster raycaster;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    PointerEventData pointerData = new PointerEventData(EventSystem.current);
                    pointerData.position = Input.mousePosition;

                    List<RaycastResult> results = new List<RaycastResult>();
                    raycaster.Raycast(pointerData, results);

                    if (results.Count > 0)
                    {
                        foreach (var item in results)
                        {
                            Debug.Log("Pointer over gameobject: " + item.gameObject.name);
                        }

                    }
                    else
                    {
                        Debug.Log("Pointer not over gameobject.");
                    }
                }
            }
        }
    }
}
