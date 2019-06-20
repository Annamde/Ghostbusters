using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostScript : MonoBehaviour, IGhostMaterial
{
    GameManager gameManager;

    public GameObject keyPrefab;

    public float chaseDistance;
    public float attackDistance;

    float attackTimer = 0.0f;
    public float attackCoolDown;

    public GameObject player;
    PlayerScript playerScript;

    public enum State { patrol, chase, attack }
    public State currentState = State.patrol;

    NavMeshAgent agent;

    Material mat;
    public MeshRenderer ghost;

    float alpha = 1.0f;

    public float alphaSpeed;

    Vector3 startPosition, gotoPosition;

    public float playerDamage;

    bool changeAlpha = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerScript>();

        gameManager = GameManager.Instance;
        gameManager.AddGhostMaterial(this);

        startPosition = transform.localPosition;
        agent = GetComponent<NavMeshAgent>();
        mat = ghost.material;

        ChangeState(State.patrol);
    }

    void Update()
    {
        UpdateState();

        if(changeAlpha)
        {
            alpha = Mathf.Abs(Mathf.Sin(Time.time * alphaSpeed));
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
        }
    }

    void ChangeState(State newState)
    {
        switch(newState)
        {
            case State.patrol:
                agent.isStopped = false;
                gotoPosition = RandomNavmeshLocation(5f);
                agent.SetDestination(gotoPosition);
                break;
            case State.chase:
                agent.isStopped = false;
                break;
            case State.attack:
                attackTimer = 0;
                agent.isStopped = true;
                break;
        }

        currentState = newState;
    }

   void UpdateState()
    {
        switch(currentState)
        {
            case State.patrol:
                if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
                {
                    ChangeState(State.chase);
                    break;
                }
                else
                    Patrol();

                break;

            case State.chase:
                if (Vector3.Distance(transform.position, player.transform.position) > chaseDistance)
                {
                    ChangeState(State.patrol);
                    break;
                }

                else if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
                {
                    ChangeState(State.attack);
                    break;
                }

                else
                    Chase();

                break;

            case State.attack:
                if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
                {
                    ChangeState(State.chase);
                    break;
                }

                else
                    Attack();
                break;
        }
    }

    #region Update States
    public void Patrol()
    {
        if(Vector3.Distance(this.transform.position, gotoPosition) <= 2f)
        {
            gotoPosition = RandomNavmeshLocation(5f);
            agent.SetDestination(gotoPosition);
        }
    }

    public void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    public void Attack()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0;
            gameManager.GetHurt(playerDamage);
        }
    }
    #endregion

    public void Die()
    {
        if (keyPrefab != null)
        {
            Instantiate(keyPrefab, transform.position, Quaternion.identity);
        }

        Destroy(this.gameObject);
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }

        return finalPosition;
    }

    #region IGhostMaterial
    public void ChangeMaterialToGlasses()
    {
        changeAlpha = false;
        alpha = 1;
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }

    public void ChangeMaterialToNonGlasses()
    {
        changeAlpha = true;
    }
    #endregion
}
