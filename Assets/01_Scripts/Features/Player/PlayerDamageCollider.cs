using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
    [SerializeField] bool isProjectile = false;
    [SerializeField] string projectileName = string.Empty;
    [SerializeField] bool isMainAttack = true;
    [SerializeField] Transform playerPosition;
    [SerializeField] string hitEffectSound = string.Empty;
    [SerializeField] string particleName = string.Empty;
    [SerializeField] float damage;

    private void Start()
    {
        if (playerPosition == null) playerPosition = GameManager.Instance.player.transform;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Monster"))
        {
            Vector2 hitPoint = other.ClosestPoint(transform.position);

            EffectController.CallAttackEffect(
                particleName, hitPoint,
                other.transform.position.x > transform.position.x ?
                false : true);

            other.GetComponent<Monster>().Hit(damage, isMainAttack);

            if (!hitEffectSound.Equals(string.Empty))
            {
                SoundManager.Play(hitEffectSound, SoundType.Effect);
            }

            if (isProjectile)
            {
                PoolingManager.Instance.ReturnObject(projectileName, gameObject);
            }
        }
    }
}