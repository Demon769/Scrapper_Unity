using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<Color> colors = new List<Color>();

    float timer;
    public static int score1 = 0;
    float spawn_timer;
    public TextMeshProUGUI timerUI;
    public Animator anim;
    public NpcAgent npcPrefab;
    public Agent playerPrefab;
    public Agent playerAiPrefab;
    public GameObject statPrefab;
    public GameObject pointPrefab;
 
    public GameObject buildingsRoot;
    public GameObject playersRoot;
    public GameObject npcRoot;
    public GameObject statRoot;
    public GameObject pointsRoot;

    public Camera cam;

    public List<GameObject> players;
    public List<GameObject> playersSpawnPos;
    public List<GameObject> npcWalker;
    public List<GameObject> points;

    public static GameManager GM;

    public bool enemy;


    private void Start()
    {
        GM = this;

        timer = 0;
        spawn_timer = 0;

        colors = new List<Color> {new Color(0, 0, .5f), new Color(0, .5f, .5f), new Color(.5f, 0, .5f), new Color(0, .5f, 0),
            new Color(.5f, .5f, 0), new Color(.5f, 0, 0), new Color(.5f, .5f, .5f), new Color(1, 0, 1), new Color(0, 1, 1),
            new Color(1, 1, 0),new Color(0, 0, 1),new Color(0, 1, 0),new Color(1, 0, 0),new Color(1, 1, 1) };
        enemy = false;
        for (int i = 0; i < 50; i++)
        {
            SpawnNpc();
        }
        enemy = true;
        SpawnPlayer();
    }

    private void Update()
    {
        if (npcWalker.Count < 200)
        {
            SpawnNpc();
        }

        if (players.Count <= 1 || timer >= 50)
        {
            GameOver();
        }
        else
        {
            timer += Time.deltaTime;
            spawn_timer += Time.deltaTime;
            timerUI.text = Mathf.RoundToInt(timer).ToString();
        }
    }

    void SpawnNpc()
    {
        if (enemy && spawn_timer < 1)
        {
            return;
        }
        spawn_timer = 0;
        float d = 1000;
        float x2 = 0, z2 = 0;
        int index = 0;
        //Debug.Log("Called "+enemy);
        if (enemy)
        {
            //Debug.Log("Players size" + players.Count);
            float x = players[0].transform.position.x;
            float z = players[0].transform.position.z;

            for (int i = 1; i < players.Count; i++)
            {
                float x1 = players[i].transform.position.x;
                float z1 = players[i].transform.position.z;
                float d1 = (x - x1) * (x - x1) + (z - z1) * (z - z1);
                if (d1 < d)
                {
                    d = d1;
                    x2 = x1;
                    z2 = z1;
                    index = i;
                }
            }
        }
        //Debug.Log("Minimum distance " + d + " at i " + index);
        Vector3 temp = new Vector3(Random.Range(2, 198), 1, Random.Range(2, 198));
        
        if(enemy && d<200)
        {
            temp = new Vector3(x2, 1, z2);

        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(temp, out hit, 50.0f, NavMesh.AllAreas))
        {
            Vector3 pos = hit.position;
            NpcAgent go = Instantiate(npcPrefab, pos, Quaternion.identity, npcRoot.transform);
            npcWalker.Add(go.gameObject);
        }
        //else
        //{
        //    SpawnNpc();
        //}
    }

    public void SpawnFollowers(GameObject _player)
    {
        int max = 10;

        for (int i = 0; i < max; i++)
        {
            Vector3 temp = _player.transform.position + Random.insideUnitSphere * Random.Range(3, 10);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(temp, out hit, 50.0f, NavMesh.AllAreas))
            {
                Vector3 pos = hit.position;

                NpcAgent go = Instantiate(npcPrefab, pos, Quaternion.identity, npcRoot.transform);
                go.currentState = NpcAgent.State.Following;
                go.AddPlayer(_player);
            }
            else
            {

                max++;
                continue;
            }
        }
        _player.GetComponent<Agent>().amount += 10;
    }

    void SpawnPlayer()
    {
        for (int i = 0; i < 12; i++)
        {
            if (i == 0)
            {
                Agent player = Instantiate(playerPrefab, playersSpawnPos[i].transform.position, Quaternion.identity, playersRoot.transform);
                cam.GetComponent<CameraFollow>().player = player.gameObject;
                player.color = colors[i];
                players.Add(player.gameObject);

                GameObject point = Instantiate(pointPrefab, pointsRoot.transform, false);
                point.GetComponent<PointUpdate>().player = player.gameObject;
                points.Add(point);
            }
            else
            {
                Agent ai = Instantiate(playerAiPrefab, playersSpawnPos[i].transform.position, Quaternion.identity, playersRoot.transform);
                ai.color = colors[13];
                players.Add(ai.gameObject);

                GameObject point = Instantiate(pointPrefab, pointsRoot.transform, false);
                point.GetComponent<PointUpdate>().player = ai.gameObject;
                points.Add(point);
            }
        }
    }

    public void RemoveNpcFromArray(GameObject _npc)
    {
        npcWalker.RemoveAt(npcWalker.IndexOf(_npc));
    }

    public void DestroyPlayer(GameObject _player)
    {
        int temp = players.IndexOf(_player);
        Destroy(_player.gameObject);
        players.RemoveAt(temp);

        Destroy(points[temp].gameObject);

        points.RemoveAt(temp);
    }

    public void GameOver()
    {
        Debug.Log("Gameover");
        //anim.SetTrigger("GameOver");
        foreach (Transform child in playersRoot.transform)
        {
            Destroy(child.gameObject);
        }
        players.Clear();

        foreach (Transform child in npcRoot.transform)
        {
            Destroy(child.gameObject);
        }
        npcWalker.Clear();

        foreach (Transform child in pointsRoot.transform)
        {
            Destroy(child.gameObject);
        }
        if(points.Count>0)
        {
            score1 += points[0];
        }
        points.Clear();
        Debug.Log("Exit");
        SceneManager.LoadScene(2);
        //Start();
    }
}
