using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public interface IObjectObserver
    {
        void OnObjectAdded(ObjectBase obj);
        void OnObjectRemoved(ObjectBase obj);
        void OnObjectUpdated(ObjectBase obj);
        void OnStateUpdated();
    }
}
