using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Plant_SeedShooter : MonoBehaviour
{
    [Header("Attack")]
    public float attackDamage = 5f;
    public float attackRange= 15f;
    public bool canAttack;
    public float attackCooldown = .5f;
    public float rotationSpeed = 10f;

    //Managers
    WaveManager waveManager;


    void Awake()
    {
        waveManager = GameObject.Find("Managers/Enemy Manager").GetComponent<WaveManager>();
    }

    void Start()
    {
        canAttack = true;
    }

    void Update ()
    {
        // Distance from agent to enemy

        if(waveManager.currentEnemyWave.Count > 0)
        {
            Transform closestEnemy = GetClosestEnemy(waveManager.currentEnemyWave);
            float distance_target = Vector3.Distance(closestEnemy.position, transform.position);
            
            // If enemy comes into the attack range
            if (distance_target <= attackRange)
            {
                FaceTarget(closestEnemy);

                // Attack
                if (canAttack)
                {
                    Attack();
                }  
            }
        }

    }

    void FaceTarget (Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,direction.y,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
    
    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


    public void Attack()
    {
        canAttack = false;
        // Attack Animation
        AttackRaycast();
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void AttackRaycast()
    {

        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange))
        { 
            // Attack Enemy
            Debug.Log("Hit: " + hit.transform);

            if(hit.transform.TryGetComponent<EnemyStats>(out EnemyStats T))
            { 
            T.TakeDamage(attackDamage); 
            }

            else
            {
                Debug.Log("No Target Found");
            }
        } 
    }

    Transform GetClosestEnemy(List<GameObject> enemies)
    {
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject t in enemies)
        {
            float dist = Vector3.Distance(t.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t.transform;
                minDist = dist;
            }
        }
        return tMin;
    }

}
