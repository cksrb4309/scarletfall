using UnityEngine;

public class Spark : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;
    public float slowSpeed;
    float startSpeed;
    private void OnEnable()
    {
        startSpeed = Random.Range(minSpeed, maxSpeed);
    }

    private void FixedUpdate()
    {
        if (startSpeed > 0)
        {
            transform.Rotate(new Vector3(0, 0, startSpeed));

            startSpeed -= Time.fixedDeltaTime * slowSpeed;
        }
    }
}
