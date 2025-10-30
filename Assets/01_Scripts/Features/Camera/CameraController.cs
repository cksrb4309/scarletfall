using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public float deltaX = 0;
    public CameraLimit cameraLimit = null;

    [SerializeField] private Transform PlayerTransform;
    [SerializeField] private Camera cam;

    private Vector3 pos;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
        pos = transform.position;
        pos.z = -10f;
    }

    private void FixedUpdate()
    {
        pos.x = transform.position.x;

        if (Mathf.Abs(pos.x - PlayerTransform.position.x) > 0.1f)
        {
            float before = pos.x;
            pos.x = Mathf.Lerp(pos.x, PlayerTransform.position.x, 0.1f);

            if (cameraLimit != null)
            {
                // 카메라 반 너비(화면 절반)
                float halfWidth = cam.orthographicSize * cam.aspect;

                // 카메라가 보여주는 실제 영역 기준으로 제한
                float leftLimit = cameraLimit.left + halfWidth;
                float rightLimit = cameraLimit.right - halfWidth;

                pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
            }

            deltaX = pos.x - before;
            transform.position = pos;
        }
        else deltaX = 0;
    }

    public void SetLimit(CameraLimit cameraLimit)
    {
        this.cameraLimit = cameraLimit;
    }
}