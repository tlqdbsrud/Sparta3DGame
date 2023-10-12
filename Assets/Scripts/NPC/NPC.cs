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

    [Header("Wandering")] // 방황
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;


    [Header("Combat")] // 공격
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
        animator = GetComponentInChildren<Animator>(); // 하위
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

    // 도망
    private void FleeingUpdate()
    {
        if (agent.remainingDistance < 0.1f) // 이동 거리 가까우면
        {
            agent.SetDestination(GetFleeLocation()); 
        }
        else
        {
            SetState(AIState.Wandering); // 경로가 남아있으면 방황하기
        }
    }

    private void AttackingUpdate()
    {
        if (playerDistance > attackDistance || !IsPlaterInFireldOfView()) // 공격 거리보다 거리가 멀면
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();
            if (agent.CalculatePath(PlayerController.instance.transform.position, path)) // 경로 새로 검색
            {
                agent.SetDestination(PlayerController.instance.transform.position); // 목적지로 찾아서 이동
            }
            else
            {
                SetState(AIState.Fleeing); // 도망
            }
        }
        else
        {
            // 공격 모드
            agent.isStopped = true;
            if (Time.time - lastAttackTime > attackRate) // 이 시간 안에서 공격
            {
                lastAttackTime = Time.time;
                PlayerController.instance.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack"); // 공격 애니메이션
            }
        }
    }

    private void PassiveUpdate()
    {
        if (aiState == AIState.Wandering && agent.remainingDistance < 0.1f) // 방황하다가 거리가 짧으면
        {
            SetState(AIState.Idle); // Idle animation 실행(wandering -> Idle)
            Invoke("WanderToNewLocation", Random.Range(minWanderWaitTime, maxWanderWaitTime)); // 방황하기 랜덤 실행
        }

        if (playerDistance < detectDistance) // 플레이어와의 거리가 가까워지면
        {
            SetState(AIState.Attacking); // 공격
        }
    }

    
    bool IsPlaterInFireldOfView() // 내 눈에 들어오는 부분에서만 공격 가능
    {
        Vector3 directionToPlayer = PlayerController.instance.transform.position - transform.position; // 플레이어 바라보는 방향
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f; // 반을 나눔.
    }

    private void SetState(AIState newState)
    {
        aiState = newState;
        switch (aiState)
        {
            case AIState.Idle:
                {
                    agent.speed = walkSpeed;// 걷는 속도
                    agent.isStopped = true; // Idle 때 멈춤
                }
                break;
            case AIState.Wandering:
                {
                    agent.speed = walkSpeed; // 걷는 속도
                    agent.isStopped = false; // Wandering 때 이동
                }
                break;

            case AIState.Attacking:
                {
                    agent.speed = runSpeed; // 달리는 속도
                    agent.isStopped = false; // Attack 때 이동
                }
                break;
            case AIState.Fleeing:
                {
                    agent.speed = runSpeed; // 달리는 속도
                    agent.isStopped = false; // 
                }
                break;
        }

        animator.speed = agent.speed / walkSpeed; // runSpeed는 기본 속도에 비례하게 좀 더 빨라짐.
    }

    void WanderToNewLocation()
    {
        if (aiState != AIState.Idle) // Idle이 아니면
        {
            return;
        }
        SetState(AIState.Wandering); // 방황하며
        agent.SetDestination(GetWanderLocation()); // 목적지 정하기
    }


    Vector3 GetWanderLocation()
    {
        NavMeshHit hit;

        // 경로상 가장 가까운 것을 가져옴
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

    Vector3 GetFleeLocation() // 도망가는 경로를 제대로 찾기 
    {
        NavMeshHit hit;

        NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);

        int i = 0;
        while (GetDestinationAngle(hit.position) > 90 || playerDistance < safeDistance) // 도망 위치 찾기
        {
            
            NavMesh.SamplePosition(transform.position + (Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position; // 도망가는 위치 사용
    }

    float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPos);
    }

    // 데미지 처리
    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash()); // 데미지 깜박임
    }

    // 사망
    void Die()
    {
        for (int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        }

        Destroy(gameObject); // NPC 오브젝트 제거
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