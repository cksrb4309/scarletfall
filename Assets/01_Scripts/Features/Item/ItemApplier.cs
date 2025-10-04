using UnityEngine;

public class ItemApplier : MonoBehaviour
{
    public static ItemApplier instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    

}