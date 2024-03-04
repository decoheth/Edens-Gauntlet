using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant_Mortar : MonoBehaviour
{
    [Header("Attack")]
    public float attackDamage = 5f;
    public GameObject mortarShellPrefab;
    public GameObject spawnPoint;
    public float attackRange= 60f;
    public float minimumRange= 10f;
    public bool canAttack;
    public float attackCooldown = 3f;
    public float rotationSpeed = 5f;
    public float launchAngle = 45f;

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
            if (distance_target <= attackRange && distance_target > minimumRange) 
            {
                FaceTarget(closestEnemy);

                // Attack
                if (canAttack)
                {
                    Attack(closestEnemy);
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


        StartCoroutine(ResetAttackCooldown());
    }

    void Attack(Transform target)
    {
        canAttack = false;
        //Attack Animation

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = target.position;

        float gravity = Physics.gravity.y;
        float angle = launchAngle;


        // Calculate the distance to the target
        float distance = Vector3.Distance(initialPosition, targetPosition);

        // Calculate the initial velocity required to reach the target
        float initialVelocity = Mathf.Sqrt(Mathf.Abs((distance * gravity) / Mathf.Sin(2 * Mathf.Deg2Rad * angle)));

        // Calculate the direction to the target
        Vector3 direction = (targetPosition - initialPosition).normalized;

        // Calculate the initial velocity components
        float vx = initialVelocity * direction.x;
        float vy = initialVelocity * Mathf.Sin(angle * Mathf.Deg2Rad);
        float vz = initialVelocity * direction.z;

        // Create and launch the projectile
        GameObject shell = Instantiate(mortarShellPrefab, spawnPoint.transform.position, Quaternion.identity);
        Rigidbody rb = shell.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(vx, vy, vz);

        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
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
