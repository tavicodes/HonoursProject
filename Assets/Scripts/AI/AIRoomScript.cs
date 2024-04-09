using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRoomScript : RoomScript
{
    public enum TrainingEnum
    {
        BeginEpisode,
        SpawnEnemies,
        FightEnemies,
        IncRoom,
        EndEpisode
    }
    public GameObject playerObj;
    public GameObject aiDirectorObj;
    private TrainingEnum trainingState;
    private int roomCount;
    private int falseFloorCount;
    private int enemyCounter;
    private Queue<EnemyStatistics.EnemyEnum[]> enemyChosenQueue;
    
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        currentState = StateEnum.Play;

        trainingState = TrainingEnum.BeginEpisode;

        aiDirectorObj.GetComponent<AIDirectorScript>().SetAttachedRoom(this);
        aiDirectorObj.GetComponent<AIDirectorScript>().SetPlayerObj(playerObj);

        enemyChosenQueue = new Queue<EnemyStatistics.EnemyEnum[]>();
    }

    public new void Update()
    {
        switch (trainingState)
        {
            case TrainingEnum.BeginEpisode:
                break;
            case TrainingEnum.SpawnEnemies:
                break;
            case TrainingEnum.FightEnemies:
                foreach (GameObject enemy in enemyArray)
                {
                    if (enemy.Equals(null)) 
                    {
                        enemyArray.Remove(enemy);
                        break;
                    }
                }        
                if (Input.GetButtonDown("Debug"))
                {
                    foreach (GameObject enemy in enemyArray)
                    {
                        enemy.GetComponent<EnemyScript>().PrintStats();
                    }
                }        
                break;
            case TrainingEnum.EndEpisode:
                break;
        }
    }

    public void FixedUpdate()
    {
        switch (trainingState)
        {
            case TrainingEnum.BeginEpisode:
                falseFloorCount = Random.Range(0, 5);
                roomCount = 0;
                playerObj.GetComponent<AIPlayerScript>().Reset(transform.position, GenerateItemRarity(falseFloorCount));
                aiDirectorObj.GetComponent<AIDirectorScript>().OnEpisodeBegin();
                aiDirectorObj.GetComponent<AIDirectorScript>().RequestDecision();

                enemyCounter = 0;
                enemyArray = new List<GameObject>();
                ResetEnemyChosenQueue();

                trainingState = TrainingEnum.SpawnEnemies;
                break;
            case TrainingEnum.SpawnEnemies:
                SetEnemies();
                break;
            case TrainingEnum.FightEnemies:
                if(enemyArray.Count == 0) 
                {
                    if (roomCount >= 5 + Mathf.FloorToInt(falseFloorCount * 0.5f)) trainingState = TrainingEnum.EndEpisode;
                    else trainingState = TrainingEnum.IncRoom;
                }
                break;
            case TrainingEnum.IncRoom:
                roomCount++;
                aiDirectorObj.GetComponent<AIDirectorScript>().RequestDecision();
                playerObj.GetComponent<AIPlayerScript>().SoftReset(transform.position);
                trainingState = TrainingEnum.SpawnEnemies;
                break;
            case TrainingEnum.EndEpisode:
                aiDirectorObj.GetComponent<AIDirectorScript>().EndEpisode();
                trainingState = TrainingEnum.BeginEpisode;
                break;
        }
    }

    protected void SetEnemies()
    {
        AIDirectorScript aiDirector = aiDirectorObj.GetComponent<AIDirectorScript>();

        for (int spawnPos = 0; spawnPos < 3; spawnPos++)
        {
            GameObject tempEnemy = Instantiate(enemyPrefab, enemySpawns[spawnPos]);
            tempEnemy.transform.SetParent(transform);
            tempEnemy.GetComponent<EnemyScript>().SetPlayer(playerObj);
            
            EnemyStatistics tempStats = aiDirector.GetGeneratedStatistics(spawnPos, falseFloorCount);
            tempEnemy.GetComponent<EnemyScript>().SetValues(tempStats);
            
            enemyArray.Add(tempEnemy);
        }
        
        EnemyStatistics.EnemyEnum counterType = playerObj.GetComponent<AIPlayerScript>().GetWeaponScript().GetCounterType();
        bool isCountered = false;

        foreach (GameObject enemy in enemyArray)
        {
            if (counterType == EnemyStatistics.EnemyEnum.Sniper && enemy.GetComponent<EnemyScript>().IsRanged()) isCountered = true;
            else if (enemy.GetComponent<EnemyScript>().GetEnemyType() == counterType) isCountered = true;
            
            enemy.GetComponent<EnemyScript>().Activate();
        }
        if (isCountered) aiDirector.AddReward(0.25f);
        aiDirector.AddReward(CalculateEnemyChosenReward());
        
        UpdateEnemyChosenQueue();
        
        trainingState = TrainingEnum.FightEnemies;
    }

    protected void SetEnemy()
    {
        if (enemyCounter < 3)
        {
            AIDirectorScript aiDirector = aiDirectorObj.GetComponent<AIDirectorScript>();
            Debug.Log("Create Enemy");
            GameObject tempEnemy = Instantiate(enemyPrefab, enemySpawns[enemyCounter]);
            tempEnemy.transform.SetParent(transform);
            tempEnemy.GetComponent<EnemyScript>().SetPlayer(playerObj);
            tempEnemy.GetComponent<EnemyScript>().SetValues(aiDirector.GetGeneratedStatistics(enemyCounter));
            
            enemyArray.Add(tempEnemy);
            enemyCounter++;
        }
        else
        {
            foreach (GameObject enemy in enemyArray)
            {
                enemy.GetComponent<EnemyScript>().Activate();
            }
            trainingState = TrainingEnum.FightEnemies;
        }
    }

    public void ClearRoom()
    {
        foreach (GameObject enemy in enemyArray)
        {
            if (!enemy.Equals(null)) 
            {
                enemy.GetComponent<EnemyScript>().KillEnemy();
            }
        } 
        enemyArray.Clear();

        // int roomsLeft = (5 + Mathf.FloorToInt(falseFloorCount * 0.5f)) - roomCount;
        // aiDirectorObj.GetComponent<AIDirectorScript>().IncReward(roomsLeft * 0.5f);

        aiDirectorObj.GetComponent<AIDirectorScript>().AddReward(0.25f);
        trainingState = TrainingEnum.EndEpisode;
    }

    private void ResetEnemyChosenQueue()
    {
        enemyChosenQueue.Clear();

        EnemyStatistics.EnemyEnum[] enemyEmptyArray = {EnemyStatistics.EnemyEnum.Empty, EnemyStatistics.EnemyEnum.Empty, EnemyStatistics.EnemyEnum.Empty};
        for (int i = 0; i < 3; i++)
        {
            enemyChosenQueue.Enqueue(enemyEmptyArray);
        }
    }
    
    private void UpdateEnemyChosenQueue()
    {
        enemyChosenQueue.Dequeue();

        EnemyStatistics.EnemyEnum[] tempArray = new EnemyStatistics.EnemyEnum[3];
        for (int enemyPos = 0; enemyPos < 3; enemyPos++)
        {
            tempArray[enemyPos] = enemyArray[enemyPos].GetComponent<EnemyScript>().GetEnemyType();
        }

        enemyChosenQueue.Enqueue(tempArray);
    }

    private float CalculateEnemyChosenReward()
    {
        float newReward = 0f;

        EnemyStatistics.EnemyEnum[][] tempArray = enemyChosenQueue.ToArray();

        // original calculation - iterate through all
        // for (int queuePos = 2; queuePos >= 0; queuePos--)
        // {
        //     if(tempArray[queuePos][0] != EnemyStatistics.EnemyEnum.Empty)
        //     {
        //         int enemyCount = 0;
        //         for (int arrayPos = 0; arrayPos < 3; arrayPos++)
        //         {
        //             if (tempArray[queuePos][arrayPos] != EnemyStatistics.EnemyEnum.Standard)
        //             {
        //                 for (int enemyPos = 0; enemyPos < 3; enemyPos++)
        //                 {
        //                     if (tempArray[queuePos][arrayPos] == enemyArray[enemyPos].GetComponent<EnemyScript>().GetEnemyType())
        //                     {
        //                         tempArray[queuePos][arrayPos] = EnemyStatistics.EnemyEnum.Empty;
        //                         enemyCount++;
        //                         enemyPos = 3; 
        //                     }
        //                 }
        //             }
        //         }
        //         if (enemyCount == 3) newReward += (enemyCount * 1f) * Mathf.Pow(10, (queuePos - 3));
        //         else newReward += (enemyCount * 0.75f) * Mathf.Pow(10, (queuePos - 3));
        //     }
        //     else queuePos = -1;
        // }

        // second calculation - add to array of vals
        // int[] tempCountArray = new int[6];
        // 
        // for (int queuePos = 0; queuePos < 3; queuePos++)
        // {
        //     for (int arrayPos = 0; arrayPos < 3; arrayPos++)
        //     {
        //         tempCountArray[(int)tempArray[queuePos][arrayPos]]++;
        //     }
        // }
        // 
        // for (int arrayPos = 1; arrayPos < 5; arrayPos++)
        // {
        //     if (tempCountArray[arrayPos] > 1) newReward += 0.05f * Mathf.Pow((tempCountArray[arrayPos] - 2), 0.69f);
        // }

        int[] scoreArray = new int[3];
        int currentScore = 0;
        
        for (int enemyPos = 0; enemyPos < 3; enemyPos++)
        {            
            switch (enemyArray[enemyPos].GetComponent<EnemyScript>().GetEnemyType())
            {
                default:
                case EnemyStatistics.EnemyEnum.Standard:
                currentScore += 1;
                break;
                case EnemyStatistics.EnemyEnum.Swarm:
                currentScore += 2;
                break;
                case EnemyStatistics.EnemyEnum.Tank:
                currentScore += 8;
                break;
                case EnemyStatistics.EnemyEnum.Turret:
                currentScore += 16;
                break;
                case EnemyStatistics.EnemyEnum.Sniper:
                currentScore += 64;
                break;
            }
        }
        // Debug.Log("Current Score: " + currentScore);

        for (int queuePos = 0; queuePos < 3; queuePos++)
        {
            for (int arrayPos = 0; arrayPos < 3; arrayPos++)
            {
                switch (tempArray[queuePos][arrayPos])
                {
                    default:
                    case EnemyStatistics.EnemyEnum.Empty:
                    scoreArray[queuePos] += 0;
                    break;
                    case EnemyStatistics.EnemyEnum.Standard:
                    scoreArray[queuePos] += 1;
                    break;
                    case EnemyStatistics.EnemyEnum.Swarm:
                    scoreArray[queuePos] += 2;
                    break;
                    case EnemyStatistics.EnemyEnum.Tank:
                    scoreArray[queuePos] += 8;
                    break;
                    case EnemyStatistics.EnemyEnum.Turret:
                    scoreArray[queuePos] += 16;
                    break;
                    case EnemyStatistics.EnemyEnum.Sniper:
                    scoreArray[queuePos] += 64;
                    break;
                }
            }
            if (scoreArray[queuePos] != 0 && scoreArray[queuePos] == currentScore) newReward += 0.05f;
        }

        return (newReward * -1f);
    }
}
