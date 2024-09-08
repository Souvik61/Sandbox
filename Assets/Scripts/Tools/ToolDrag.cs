using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolDrag : ToolBase
{
    EditController editController;
    TouchManager tManager;
    ObjectManager oManager;

    //Private

    private Vector3 _dragStartPos;
    private Vector3 _dragEndPos;

    public ToolDrag(EditController editC)
    {
        editController = editC;
        this.tManager = editC.tManager;
        this.oManager = editC.oManager;

        //Subscribe to event functions
        tManager.OnDragStarted += OnStartedDraging;
        tManager.OnDragEnded += OnEndDraging;
    }

    ~ToolDrag()
    {
        
    }

    public override void OnToolDeselected()
    {
        Debug.Log("Drag Tool Deselected");
        editController.dragController.SetControlActive(false);

        tManager.OnDragStarted -= OnStartedDraging;
        tManager.OnDragEnded -= OnEndDraging;
    }

    public override void OnToolSelected()
    {
        Debug.Log("Drag Tool Selected");

        editController.dragController.SetControlActive(true);

    }

    public override void OnToolUpdate()
    {
        //Debug.Log("Drawing Rect");
        ProcessInputs();

    }

    //----------
    //Events
    //----------

    void OnStartedDraging()
    {
        

    }

    void OnEndDraging()
    {

        
    }

    //------------------
    //Helper
    //------------------

    void ProcessInputs()
    {

    }
}
