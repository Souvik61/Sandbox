using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	public static bool InstanceValid => Instance != null;
    public static T Instance { get { return instance; } }
    private static T instance;

	public static TSingletonClass GetInstance<TSingletonClass>() where TSingletonClass : Singleton<T>
    {
        return Instance as TSingletonClass;
    }

    protected static void SetInstance(T _NewInstance)
    {
        instance = _NewInstance;
    }

    protected virtual void Awake()
    {
        if (null == instance || this == instance)
        {
            instance = this as T;
        }
        else
        {
            // What to do when an instance already exists?
            Debug.LogError("Instance of type " + typeof(T).ToString() + " already exists! Gameobject: " + gameObject.name + " at address: " + 
                instance.transform.GetTransformPath() + " new " + gameObject.name + " is at address: " + instance.transform.GetTransformPath());
        }
    }

    public void DontDestroy()
    {
        if ((transform.parent == null) && (this == instance))
            DontDestroyOnLoad(instance);
    }

    protected virtual void OnDestroy()
    {
        if (this == instance)
            instance = null;
    }
}
