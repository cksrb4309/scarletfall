using UnityEngine;

public class ParticleAutoReturn : MonoBehaviour
{
    public string particleName;
    void ReturnParticle()
    {
        PoolingManager.Instance.ReturnObject(particleName, gameObject);
    }
}
