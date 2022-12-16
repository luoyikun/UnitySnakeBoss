using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

[System.Serializable] public class CameraEvent : UnityEvent<bool> { }
[System.Serializable] public class EffectEvent : UnityEvent<bool,bool> { }
[System.Serializable] public class ParticleEvent : UnityEvent<bool,int> { }

public class WormAI : MonoBehaviour, IMessageReceiver
{

    [HideInInspector] public CameraEvent OnBossReveal;
    [HideInInspector] public EffectEvent GroundContact;
    [HideInInspector] public ParticleEvent GroundDetection;

    [Header("Pathing")]
    [SerializeField] CinemachineSmoothPath path = default;
    [SerializeField] CinemachineDollyCart cart = default;
    [SerializeField] LayerMask terrainLayer = default;
    PlayerSpaceshipController playerShip;
    UIHooks ui;

    [HideInInspector] public Vector3 startPosition, endPosition;

    RaycastHit hitInfo;
    int totalHealth;
    int currentHealth;
    Damageable[] damageables;
    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponent<UIHooks>();

        damageables = GetComponentsInChildren<Damageable>();

        foreach (Damageable damageable in damageables)
            totalHealth += damageable.currentHitPoints;

        currentHealth = totalHealth;

        ui.SetHealth(currentHealth, totalHealth);

        playerShip = Object.FindObjectOfType<PlayerSpaceshipController>();

        AI();
    }
    void AI()
    {
        UpdatePath();
        StartCoroutine(FollowPath());
        IEnumerator FollowPath()
        {
            while (true)
            {
                //play leaving ground effect

                yield return new WaitUntil(() => cart.m_Position >= 0.06f);
                GroundContact.Invoke(true,true);
                yield return new WaitUntil(() => cart.m_Position >= 0.23f);
                GroundContact.Invoke(false,true);

                // wait to reenter ground

                yield return new WaitUntil(() => cart.m_Position >= 0.60f);
                GroundContact.Invoke(true, false);
                yield return new WaitUntil(() => cart.m_Position >= 0.90f);
                GroundContact.Invoke(false, false);
                OnBossReveal.Invoke(false);

                // wait a beat to come out of ground again
                yield return new WaitUntil(() => cart.m_Position >= 0.99f);
                yield return new WaitForSeconds(Random.Range(1, 2));

                //reset path
                UpdatePath();
                yield return new WaitUntil(() => cart.m_Position <= 0.05f);
            }
        }
    }

    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        if (type == MessageType.DAMAGED)
        {
            Damageable damageable = sender as Damageable;
            Damageable.DamageMessage message = (Damageable.DamageMessage)msg;
            currentHealth -= message.amount;

            if (currentHealth <= 0)
                ui.ReloadScene();

            ui.SetHealth(currentHealth, totalHealth);

        }
    }


    void UpdatePath()
    {
        Vector3 playerPosition = playerShip.transform.position + (playerShip.spaceshipRigidbody.velocity * 3);
        playerPosition.y = Mathf.Max(10, playerPosition.y);
        Vector3 randomRange = Random.insideUnitSphere * 100;
        randomRange.y = 0;
        startPosition = playerPosition + randomRange;
        endPosition = playerPosition - randomRange;

        if (Physics.Raycast(startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            startPosition = hitInfo.point;
 
        }

        if (Physics.Raycast(endPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            endPosition = hitInfo.point;
            GroundDetection.Invoke(false, hitInfo.transform.CompareTag("Terrain") ? 0 : 1);
        }

        path.m_Waypoints[0].position = startPosition + (Vector3.down * 15);
        path.m_Waypoints[1].position = playerPosition + (Vector3.up * 10);
        path.m_Waypoints[2].position = endPosition + (Vector3.down * 45);

        path.InvalidateDistanceCache();
        cart.m_Position = 0;

        //speed
        cart.m_Speed = cart.m_Path.PathLength / 1500;

        OnBossReveal.Invoke(true);

    }

    private void OnCollisionEnter(Collision other)
    {
        print("collision enter");
        if (other.transform.TryGetComponent(out Damageable damageable))
        {
            print("hit ship");
            Damageable.DamageMessage message = new Damageable.DamageMessage()
            {
                amount = 1,
                damageSource = transform.position
            };
            damageable.ApplyDamage(message);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 1);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 1);

    }
}
