using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DoorsStruct
{
    private readonly bool top;
    public bool topDoor { get { return top; } }
    private readonly bool right;
    public bool rightDoor { get { return right; } }
    private readonly bool bottom;
    public bool bottomDoor { get { return bottom; } }
    private readonly bool left;
    public bool leftDoor { get { return left; } }

    public DoorsStruct(int x, int y, int maxVal)
    {
        top = right = bottom = left = true;
        if (x == 0) left = false;
        if (x == maxVal) right = false;
        if (y == 0) bottom = false;
        if (y == maxVal) top = false;
    }
}

public class RoomScript : MonoBehaviour
{
    public enum RoomEnum
    {
        Empty,
        Start,
        Item,
        Enemies,
        Boss
    }
    public enum StateEnum
    {
        Inactive,
        Active,
        Spawning,
        Play,
        Clear
    }
    public GameObject itemPrefab;
    public GameObject enemyPrefab;
    public Transform floorObject;
    public GameObject[] doorObjects;
    public Transform[] itemSpawns;
    public Transform[] enemySpawns;
    private FloorScript owningFloor;
    private RoomEnum roomType;
    protected StateEnum currentState;
    protected List<GameObject> enemyArray;
    private float doorCooldown;

    // Start is called before the first frame update
    virtual public void Start()
    {
        doorCooldown = 0;
    }

    // Update is called once per frame
    virtual public void Update()
    {
        switch (currentState)
        {
            default:
            case StateEnum.Inactive:
                break;
            case StateEnum.Active:
                if (roomType == RoomEnum.Enemies || roomType == RoomEnum.Boss) 
                {  
                    owningFloor.GetDungeon().GetAIScript().RequestDecision();
                }
                currentState = StateEnum.Spawning;
                break;
            case StateEnum.Spawning:
                int doorEntryNum = owningFloor.GetDoorEntryNum(); 
                int floorCount = owningFloor.GetDungeon().GetFloorCount();
                GameObject playerObj = owningFloor.GetDungeon().GetPlayerScript().gameObject;
                
                switch (roomType)
                {
                    case RoomEnum.Empty:
                        currentState = StateEnum.Play;
                        break;
                    case RoomEnum.Start:
                        if (floorCount == 0) SetSpawn();
                        currentState = StateEnum.Play;
                        break;
                    case RoomEnum.Item:
                        SetItems(floorCount);
                        currentState = StateEnum.Play;
                        break;
                    case RoomEnum.Enemies:
                        SetEnemies(doorEntryNum, floorCount, playerObj);
                        break;
                    case RoomEnum.Boss:
                        SetBoss(doorEntryNum, floorCount, playerObj);
                        break;
                    default:
                        break;
                }

                break;
            case StateEnum.Play:
                if (roomType == RoomEnum.Enemies || roomType == RoomEnum.Boss) 
                {
                    foreach (var enemy in enemyArray)
                    {
                        if (enemy.Equals(null)) 
                        {
                            enemyArray.Remove(enemy);
                            break;
                        }
                    }
                    if (enemyArray.Count == 0) SetRoomCleared(true);
                }
                else SetRoomCleared(true);
                break;
            case StateEnum.Clear:
                if(doorCooldown != -1f) doorCooldown = Mathf.Clamp(doorCooldown - Time.deltaTime, 0f, 1f);
                break;
        }
    }

    public void SetFloor(FloorScript newFloor)
    {
        owningFloor = newFloor;

        foreach (GameObject door in doorObjects)
        {
            door.GetComponent<SpriteRenderer>().color = new Color32(0x4B, 0x36, 0x2B, 0xFF);
        }

        gameObject.SetActive(false);
        currentState = StateEnum.Inactive;
        roomType = RoomEnum.Empty;
        enemyArray = new List<GameObject>();
    }

    public void InitializeDoors(DoorsStruct doorValues)
    {
        doorObjects[0].SetActive(doorValues.topDoor);
        doorObjects[1].SetActive(doorValues.rightDoor);
        doorObjects[2].SetActive(doorValues.bottomDoor);
        doorObjects[3].SetActive(doorValues.leftDoor);
        doorObjects[4].SetActive(false);
    }

    public void SetRoomType(RoomEnum newType)
    {
        roomType = newType;
        switch (roomType)
        {
            default:
            case RoomEnum.Empty:
                SetRoomActive();
                gameObject.name = "Empty";
                break;
            case RoomEnum.Start:
                gameObject.name = "Start";
                break;
            case RoomEnum.Item:
                gameObject.name = "Item";
                break;
            case RoomEnum.Enemies:
                gameObject.name = "Enemy";
                break;
            case RoomEnum.Boss:
                gameObject.name = "Boss";
                break;
        }
    }

    public RoomEnum GetRoomType()
    {
        return roomType;
    }

    public void SetRoomActive()
    {
        gameObject.SetActive(true);
        currentState = StateEnum.Active;
    }

    virtual protected void SetRoomCleared(bool newValue)
    {
        if (newValue)
        {
            doorCooldown = 1f;
            gameObject.name += " (Cleared)";
            foreach (GameObject door in doorObjects)
            {
                door.GetComponent<SpriteRenderer>().color = new Color32(0x34, 0x41, 0x43, 0xFF);
            }
            if (roomType == RoomEnum.Boss) 
            {
                doorObjects[4].SetActive(true);
                doorObjects[4].GetComponent<SpriteRenderer>().color = new Color32(0x4B, 0x36, 0x2B, 0xFF);
            }

            if (gameObject.activeSelf)
            {
                owningFloor.GetUIMap().SetVisibility(newValue);
                if (roomType == RoomEnum.Enemies) owningFloor.GetDungeon().GetPlayerScript().ReduceItemCooldown(false);   
                if (roomType == RoomEnum.Boss) owningFloor.GetDungeon().GetPlayerScript().ReduceItemCooldown(true);   
            }

            currentState = StateEnum.Clear;
        }
    }

    private void SetSpawn()
    {
        GameObject tempWeapon = Instantiate(itemPrefab, (itemSpawns[0].position + new Vector3(0, 1.5f, 0f)), itemSpawns[0].rotation);
        tempWeapon.transform.SetParent(transform);
        tempWeapon.GetComponent<WeaponPlayerScript>().Regenerate(1);
    }

    protected int GenerateItemRarity(int floorCount)
    {
        int minWeaponRarity = 1;
        int maxWeaponRarity = 4;
        switch (floorCount)
        {
            case 0:
                minWeaponRarity = 1;
                maxWeaponRarity = 2;
                break;
            case 1:
                minWeaponRarity = 1;
                maxWeaponRarity = 3;
                break;
            case 2:
                minWeaponRarity = 1;
                maxWeaponRarity = 4;
                break;
            case 3:
                minWeaponRarity = 2;
                maxWeaponRarity = 4;
                break;
            case 4:
                minWeaponRarity = 3;
                maxWeaponRarity = 4;
                break;
            default:
            break;
        }
        return Random.Range(minWeaponRarity,maxWeaponRarity);
    }

    private void SetItems(int floorCount)
    {
        foreach (var spawn in itemSpawns)
        {
            GameObject tempWeapon = Instantiate(itemPrefab, spawn);
            tempWeapon.transform.SetParent(transform);
            tempWeapon.GetComponent<WeaponPlayerScript>().Regenerate(GenerateItemRarity(floorCount));
        }
    }

    virtual protected void SetEnemies(int doorEntry, int floorCount, GameObject playerObj)
    {
        SetRoomCleared(false);

        AIDirectorScript aiDirector = owningFloor.GetDungeon().GetAIScript();

        Transform[] calculatedSpawns = CalculateEnemySpawns(doorEntry);

        for (int spawnPos = 0; spawnPos < 3; spawnPos++)
        {
            GameObject tempEnemy = Instantiate(enemyPrefab, calculatedSpawns[spawnPos]);
            tempEnemy.transform.SetParent(transform);
            tempEnemy.GetComponent<EnemyScript>().SetPlayer(playerObj);
            
            EnemyStatistics tempStats = aiDirector.GetGeneratedStatistics(spawnPos, floorCount);
            tempEnemy.GetComponent<EnemyScript>().SetValues(tempStats);
            
            enemyArray.Add(tempEnemy);
        }
        currentState = StateEnum.Play;
    }

    private void SetBoss(int doorEntry, int floorCount, GameObject playerObj)
    {
        SetRoomCleared(false);

        GameObject tempBoss = Instantiate(enemyPrefab, enemySpawns[0]);
        tempBoss.transform.SetParent(transform);

        AIDirectorScript aiDirector = owningFloor.GetDungeon().GetAIScript();
        tempBoss.GetComponent<EnemyScript>().SetValues(aiDirector.GetGeneratedStatistics(0, floorCount), true);
        
        tempBoss.GetComponent<EnemyScript>().SetPlayer(playerObj);
        enemyArray.Add(tempBoss);

        Transform[] calculatedSpawns = CalculateEnemySpawns(doorEntry, true);

        for (int spawnPos = 0; spawnPos < 2; spawnPos++)
        {
            for (int valuePos = 0; valuePos < 2; valuePos++)
            {
                GameObject tempEnemy = Instantiate(enemyPrefab, calculatedSpawns[(spawnPos * 2 + valuePos)]);
                tempEnemy.transform.SetParent(transform);
                tempEnemy.GetComponent<EnemyScript>().SetPlayer(playerObj);
                
                EnemyStatistics tempStats = aiDirector.GetGeneratedStatistics((valuePos + 1), floorCount);
                tempEnemy.GetComponent<EnemyScript>().SetValues(tempStats);
                
                enemyArray.Add(tempEnemy);
            }
        }
        currentState = StateEnum.Play;
    }

    public void ActivateRoomContents(UIMapScript uiMap)
    {
        if (currentState == StateEnum.Clear) uiMap.SetVisibility(true);
        else uiMap.SetVisibility(false);

        if (enemyArray.Count > 0) ActivateEnemies();
        EnterDoorCooldown();
    }

    private Transform[] CalculateEnemySpawns(int doorEntryNum, bool isBoss = false)
    {
        Transform[] tempTransform;

        if (isBoss)
        {
            tempTransform = new Transform[4];
            switch (doorEntryNum)
            {
                default:
                case 0:
                    // From the bottom
                    tempTransform[0] = enemySpawns[1];
                    tempTransform[1] = enemySpawns[2];
                    tempTransform[2] = enemySpawns[3];
                    tempTransform[3] = enemySpawns[4];
                break;
                case 1:
                    // From the left
                    tempTransform[0] = enemySpawns[2];
                    tempTransform[1] = enemySpawns[3];
                    tempTransform[2] = enemySpawns[4];
                    tempTransform[3] = enemySpawns[1];
                break;
                case 2:
                    // From the top
                    tempTransform[0] = enemySpawns[3];
                    tempTransform[1] = enemySpawns[4];
                    tempTransform[2] = enemySpawns[1];
                    tempTransform[3] = enemySpawns[2];
                break;
                case 3:
                    // From the right
                    tempTransform[0] = enemySpawns[4];
                    tempTransform[1] = enemySpawns[1];
                    tempTransform[2] = enemySpawns[2];
                    tempTransform[3] = enemySpawns[3];
                break;
            }
        }
        else
        {
            tempTransform = new Transform[3];
            tempTransform[0] = enemySpawns[0];
            switch (doorEntryNum)
            {
                default:
                case 0:
                    // From the bottom
                    tempTransform[1] = enemySpawns[1];
                    tempTransform[2] = enemySpawns[2];
                break;
                case 1:
                    // From the left
                    tempTransform[1] = enemySpawns[2];
                    tempTransform[2] = enemySpawns[3];
                break;
                case 2:
                    // From the top
                    tempTransform[1] = enemySpawns[3];
                    tempTransform[2] = enemySpawns[4];
                break;
                case 3:
                    // From the right
                    tempTransform[1] = enemySpawns[1];
                    tempTransform[2] = enemySpawns[4];
                break;
            }
        }
        return tempTransform;
    }

    public void ActivateEnemies()
    {
        foreach (GameObject enemy in enemyArray)
        {
            enemy.GetComponent<EnemyScript>().Activate();
        }
    }

    public void AddToEnemyArray(GameObject newEnemy)
    {
        enemyArray.Add(newEnemy);
    }
    public List<GameObject> GetEnemyArray()
    {
        if (currentState != StateEnum.Play) return null;
        return enemyArray;
    }

    public bool GetRoomReady()
    {
        if (currentState == StateEnum.Play || currentState == StateEnum.Clear) return true;
        return false;
    }

    public void EnterDoorCooldown()
    {
        doorCooldown = -1f;
    }

    public Transform GetRoomTransform()
    {
        return floorObject;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerScript tempPlayer = other.GetComponent<PlayerScript>();
            for (int i = 0; i < doorObjects.Length; i++)
            {
                if (currentState == StateEnum.Inactive && doorObjects[i].GetComponent<Collider2D>().IsTouching(other))
                {
                    // Change room
                    //Debug.Log("Door hit: " + i.ToString());
                    return;
                }
            }
        }
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerScript tempPlayer = other.GetComponent<PlayerScript>();
            for (int i = 0; i < doorObjects.Length; i++)
            {
                if ((currentState == StateEnum.Clear && doorCooldown == 0f) && (doorObjects[i].GetComponent<Collider2D>().IsTouching(other)))
                {
                    // Change room
                    if (owningFloor != null) owningFloor.StartRoomChange(i);
                    return;
                }
            }
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player") doorCooldown = 0.5f;
    }
}
