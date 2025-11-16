using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                SetupInstance();
            }

            return instance;
        }
    }
    private static void SetupInstance()
    {
        instance = FindFirstObjectByType<T>();

        if (instance == null)
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;

            T type = gameObj.AddComponent<T>();

            instance = type;

            DontDestroyOnLoad(gameObj);
        }
        else
        {
            if (instance.transform.parent != null)
            {
                instance.transform.SetParent(null);
            }

            DontDestroyOnLoad(instance);
        }
    }
    public virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;

            if (transform.parent != null)
            {
                transform.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
        }
    }
}
