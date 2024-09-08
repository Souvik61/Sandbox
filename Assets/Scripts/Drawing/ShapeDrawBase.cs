using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract class to be used with Gizmo drawing
/// </summary>
public abstract class ShapeDrawBase
{
    public ShapeDrawType type;
    public TouchManager tManager;

    public virtual void OnDrawStart() { }
    public virtual void OnDrawUpdate() { }
    public virtual void OnDrawEnd() { }

}
