using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null) SetupInstance();

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
    }
    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("°ãÄ¡´Â Singleton Á¦°Å : " + typeof(T).Name);

            Destroy(gameObject);
        }
    }
}
