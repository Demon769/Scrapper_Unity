using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public bool isAi;
    public int amount;
    Vector3 moveDir = Vector3.zero;
    public Color color;

    //for player
    public float speed;
    CharacterController controller;

    //for ai
    NavMeshAgent agent;

    private void Start()
    {
        amount = 1;
        color.a = .4f;

        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", color);
        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_OutlineColor", color);

        if (!isAi)
        {
            controller = GetComponent<CharacterController>();
            //Debug.Log(controller);
        }
        else
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    private void Update()
    {
        if (!isAi)
        {
            moveDir = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")).normalized;
            moveDir *= speed;

            controller.Move(moveDir * Time.deltaTime);

            if (moveDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(moveDir);
        }
        else
        {
            agent.SetDestination(moveDir);
            SetDir();
        }
        //Debug.Log("check");
        CheckCollider();
    }

    void SetDir()
    {
        if (agent.velocity == Vector3.zero)
        {
            moveDir = transform.position + Random.insideUnitSphere * Random.Range(10, 40);
        }
    }

    public void AddAmount(int number)
    {
        amount += number;
    }

    public void CheckCollider()
    {
        foreach (Collider col in Physics.OverlapSphere(transform.position, 2))
        {
            if (gameObject.name == "Player1(Clone)" && col.gameObject.tag == "Npc" && col.GetComponent<NpcAgent>().player != gameObject)
            {
                //Debug.Log(gameObject.name);
                AddNpc(col.gameObject);
            }
        }
        foreach (Collider col in Physics.OverlapSphere(transform.position, 5))
        {
            if (col.gameObject.tag == "Player" && col.GetComponent<Agent>().amount < amount)
            {
                //Debug.Log("col with"+col.gameObject.name);
                KillPlayer(col.gameObject);
            }
        }
    }

    public void AddNpc(GameObject npc)
    {
        NpcAgent npcController = npc.GetComponent<NpcAgent>();

        if (npcController.currentState == NpcAgent.State.Wondering)
        {
            npcController.AddPlayer(gameObject);
            npcController.offset = npcController.offset * Mathf.RoundToInt(amount/5);
            AddAmount(1);
        }
        else if (npcController.currentState == NpcAgent.State.Following && npcController.player != gameObject)
        {
            if (npcController.player.GetComponent<Agent>().amount < amount)
            {
                npcController.player.GetComponent<Agent>().AddAmount(-1);
                npcController.AddPlayer(gameObject);
                npcController.offset = npcController.offset * Mathf.RoundToInt(amount / 5);
                AddAmount(1);
            }
        }
    }

    public void KillPlayer(GameObject player)
    {
        //Debug.Log("Kill");
        if (player.GetComponent<Agent>().amount == 1)
        {
            GameManager.GM.DestroyPlayer(player);
            GameManager.GM.SpawnFollowers(gameObject);
        }
    }
}
