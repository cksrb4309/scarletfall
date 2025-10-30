using UnityEngine;

// 하나의 레이어 단위 구조체
[System.Serializable]
public class ParallaxLayer
{
    public string layerName;
    public RectTransform[] backgrounds; // 2~4장 정도
    public float parallaxSpeed = 0.5f;
}

public class ParallaxController : MonoBehaviour
{
    [SerializeField] private CameraController cam;
    [SerializeField] private ParallaxLayer[] layers;

    private float screenWidth;
    private Vector2[][] positions; // 각 레이어별 배경 위치 저장

    private void Start()
    {
        screenWidth = Screen.width;

        // positions 초기화
        positions = new Vector2[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
        {
            positions[i] = new Vector2[layers[i].backgrounds.Length];
        }

        InitializeLayers();
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(cam.deltaX) > 0.00000001f)
        {
            MoveLayers(cam.deltaX);
            HandleLooping();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        if (Mathf.Abs(screenWidth - Screen.width) > 1f)
        {
            screenWidth = Screen.width;
            UpdateLayersResolution();
        }
    }

    // 초기 위치 + 크기 설정
    private void InitializeLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            ParallaxLayer layer = layers[i];
            for (int j = 0; j < layer.backgrounds.Length; j++)
            {
                RectTransform bg = layer.backgrounds[j];
                // Stretch
                bg.anchorMin = Vector2.zero;
                bg.anchorMax = Vector2.one;
                bg.pivot = new Vector2(0.5f, 0.5f);
                //bg.sizeDelta = new Vector2(screenWidth, Screen.height);

                // 초기 위치: index 기준
                positions[i][j] = new Vector2((j - 1) * screenWidth, 0);
                bg.anchoredPosition = positions[i][j];
            }
        }
    }

    // 배경 이동
    private void MoveLayers(float deltaX)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            float moveX = deltaX * layers[i].parallaxSpeed * Time.fixedDeltaTime;
            for (int j = 0; j < layers[i].backgrounds.Length; j++)
            {
                positions[i][j].x += moveX;
                layers[i].backgrounds[j].anchoredPosition = positions[i][j];
            }
        }
    }

    // 루프 처리
    private void HandleLooping()
    {
        foreach (var layer in layers)
        {
            int len = layer.backgrounds.Length;
            for (int j = 0; j < len; j++)
            {
                RectTransform leftMost = layer.backgrounds[0];
                RectTransform rightMost = layer.backgrounds[0];

                float leftX = layer.backgrounds[0].anchoredPosition.x;
                float rightX = layer.backgrounds[0].anchoredPosition.x;

                // 좌우 최상단 배경 탐색
                foreach (var bg in layer.backgrounds)
                {
                    float x = bg.anchoredPosition.x;
                    if (x < leftX)
                    {
                        leftX = x;
                        leftMost = bg;
                    }
                    if (x > rightX)
                    {
                        rightX = x;
                        rightMost = bg;
                    }
                }

                // 왼쪽 이동 중
                if (cam.deltaX < 0)
                {
                    if (rightMost.anchoredPosition.x - screenWidth > screenWidth)
                    {
                        leftMost.anchoredPosition = new Vector2(rightMost.anchoredPosition.x + screenWidth, 0);
                    }
                }
                // 오른쪽 이동 중
                else if (cam.deltaX > 0)
                {
                    if (leftMost.anchoredPosition.x + screenWidth < -screenWidth)
                    {
                        rightMost.anchoredPosition = new Vector2(leftMost.anchoredPosition.x - screenWidth, 0);
                    }
                }
            }
        }
    }

    // 해상도 변경 시 각 레이어 배경 재배치
    private void UpdateLayersResolution()
    {
        if (layers == null) return;

        for (int i = 0; i < layers.Length; i++)
        {
            ParallaxLayer layer = layers[i];
            if (layer == null || layer.backgrounds == null || layer.backgrounds.Length == 0) continue;

            for (int j = 0; j < layer.backgrounds.Length; j++)
            {
                RectTransform bg = layer.backgrounds[j];
                if (bg == null) continue;

                // 화면 순서 유지
                if (positions != null && positions.Length > i && positions[i].Length > j)
                {
                    positions[i][j].x = (j - 1) * screenWidth;
                    positions[i][j].y = 0;
                    bg.anchoredPosition = positions[i][j];
                }
            }
        }
    }
}