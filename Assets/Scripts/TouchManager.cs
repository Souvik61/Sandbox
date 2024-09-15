using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// Responsible for getting inputs and gestures
/// </summary>
public class TouchManager : Singleton<TouchManager>, IManager
{

    //Public 

    #region Drawing

    public ShapeDrawType currentDrawType = ShapeDrawType.NONE;

    /// <summary>
    /// A dummy variable to be used with Object manager
    /// </summary>
    public ShapeDrawType prevDrawType = ShapeDrawType.NONE;

    public ShapeDrawBase currentShapeDrawObj;

    #endregion

    /// <summary>
    /// Greater than this value will be considered touch drag
    /// </summary>
    public float dragMagnitude;

    /// <summary>
    /// For square gizmo
    /// </summary>
    public GameObject squareGizmo;

    /// <summary>
    /// For circle gizmo
    /// </summary>
    public GameObject circleGizmo;
    public GameObject marker;

    public bool IsDrawing;

    public bool IsDraging;

    public bool IsSpaceBarHeld { get => Input.GetKey(KeyCode.Space); }


    public Vector3 startMousePosition;

    /// <summary>
    /// Start screen coord for the current drag input
    /// </summary>
    public Vector3 startMousePositionScreen;
    public Vector3 mousePositionScreen;

    /// <summary>
    /// If touch sequence started over a shape
    /// </summary>
    public bool? isTouchStartedOnShape;

    #region Events

    public Action OnDragStarted;
    public Action OnDragEnded;

    #endregion

    public IEnumerator Init()
    {
        yield return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        squareGizmo.SetActive(false);
        circleGizmo.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // verify pointer is not on top of GUI; if it is, return
        if (EventSystem.current.IsPointerOverGameObject()) return;

        //Process inputs
        ProcessInputs();
        //
        ////Draw ingame gizmos
        DrawGizmos();
    }

    #region Gizmo functions

    private void DrawGizmos()
    {
        if (IsDrawing && Input.GetMouseButtonUp(0)) // done drawing line
        {
            IsDrawing = false;
            OnEndDrawing();
        }

        if (Input.GetMouseButton(0)) // Mouse is being held down
        {
            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {
                startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                startMousePosition.z = 0;

                //squareGizmo.transform.position = new Vector3(startMousePosition.x, startMousePosition.y, 0);
                //marker.transform.position = squareGizmo.transform.position;

                OnStartDrawing();
            }
            else
            {
                // mouse held down or touch held down
                Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePos.z = 0;

                float _endXDistance = (_mousePos.x - squareGizmo.transform.position.x);
                float _endYDistance = (_mousePos.y - squareGizmo.transform.position.y) * -1; // * -1 since scales are swaped negative is up positive is down

                //squareObject.transform.localScale = new Vector3(_endXDistance * 2, _endYDistance * 2, 0);
                //squareObject.transform.position = startMousePosition + ((_mousePos - startMousePosition) / 2);

                if (Vector3.Magnitude(_mousePos - startMousePosition) > dragMagnitude)
                {
                    IsDrawing = true;
                }

                //Process current draw object type
                currentShapeDrawObj?.OnDrawUpdate();

            }
        }


    }

    private void OnDrawGizmosRect()
    {


    }

    private void OnDrawGizmosCircle()
    {


    }

    /// <summary>
    /// Crude functions will change later
    /// </summary>
    void OnStartDrawing()
    {
        //Process the draw object
        currentShapeDrawObj?.OnDrawStart();

    }

    /// Crude functions will change later
    void OnEndDrawing()
    {

        //Debug.Log("End Drawing");

        //squareGizmo.SetActive(false);
        //marker.SetActive(false);

        //Process the draw object
        currentShapeDrawObj?.OnDrawEnd();

    }

    public void SetDrawType(ShapeDrawType? type)
    {
        currentDrawType = type.Value;

        currentShapeDrawObj = CreateShapeDrawObjectOfType(type.Value);

        //If shape draw object is null disable gizmos
        if (currentShapeDrawObj == null)
        {
            squareGizmo.SetActive(false);
            circleGizmo.SetActive(false);
            marker.SetActive(false);
        }
        else
        {
            currentShapeDrawObj.tManager = this;
        }
    }

    #endregion

    void ProcessInputs()
    {
        //Set draw type
        if (IsDraging)
        {
            //if (Input.GetKey(KeyCode.LeftShift) && currentDrawType == ShapeDrawType.RECT)
            //{
            //    currentDrawType = ShapeDrawType.SQUARE;
            //}
        }
        else
        {
            //currentDrawType = ShapeDrawType.NONE;
        }

        if (Input.GetMouseButton(0)) // Mouse is being held down
        {
            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {

                var rB = PhysicsSimulatorManager.Instance.Get2dRigidbodyAtPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                isTouchStartedOnShape = rB != null;
            }
           
        }

        if (Input.GetMouseButton(0)) // Mouse is being held down
        {
            if (Input.GetMouseButtonDown(0)) // mouse/touch start / was just clicked down
            {
                startMousePositionScreen = Input.mousePosition;
                startMousePositionScreen.z = 0;
            }
            else
            {
                // mouse held down or touch held down
                mousePositionScreen = Input.mousePosition;
                mousePositionScreen.z = 0;

                if (Vector3.Magnitude(mousePositionScreen - startMousePositionScreen) > dragMagnitude)
                {
                    IsDraging = true;
                    //Invoke OnDrag started
                    OnDragStarted?.Invoke();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (IsDraging)
            {
                IsDraging = false;
                OnDragEnded?.Invoke();
            }
        }

        prevDrawType = currentDrawType;
    }

    void ProcessShapeDrawObjectType()
    {
        if (currentShapeDrawObj == null)
        {
            if (currentDrawType != ShapeDrawType.NONE)
            {
                currentShapeDrawObj = CreateShapeDrawObjectOfType(currentDrawType);
                currentShapeDrawObj.tManager = this;

            }
        }
        else
        {
            if (currentDrawType == ShapeDrawType.NONE)
            {
                currentShapeDrawObj = CreateShapeDrawObjectOfType(currentDrawType);
            }
            else if (currentDrawType != currentShapeDrawObj.type)
            {
                currentShapeDrawObj = CreateShapeDrawObjectOfType(currentDrawType);
                currentShapeDrawObj.tManager = this;
            }
        }

    }

    ShapeDrawBase CreateShapeDrawObjectOfType(ShapeDrawType type)
    {
        ShapeDrawBase sD = null;

        switch (type)
        {
            case ShapeDrawType.NONE:
                break;
            case ShapeDrawType.SQUARE:
                sD = new ShapeDrawSquare();
                break;
            case ShapeDrawType.RECT:
                sD = new ShapeDrawRect();
                break;
            case ShapeDrawType.CIRCLE:
                sD = new ShapeDrawCircle();
                break;
            default:
                break;
        }

        return sD;
    }

}
