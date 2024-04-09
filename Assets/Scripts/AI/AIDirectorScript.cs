using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;


public class AIDirectorScript : Agent
{
    private GameObject playerObj;
    private PlayerScript playerScr;
    private RoomScript attachedRoom;
    private Vector2 enemyPos;
    private float cumulativeReward;
    private float[,] enemyValueArray;

    // Start is called before the first frame update
    void Start()
    {
        playerObj = null;
        attachedRoom = null;
        enemyValueArray = new float[5,3];
    }

    public override void OnEpisodeBegin()
    {
        cumulativeReward = 0;
    }

    // Old CollectObservations (14)
    // if (playerObj != null)
    //     {

    //         float playerHealth = playerScr.GetHealth();
    //         playerHealth = playerHealth / 50f;
    //         sensor.AddObservation(playerHealth);

    //         WeaponPlayerScript playerWeapon = playerScr.GetWeaponScript();
    //         if (playerWeapon != null) 
    //         {
    //             PointsStruct playerWeaponPoints = playerWeapon.GetPointsVal();
    //             int totalPoints = playerWeaponPoints.GetMaxPoints();

    //             for (int attribute = 0; attribute < 5; attribute++)
    //             {
    //                 float tempVal = playerWeaponPoints.GetAtPos(attribute) / totalPoints;
    //                 sensor.AddObservation(tempVal);
    //             }
    //         }
            
    //         if (attachedRoom != null) sensor.AddObservation((Vector2)((playerObj.transform.position - attachedRoom.transform.position) / 8f));
    //         else sensor.AddObservation((Vector2)(playerObj.transform.localPosition / 8f));
    //     }

    //     if (attachedRoom != null)
    //     {
    //         ArrayList enemies = attachedRoom.GetEnemyArray();
    //         if (enemies != null)
    //         {
    //             int enemyCount = 0;
    //             foreach (GameObject enemy in enemies)
    //             {
    //                 if (enemy != null) 
    //                 {
    //                     sensor.AddObservation((Vector2)(enemy.transform.localPosition / 8f));
    //                     enemyCount++;
    //                 }
    //             }
    //             for (int i = 0; i < (3 - enemyCount); i++)
    //             {
    //                 sensor.AddObservation(Vector2.zero);
    //             }
    //         }
    //     }

    public override void CollectObservations(VectorSensor sensor)
    {
        WeaponPlayerScript playerWeapon = playerScr.GetWeaponScript();
        if (playerWeapon != null) 
        {
            PointsStruct playerWeaponPoints = playerWeapon.GetPointsVal();
            int totalPoints = playerWeaponPoints.GetMaxPoints();

            for (int attribute = 0; attribute < 5; attribute++)
            {
                float tempVal = playerWeaponPoints.GetAtPos(attribute) / totalPoints;
                sensor.AddObservation(tempVal);
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActionsOut = actionsOut.ContinuousActions;
        //ActionSegment<int> discreteActionsOut = actionsOut.DiscreteActions;
        
        for (int i = 0; i < 12; i++)
        {
            continuousActionsOut[i] = Random.Range(0f, 1f);            
        }
        // for (int i = 0; i < 3; i++)
        // {
        //     discreteActionsOut[i] = Random.Range(0, 5);
        // }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float tempVal;
        for (int enemyPos = 0; enemyPos < 3; enemyPos++)
        {
            for (int atrrPos = 0; atrrPos < 4; atrrPos++)
            {
                tempVal = 0.5f * Mathf.Clamp(actions.ContinuousActions[atrrPos + (4 * enemyPos)], -1f, 1f);
                enemyValueArray[atrrPos, enemyPos] = tempVal + 0.5f;
            }
        }
        // for (int enemyPos = 0; enemyPos < 3; enemyPos++)
        // {
        //     enemyValueArray[4, enemyPos] = actions.DiscreteActions[enemyPos];            
        // }
    }

    public void IncReward(float rewardValue)
    {
        cumulativeReward += (rewardValue / 100f);
    }

    public void RewardAI()
    {
        SetReward(cumulativeReward);
        cumulativeReward = 0;
    }

    public EnemyStatistics GetGeneratedStatistics(int enemyPos, int floorCount = 0)
    {
        EnemyStatistics tempStatistics = new EnemyStatistics();
        
        float totalAttributeValue = 0;
        for (int atrrPos = 0; atrrPos < 4; atrrPos++)
        {
            totalAttributeValue += enemyValueArray[atrrPos, enemyPos];
        }

        int pointMax = tempStatistics.GetPointCount(floorCount);
        float pointMultiplier = pointMax / totalAttributeValue;

        for (int atrrPos = 0; atrrPos < 4; atrrPos++)
        {
            tempStatistics.LoadAtPos(atrrPos, Mathf.Clamp(Mathf.FloorToInt(enemyValueArray[atrrPos, enemyPos] * pointMultiplier), 0, pointMax));
        }
        //tempStatistics.LoadType((int)enemyValueArray[4, enemyPos]);
        tempStatistics.LoadType(-1);

        return tempStatistics;
    }

    public void SetPlayerObj(GameObject newPlayer)
    {
        playerObj = newPlayer;
        playerScr = playerObj.GetComponent<PlayerScript>();
    }

    public void SetAttachedRoom(RoomScript newRoom)
    {
        attachedRoom = newRoom;
    }
}
