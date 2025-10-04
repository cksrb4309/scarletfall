using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public float Vx = 0;
    public CameraLimit cameraLimit = null;

    [SerializeField] private Transform PlayerTransform;

    private Vector3 pos;
    private void Start()
    {
        pos.Set(0, 0, -10f);
    }
    private void FixedUpdate()
    {
        pos.x = transform.position.x;

        if (Mathf.Abs(pos.x - PlayerTransform.position.x) > 0.1f)
        {
            float before = pos.x;

            pos.x = Mathf.Lerp(pos.x, PlayerTransform.position.x, 0.1f);

            bool isCheck = true;

            if (cameraLimit != null) isCheck = cameraLimit.IsLimit(pos.x, pos.y);

            if (isCheck)
            {
                Vx = pos.x - before;

                transform.position = pos;
            }
            else Vx = 0;
        }
        else Vx = 0;
    }

    public void SetLimit(CameraLimit cameraLimit)
    {
        this.cameraLimit = cameraLimit;
    }
}
