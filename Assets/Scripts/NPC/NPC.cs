using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Wandering,
    Attacking,
    Fleeing
}


public class NPC : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private AIState aiState;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")] // ��Ȳ
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;


    [Header("Combat")] // ����
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f;

    private NavMeshAgent agent;
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>(); // ����
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch (aiState)
        {
            case AIState.Idle: PassiveUpdate(); break;
            case AIState.Wandering: PassiveUpdate(); break;
            case AIState.Attacking: AttackingUpdate(); break;
            case AIState.Fleeing: FleeingUpdate(); break;
        }

    }

    // ����
    private void FleeingUpdate()
    {
        if (agent.remainingDistance < 0.1f) // �̵� �Ÿ� ������
        {
            agent.SetDestination(GetFleeLocation()); 
        }
        else
        {
            SetState(AIState.Wandering); // ��ΰ� ���������� ��Ȳ�ϱ�
        }
    }

    private void AttackingUpdate()
    {
        if (playerDistance > attackDistance || !IsPlaterInFireldOfView()) // ���� �Ÿ����� �Ÿ��� �ָ�
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(PlayerController.instance.transform.position, path)) // ��� ���� �˻�
            {
                agent.SetDestination(PlayerController.instance.transform.position); // �������� ã�Ƽ� �̵�
            }
            else
            {
                SetState(AIState.Fleeing); // ����
            }
        }
        else
        {
            // ���� ���
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate) // �� �ð� �ȿ��� ����
            {
                lastAttackTime = Time.time;
                PlayerController.instance.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack"); // ���� �ִϸ��̼�
            }
        }
    }

    private void PassiveUpdate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f) // ��Ȳ�ϴٰ� �Ÿ��� ª����
        {
            SetState(AIState.Idle); // Idle animation ����(wandering -> Idle)
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime)); // ��Ȳ�ϱ� ���� ����
        }

        if (playerDistance < detectDistance) // �÷��̾���� �Ÿ��� ���������
        {
            SetState(AIState.Attacking); // ����
        }
    }

    
    bool IsPlaterInFireldOfView() // �� ���� ������ �κп����� ���� ����
    {
        Vector3 directionToPlayer = PlayerController.instance.transform.position - transform.position; // �÷��̾� �ٶ󺸴� ����
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f; // ���� ����.
    }

    private void SetState(AIState newState)
    {
        aiState = newState;
        switch (aiState)
        {
            case AIState.Idle:
                {
                    agent.speed = walkSpeed;// �ȴ� �ӵ�
                    agent.isStopped = true; // Idle �� ����
                }
                break;
            case AIState.Wandering:
                {
                    agent.speed = walkSpeed; // �ȴ� �ӵ�
                    agent.isStopped = false; // Wandering �� �̵�
                }
                break;

            case AIState.Attacking:
                {
                    agent.speed = runSpeed; // �޸��� �ӵ�
                    agent.isStopped = false; // Attack �� �̵�
                }
                break;
            case AIState.Fleeing:
                {
                    agent.speed = runSpeed; // �޸��� �ӵ�
                    agent.isStopped = false; // 
                }
                break;
        }

        animator.speed = agent.speed / walkSpeed; // runSpeed�� �⺻ �ӵ��� ����ϰ� �� �� ������.
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) // Idle�� �ƴϸ�
        {
            return;
        }
        SetState(AIState.Wandering); // ��Ȳ�ϸ�
        agent.SetDestination(GetWanderLocation()); // ������ ���ϱ�
    }


    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        // ��λ� ���� ����� ���� ������
        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * Random.Range(minWanderDistance, maxWanderDistance)), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    Vector3 GetFleeLocation() // �������� ��θ� ����� ã�� 
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (GetDestinationAngle(hit.position) > 90 || playerDistance < safeDistance) // ���� ��ġ ã��
        {
            
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position; // �������� ��ġ ���
    }

    float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPos);
    }

    // ������ ó��
    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash()); // ������ ������
    }

    // ���
    void Die()
    {
        for (int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject); // NPC ������Ʈ ����
    }

    IEnumerator DamageFlash()
    {
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);

        yield return new WaitForSeconds(0.1f);
        for (int x = 0; x < meshRenderers.Length; x++)
            meshRenderers[x].material.color = Color.white;
    }
}