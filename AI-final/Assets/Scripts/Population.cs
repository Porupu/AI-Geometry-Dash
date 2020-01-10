using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    public GameObject player;
    public GameObject goal;
    private GameObject champion; //player with best fitness score

    public static int playerNum = 20; //number of players in a generation (?)
    public GameObject[] Players;

    private Vector3 spawn = new Vector3(0.0f, -2.0f, 0.0f); //spawnpoint location - needs to be figured out

    private float fitnessSum;
    private float mutationRate = 0.1f; //2% of moveemnt vectors will be modified for each new player (2%)
    private int minStep = Player.brainSize; //minimum of steps taken to reach the goal
    public int generation = 0;

    private bool noWinnerBefore = true;
    private long k = 0; //counter


    // Start is called before the first frame update
    void Start()
    {
        Players = new GameObject[playerNum];
        SpawnPlayers();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (k % 5 == 0) //update only once per 5 physics updates
        {
            k = 0;
            if (ReachedTheGoal())
            {
                if (noWinnerBefore)
                {
                    print("Success, winner was born in " + generation + " generation");
                    noWinnerBefore = false;
                }
            }

            if (AllDead()) //if everyone is dead, evaluate their score and select the champion, pause for 1 second
            {
                NaturalSelection();
                champion.GetComponent<Player>().SetAsChampion();
                StartCoroutine(PauseAndRespawn()); //included in coroutine so the pause works and then respawn
            }
            else
            {
                for (int i = 0; i < playerNum; i++)
                {
                    if (Players[i].GetComponent<Player>().i > minStep)
                    {
                        Players[i].GetComponent<Player>().Die(); //player dies when takes more steps then the champion that reached the goal - so it optimizes the moves to reach goal faster
                    }
                    else if (Players[i].GetComponent<Rigidbody>().velocity.magnitude < Player.maxSpeed) //movement vector is applied only if player hasnt crossed the max speed limit
                    {
                        Players[i].GetComponent<Player>().MovePlayer();
                    }
                }
                        
            }
                    
        }
        k++;
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < playerNum; i++)
        {
            GameObject player_x; //spawn player
            player_x = Instantiate(player, spawn, Quaternion.identity) as GameObject;

            //generate name for player
            string[] name_tmp = { "player", (i + 1).ToString() };
            name = string.Join("", name_tmp, 0, 2);
            player_x.name = name;

            Players[i] = player_x; //assign player to array

            //calculate distance from spawn to goal 
            Players[i].GetComponent<Player>().distToGoalFromSpawn = Vector3.Distance(Players[i].transform.position, goal.transform.position);

        }
    }

    IEnumerator PauseAndRespawn()
    {
        enabled = false; //turn off update func
        yield return new WaitForSeconds(1.0f); // pause for 1 sec
        enabled = true;

        RespawnAll();
        generation++;

        if (generation % 5 == 0)
        {
            IncreaseLifespan(); //every 5 generations expand lifetime
        }
    }
       
    bool AllDead()
    {
        for (int i = 0; i < playerNum; i++)
        {
            if (Players[i].GetComponent<Player>().dead == false)
            {
                return false;
            }
        }
        return true;
    }

    bool ReachedTheGoal() //return true if any player reached the goal
    {
        bool reachedTheGoal = false;
        for (int i = 0; i < playerNum; i++)
        {
            if (Players[i].GetComponent<Player>().reachedGoal == true)
            {
                if (Players[i].GetComponent<Player>().i < minStep)
                {
                    minStep = Players[i].GetComponent<Player>().i;
                }
                reachedTheGoal = true;
            }
        }
        if (reachedTheGoal)
            return true;
        else
            return false;
    }

    void RespawnAll()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Players[i].GetComponent<Player>().Respawn();
        }
        Players[0].GetComponent<Player>().SetAsChampion(); //makes the champion green
    }
        
    void SetChampion()
    {
        float best_score = Vector3.Distance(Players[0].transform.position, goal.transform.position);
        champion = Players[0];

        for (int i = 1; i < playerNum; i++)
        {
            float DistanceToGoal = Vector3.Distance(Players[i].transform.position, goal.transform.position);
            if (DistanceToGoal < best_score)
            {
                best_score = DistanceToGoal;
                champion = Players[i];
            }
        }
    }

    void IncreaseLifespan()
    {
        for (int i = 0; i < playerNum; i++)
        {
            Players[i].GetComponent<Player>().lifespan += 50;
        }
    }

    void NaturalSelection()
    {
        SetChampion();

        CalculateFitness();
        CalculateFitnessSum();

        CopyBrain(Players[0], champion); //champion is always reborn in the next generation unchanged


        for (int i = 1; i < playerNum; i++) //i=1 to exclude the champion and then copy and mutate the remaining 99
        {
            GameObject parent = champion; //SelectParent();
            CopyBrain(Players[i], parent);
            Mutate(Players[i]);
        }
        //for (int i = (playerNum / 2) - 1; i < playerNum; i++)
        //{
        //    Players[i].GetComponent<Player>().brain[i] = new Vector3(0, Random.Range(0, 2) * 5, 0);
        //}
    }

    void CopyBrain (GameObject P1, GameObject P2)
    {
        for (int i = 0; i < Player.brainSize; i++)
        {
            P1.GetComponent<Player>().brain[i][0] = P2.GetComponent<Player>().brain[i][0]; // vectors on x
            P1.GetComponent<Player>().brain[i][1] = P2.GetComponent<Player>().brain[i][1]; //y
            P1.GetComponent<Player>().brain[i][2] = P2.GetComponent<Player>().brain[i][2]; //z
        }
    }

    void CalculateFitness()
    {
        for (int i = 0; i < playerNum; i++)
        {
            float DistanceToGoal = Vector3.Distance(Players[i].transform.position, goal.transform.position);

            if (Players[i].GetComponent<Player>().reachedGoal)
            {
                int step = Players[i].GetComponent<Player>().i;
                float distToGoalFromSpawn = Players[i].GetComponent<Player>().distToGoalFromSpawn;
                Players[i].GetComponent<Player>().fitness = 1.0f / 24 + distToGoalFromSpawn * 100 / (step * step);
            }
            else
            {
                //Players[i].GetComponent<Player>().fitness = 10.0f / (DistanceToGoal * DistanceToGoal * DistanceToGoal * DistanceToGoal);
                Players[i].GetComponent<Player>().fitness = (150.0f / DistanceToGoal)*10;
            }
        }
                
    }

    void CalculateFitnessSum()
    {
        fitnessSum = 0;
        for (int i = 0; i < playerNum; i++)
        {
            fitnessSum += Players[i].GetComponent<Player>().fitness;
        }
    }

    GameObject SelectParent()
    {
        float rand = Random.Range(0.0f, fitnessSum);
        float runningSum = 0;

        for (int i = 0; i < playerNum; i++)
        {
            runningSum += Players[i].GetComponent<Player>().fitness;
            if (runningSum >= rand)
            {
                return Players[i];
            }
        }
        return null; //should never happen
    }

    void Mutate(GameObject PlayerX)
    {
        for (int i = 0; i < PlayerX.GetComponent<Player>().lifespan && i < 300; i++)
        {
            float rand = Random.Range(0.0f, 1.0f);
            if (rand < mutationRate)
            {
                PlayerX.GetComponent<Player>().brain[i] = new Vector3(0, Random.Range(0, 2) * 5, 0); //x y z ; gonna need to change the y so they jump; x = Random.Range(10, -11)
            }
        }
    }
}
