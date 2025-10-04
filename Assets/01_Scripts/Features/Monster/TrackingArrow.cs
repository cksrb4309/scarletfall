using System.Collections;
using UnityEngine;

public class TrackingArrow : MonoBehaviour
{
    public BoxCollider2D attackCollider;
    public SpriteRenderer arrowRenderer;
    public LineRenderer arrowTrail;
    public LineRenderer targetLine;
    
    public Rigidbody2D rb;
    Vector3 PlayerPos { get { return GameManager.Instance.player.transform.position; } }

    float arrowTrailMaxWidth = 0.3f;

    public void StartMove(Vector3 pos)
    {
        rb.angularVelocity = 0;

        rb.AddTorque(80);

        attackCollider.enabled = false;

        transform.position = pos;

        arrowRenderer.enabled = true;

        arrowTrail.enabled = false;

        targetLine.enabled = false;

        StartCoroutine(StartCoroutine());
    }

    IEnumerator StartCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        rb.angularVelocity = 0;

        float t = 0;

        float currentAngle = transform.eulerAngles.z;

        float nextAngle;

        while (t < 1f)
        {
            t += Time.deltaTime * 0.5f;

            Vector3 dir = (PlayerPos - transform.position).normalized;

            if (t > 0.5f) targetLine.enabled = true;

            nextAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            Debug.Log("NextAngle:" + nextAngle.ToString());

            rb.SetRotation(Mathf.LerpAngle(currentAngle, nextAngle, t));

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        SoundManager.Play("RedHoodTrackingArrow", SoundType.Effect);

        targetLine.enabled = false;

        arrowTrail.enabled = true;

        arrowRenderer.enabled = false; // 화살 Renderere 비활성화

        attackCollider.enabled = true; // 공격 콜라이더 활성화

        arrowTrail.enabled = true; // 화면 공격 경로 표시

        t = 0;

        Vector2 attackColliderSize = new Vector2(30.12548f, 0.33f);
        
        while (t < 1f)
        {
            t += Time.deltaTime * 0.3f;

            arrowTrail.startWidth = Mathf.Lerp(arrowTrailMaxWidth, 0, t);

            attackColliderSize.y = Mathf.Lerp(0.33f, 0f, t);

            attackCollider.size = attackColliderSize;

            yield return null;
        }
        PoolingManager.Instance.ReturnObject("TrackingArrow", gameObject);
    }
}