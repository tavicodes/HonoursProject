using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorScript : MonoBehaviour
{
    public enum RoomChangeEnum
    {
        Exit,
        Enter,
        Ready,
        Play
    }
    public GameObject defaultRoomPrefab;
    private RoomChangeEnum roomState;
    private DungeonScript owningDungeon;
    private UIMapScript uiMap;
    private Transform playerPos;
    private GameObject[,] roomArray;
    private Vector2Int currentRoom;
    private Vector2Int itemRoom;
    private Vector2Int bossRoom;
    private int doorEntryNum;

    // Start is called before the first frame update
    void Start()
    {
        roomState = RoomChangeEnum.Play;
    }

    // Update is called once per frame
    void Update()
    {
        switch (roomState)
        {
            case RoomChangeEnum.Exit:
                // fade out to black
                if (true) ChangeRoom();
                break;
            case RoomChangeEnum.Enter:
                // fade back in
                if (true) FinishRoomChange();
                break;
            case RoomChangeEnum.Ready:
            default:
                break;
        }
    }

    public void SetDungeon(DungeonScript newDungeon)
    {
        owningDungeon = newDungeon;
    }

    public DungeonScript GetDungeon()
    {
        return owningDungeon;
    }

    public void SetUIMap(UIMapScript newUIMap)
    {
        uiMap = newUIMap;
    }

    public UIMapScript GetUIMap()
    {
        return uiMap;
    }

    public void SetPlayer(Transform newPlayer)
    {
        playerPos = newPlayer;
    }

    public void GenerateRoomArray(int floorCount)
    {
        // Create rooms
        int sideSize = 5 + Mathf.FloorToInt(floorCount * 0.5f);
        roomArray = new GameObject[sideSize, sideSize];
        for (int x = 0; x < sideSize; x++)
        {
            for (int y = 0; y < sideSize; y++)
            {
                DoorsStruct tempDoors = new DoorsStruct(x, y, sideSize - 1);
                roomArray[x, y] = Instantiate(defaultRoomPrefab, new Vector3(x * 19, y * 11, 0), Quaternion.identity);
                roomArray[x, y].transform.SetParent(transform);
                roomArray[x, y].GetComponent<RoomScript>().SetFloor(this);
                roomArray[x, y].GetComponent<RoomScript>().InitializeDoors(tempDoors);
            }
        }
    }

    public void SetRoomTypes(int floorCount)
    {
        // Set room values
        int sideSize = owningDungeon.GetFloorSideSize();
        currentRoom = new Vector2Int(Random.Range(0, sideSize - 1), Random.Range(0, sideSize - 1));        
        roomArray[currentRoom.x, currentRoom.y].GetComponent<RoomScript>().SetRoomType(RoomScript.RoomEnum.Start);
        roomArray[currentRoom.x, currentRoom.y].GetComponent<RoomScript>().SetRoomActive();

        playerPos.position = roomArray[currentRoom.x, currentRoom.y].transform.position;
        owningDungeon.playerCamera.GetComponent<CameraScript>().SetRoomTransform(roomArray[currentRoom.x, currentRoom.y].transform);
        owningDungeon.GetAIScript().SetAttachedRoom(roomArray[currentRoom.x, currentRoom.y].GetComponent<RoomScript>());
        Debug.Log("Spawn Room Pos: " + currentRoom.ToString());
        playerPos.gameObject.GetComponent<PlayerScript>().ToggleUpdate();

        do
        {
            itemRoom = new Vector2Int(Random.Range(0, sideSize - 1), Random.Range(0, sideSize - 1));
        } while (itemRoom == currentRoom);
        roomArray[itemRoom.x, itemRoom.y].GetComponent<RoomScript>().SetRoomType(RoomScript.RoomEnum.Item);
        
        do
        {
            bossRoom = new Vector2Int(Random.Range(0, sideSize - 1), Random.Range(0, sideSize - 1));
        } while ((bossRoom == itemRoom) || (Mathf.Abs(bossRoom.x - currentRoom.x) + Mathf.Abs(bossRoom.y - currentRoom.y)) < (3 + Mathf.FloorToInt(floorCount * 0.5f)));
        roomArray[bossRoom.x, bossRoom.y].GetComponent<RoomScript>().SetRoomType(RoomScript.RoomEnum.Boss);

        foreach (var room in roomArray)
        {
            RoomScript tempRoom = room.GetComponent<RoomScript>();
            if (tempRoom.GetRoomType() == RoomScript.RoomEnum.Empty) tempRoom.SetRoomType(RoomScript.RoomEnum.Enemies);
        }

        uiMap.UpdateRooms(this);
    }

    public GameObject GetRoomAtAdjacentPos(int x = 0, int y = 0)
    {
        int xPos = currentRoom.x + x;
        int yPos = currentRoom.y + y;
        if (xPos < 0 || xPos == owningDungeon.GetFloorSideSize() || yPos < 0 || yPos == owningDungeon.GetFloorSideSize()) return null;
        return roomArray[currentRoom.x + x, currentRoom.y + y];
    }

    public void StartRoomChange(int doorNum)
    {
        doorEntryNum = doorNum;
        switch (doorEntryNum)
        {
            case 0:
                // From the bottom
                currentRoom.y += 1;
                playerPos.position = roomArray[currentRoom.x, currentRoom.y].transform.position + new Vector3(0f, -4f, 1f);
            break;
            case 1:
                // From the right
                currentRoom.x += 1;
                playerPos.position = roomArray[currentRoom.x, currentRoom.y].transform.position + new Vector3(-8f, 0f, 1f);
            break;
            case 2:
                // From the top
                currentRoom.y -= 1;
                playerPos.position = roomArray[currentRoom.x, currentRoom.y].transform.position + new Vector3(0f, 4f, 1f);
            break;
            case 3:
                // From the left
                currentRoom.x -= 1;
                playerPos.position = roomArray[currentRoom.x, currentRoom.y].transform.position + new Vector3(8f, 0f, 1f);
            break;
            case 4:
                // boss beat
                owningDungeon.IncFloor();
            break;
            default:
                break;
        }

        playerPos.gameObject.GetComponent<PlayerScript>().ToggleUpdate();
        roomState = RoomChangeEnum.Exit;
    }

    public void ChangeRoom()
    {
        RoomScript tempRoom = roomArray[currentRoom.x, currentRoom.y].GetComponent<RoomScript>();

        if (!tempRoom.gameObject.activeSelf) tempRoom.SetRoomActive();

        owningDungeon.playerCamera.GetComponent<CameraScript>().SetRoomTransform(tempRoom.transform);
        owningDungeon.GetAIScript().SetAttachedRoom(tempRoom);

        uiMap.UpdateRooms(this);

        roomState = RoomChangeEnum.Enter;
    }

    public void FinishRoomChange()
    {
        RoomScript tempRoom = roomArray[currentRoom.x, currentRoom.y].GetComponent<RoomScript>();
        if (tempRoom.GetRoomReady())
        {
            tempRoom.ActivateRoomContents(uiMap);
            playerPos.gameObject.GetComponent<PlayerScript>().ToggleUpdate();
            owningDungeon.SetUIWeaponVisibility(tempRoom.GetRoomType() == RoomScript.RoomEnum.Item);
            
            roomState = RoomChangeEnum.Ready;
        }
    }

    public int GetDoorEntryNum()
    {
        return doorEntryNum;
    }
}
