using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolBase
{
    public ToolType type;

    public abstract void OnToolSelected();

    public abstract void OnToolUpdate();

    public abstract void OnToolDeselected();

    /// <summary>
    /// Optionally implement this method to receive OnGizmoDraw() callback
    /// </summary>
    public virtual void OnDrawGizmos()
    { }

}
