using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostScript : MonoBehaviour
{
    public GameObject keyPrefab;

    public float chaseDistance;
    public float attackDistance;

    float attackTimer = 0.0f;
    public float attackCoolDown;

    GameObject player;

    public enum State { patrol, chase, attack }
    public State currentState = State.patrol;

    NavMeshAgent agent;

    Material mat;
    public MeshRenderer ghost;

    float alpha = 1.0f;

    public float alphaSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        //mat = GetComponent<Renderer>().material;
        mat = ghost.material;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.patrol:
                if (Vector3.Distance(transform.position, player.transform.position) <= chaseDistance)
                {
                    currentState = State.chase;
                    break;
                }
                else Patrol();

                break;

            case State.chase:
                if (Vector3.Distance(transform.position, player.transform.position) > chaseDistance)
                {
                    currentState = State.patrol;
                    break;
                }

                else if (Vector3.Distance(transform.position, player.transform.position) <= attackDistance)
                {
                    attackTimer = 0;
                    currentState = State.attack;
                    break;
                }

                else
                    Chase();

                break;

            case State.attack:
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

                if (Vector3.Distance(transform.position, player.transform.position) > attackDistance)
                {
                    currentState = State.chase;
                    break;
                }

                else
                    Attack();
                break;
        }

        if(!player.GetComponent<PlayerScript>().glasses)
        {
            alpha = Mathf.Abs(Mathf.Sin(Time.time * alphaSpeed));
        }

        else
        {
            alpha = 1;
        }
            mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, alpha);
    }

    public void Patrol()
    {
        agent.isStopped = false;

        agent.SetDestination(RandomNavmeshLocation(10f));
    }

    public void Chase()
    {
        agent.isStopped = false;

        agent.SetDestination(player.transform.position);
    }

    public void Attack()
    {
        agent.isStopped = true; 

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0;
            player.GetComponent<PlayerScript>().GetHurt();
        }
    }

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

        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
