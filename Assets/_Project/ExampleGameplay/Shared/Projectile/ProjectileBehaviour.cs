using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    [Header("Settings")]
    public ProjectileData data;

    void OnEnable()
    {
        StartCoroutine(TimerToAutoRemoveProjectile());
    }

    void FixedUpdate()
    {
        Vector3 tempPos = transform.position;
        MoveProjectile();
        if (Physics.Linecast(tempPos, transform.position, out RaycastHit hitInfo, data.layerMask))
        {
            RemoveProjectile();
            if (hitInfo.transform.CompareTag("Enemy") && hitInfo.transform.TryGetComponent(out Damageable damageable))
            {
                damageable.ApplyDamage(new Damageable.DamageMessage()
                {
                    amount = 2,
                    damageSource = transform.position
                });
            }
        }

    }

    void MoveProjectile()
    {
        Vector3 tempVect = transform.forward * data.movementSpeed * Time.deltaTime;
        transform.position += tempVect;
    }

    IEnumerator TimerToAutoRemoveProjectile()
    {
        yield return new WaitForSeconds(data.autoRemoveCountdown);
        RemoveProjectile();
    }

    void RemoveProjectile()
    {
        gameObject.SetActive(false);
    }

}
