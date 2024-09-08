using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragController : MonoBehaviour
{
    public int layer;
    public Rigidbody2D selectedRb;


    //Private

    bool isMouseHeldDown;
    Vector3 previousMousePos;
    Vector3 deltaMousePos;
    /// <summary>
    /// Is my influence active?
    /// </summary>
    bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        Vector3 currentMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            isMouseHeldDown = true;

            RaycastHit2D hit = Physics2D.Raycast(currentMousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.layer == layer)
            {
                //Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
                selectedRb = hit.collider.gameObject.GetComponent<Rigidbody2D>() ? hit.collider.gameObject.GetComponent<Rigidbody2D>() : hit.collider.gameObject.GetComponentInParent<Rigidbody2D>();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseHeldDown = false;
            selectedRb = null;
        }

        //Debug.Log("Delta pos: x:" + deltaMousePos.x + " y:" + deltaMousePos.y);

        deltaMousePos = currentMousePos - previousMousePos;
        previousMousePos = currentMousePos;

    }

    private void FixedUpdate()
    {
        if (selectedRb && isMouseHeldDown && isActive)
        {
            selectedRb.MovePosition(previousMousePos);
        }
    }

    public void SetControlActive(bool active)
    {
        isActive = active;
    }

    public bool GetControlActive()
    {
        return isActive;
    }
}
