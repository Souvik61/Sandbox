using SandboxGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastTest : MonoBehaviour
{

    public GameObject spawnObject;

    public List<GameObject> spawnedObjects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPositionOverlap(Camera.main.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Object"));

            if (rB)//If a rigidbody is present
            {
                Debug.Log("Clicked on object: " + rB.name);
            }
            else//Nothing is clicked
            {
                Debug.Log("No object");
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            var gO = Instantiate(spawnObject);

            Vector3 pos = Random.insideUnitCircle * 2;

            spawnedObjects.Add(gO);

            gO.transform.position = pos;

        }
    }
}
