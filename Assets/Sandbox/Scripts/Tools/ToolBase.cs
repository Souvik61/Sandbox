using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{

    public abstract class ToolBase
    {
        public ToolType type;

        public abstract void OnToolSelected();

        public abstract void OnToolUpdate();

        public abstract void OnToolDeselected();

        public virtual void OnObjectSelected() { }

        /// <summary>
        /// If should block selection events from Edit controller
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldBlockOtherEvents();

        /// <summary>
        /// Optionally implement this method to receive OnGizmoDraw() callback
        /// </summary>
        public virtual void OnDrawGizmos()
        { }

    }
}