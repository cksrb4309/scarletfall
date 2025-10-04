using UnityEngine;

public class Bow : MonoBehaviour
{
    public Transform startPos;
    public PlayerController pc;
    public float speed;
    Vector3 dir = Vector3.left;

    private void OnEnable()
    {
        if (Random.value <= 0.5f)
        {
            Arrow arrow = PoolingManager.Instance.GetObject<Arrow>("Arrow");

            Debug.Log("Arrow:" + arrow.gameObject.name);

            arrow.transform.position = startPos.position;

            arrow.transform.localScale = new Vector3(pc.currentDir == Dir.Right ? 1 : -1, 1, 1);

            arrow.speed = speed;

            arrow.dir = new Vector3(pc.currentDir == Dir.Right ? 1 : -1, 0, 0);

            arrow.CallDestroy();
        }
    }
}
