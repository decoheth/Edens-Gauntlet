using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Raider : MonoBehaviour
{


    public float attackDamage = 10;
    [SerializeField] private EnemyMeleeWeapon weapon;
    public float lookRadius = 20f;
    public bool canAttack;
    public float attackCooldown = 1f;


    Animator anim;

    Transform player;
    Transform tree;
    NavMeshAgent agent;

    GameObject enemyManager;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        tree = GameObject.FindWithTag("HomeTree").transform;
        agent = GetComponent<NavMeshAgent>();

        enemyManager = GameObject.Find("/Managers/Enemy Manager");
    }

    void Start()
    {
        canAttack = true;
        weapon.damage = attackDamage;
        anim = GetComponent<Animator>();
    }


    void Update ()
    {
        // Distance from enemy to player
        float distance_player = Vector3.Distance(player.position, transform.position);
        float distance_tree = Vector3.Distance(tree.position, transform.position);

        // By default, move towards the home tree, then attack if in radius of home tree
        if (distance_player > lookRadius)
        {
            agent.SetDestination(tree.position);

            if (distance_tree <= agent.stoppingDistance+6)
            {
                agent.ResetPath();
                // Face target
                FaceTarget(tree);
                // Attack
                if(canAttack)
                    DestroyTree();
            }
        }

        // If player comes into the aggro look radius
        if (distance_player <= lookRadius)
        {
            agent.SetDestination(player.position);

            if (distance_player <= agent.stoppingDistance)
            {
                // Face target
                FaceTarget(player);
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
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x,0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }


    public void Attack()
    {
        canAttack = false;

        // Attack Animation
        anim.SetTrigger("Attack");


        StartCoroutine(ResetAttackCooldown());
    }


    public void DestroyTree()
    {
        canAttack = false;
        // Attack Animation
        tree.GetComponent<HomeTree>().TakeDamage(attackDamage);
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

}