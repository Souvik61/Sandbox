using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ShapeDrawRect : ShapeDrawBase
{
    public override void OnDrawStart()
    {
        tManager.squareGizmo.SetActive(true);
        tManager.marker.SetActive(true);
        tManager.marker.transform.position = new Vector3(tManager.startMousePosition.x, tManager.startMousePosition.y, 0);
    }

    public override void OnDrawUpdate()
    {
        Debug.Log("Drawing Rect");

        // mouse held down or touch held down
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;

        float _endXDistance = (_mousePos.x - tManager.startMousePosition.x);
        float _endYDistance = (_mousePos.y - tManager.startMousePosition.y);

        tManager.squareGizmo.transform.localScale = new Vector3(_endXDistance, _endYDistance, 0);
        tManager.squareGizmo.transform.position = tManager.startMousePosition + ((_mousePos - tManager.startMousePosition) / 2);
    }

    public override void OnDrawEnd()
    {
        tManager.squareGizmo.SetActive(false);
        tManager.marker.SetActive(false);
    }

}

public class ShapeDrawSquare : ShapeDrawBase
{
    public override void OnDrawStart()
    {
        tManager.squareGizmo.SetActive(true);
        tManager.marker.SetActive(true);
        tManager.marker.transform.position = new Vector3(tManager.startMousePosition.x, tManager.startMousePosition.y, 0);
    }

    public override void OnDrawUpdate()
    {
        Debug.Log("Drawing Square");

        // mouse held down or touch held down
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;

        float _endXDistance = (_mousePos.x - tManager.startMousePosition.x);
        float _endYDistance = (_mousePos.y - tManager.startMousePosition.y);

        float sqSize = Mathf.Max(Mathf.Abs(_endXDistance), Mathf.Abs(_endYDistance));

        tManager.squareGizmo.transform.localScale = new Vector3(sqSize, sqSize, 0);
        tManager.squareGizmo.transform.position = tManager.startMousePosition + new Vector3(Mathf.Sign(_endXDistance) * sqSize / 2, Mathf.Sign(_endYDistance) * sqSize / 2, 0);

        //Debug.Log("Start mouse pos" + tManager.startMousePosition);

    }

    public override void OnDrawEnd()
    {
        tManager.squareGizmo.SetActive(false);
        tManager.marker.SetActive(false);
    }

}

public class ShapeDrawCircle : ShapeDrawBase
{
    public override void OnDrawStart()
    {
        tManager.marker.transform.position = new Vector3(tManager.startMousePosition.x, tManager.startMousePosition.y, 0);
        tManager.circleGizmo.SetActive(true);
    }

    public override void OnDrawUpdate()
    {
        // mouse held down or touch held down
        Vector3 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;

        float radius = Vector3.Distance(_mousePos, tManager.startMousePosition);

        tManager.circleGizmo.transform.localScale = new Vector3(radius * 2, radius * 2, 0);
        tManager.circleGizmo.transform.position = tManager.startMousePosition;

        //Debug.Log("Start mouse pos" + tManager.startMousePosition);

    }

    public override void OnDrawEnd()
    {
        tManager.circleGizmo.SetActive(false);
    }

}
