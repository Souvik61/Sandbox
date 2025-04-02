using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawModeSelector : MonoBehaviour
{
    public ShapeDrawType currentDrawMode;

    public TouchManager tManager;

    // Start is called before the first frame update
    void Start()
    {
        currentDrawMode = ShapeDrawType.SQUARE;
    }

    // Update is called once per frame
    void Update()
    {
        currentDrawMode = TouchManager.Instance.currentDrawType;

        if (currentDrawMode != ShapeDrawType.CIRCLE)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                currentDrawMode = ShapeDrawType.SQUARE;

                TouchManager.Instance.currentDrawType = currentDrawMode;
            }
            else
            {
                currentDrawMode = ShapeDrawType.RECT;

                TouchManager.Instance.currentDrawType = currentDrawMode;
            }
        }

        //Assign to draw type
        //tManager.currentDrawType = currentDrawMode;
    }
}
