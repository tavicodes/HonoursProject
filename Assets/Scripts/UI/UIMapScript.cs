using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMapScript : MonoBehaviour
{
    public GameObject[] mapObjArray;
    public Sprite[] iconSpriteArray;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetColor()
    {
        foreach (GameObject room in mapObjArray)
        {
            room.GetComponent<Image>().sprite = iconSpriteArray[0];
            room.GetComponent<Image>().color = new Color(1, 1, 1, 0.45f);
        }
    }

    public void UpdateRooms(FloorScript floor)
    {
        ResetColor();

        int mapArrayPos = 0;
        for (int yPos = -1; yPos < 2; yPos++)
        {
            for (int xPos = -1; xPos < 2; xPos++)
            {
                GameObject room = floor.GetRoomAtAdjacentPos(xPos, yPos);
                
                if (room == null) mapObjArray[mapArrayPos].GetComponent<Image>().color = new Color(0.35f, 0.35f, 0.35f, 0.45f);
                else 
                {
                    if (!room.activeSelf) mapObjArray[mapArrayPos].GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 0.45f);

                    switch (room.GetComponent<RoomScript>().GetRoomType())
                    {
                        case RoomScript.RoomEnum.Item:
                            mapObjArray[mapArrayPos].GetComponent<Image>().sprite = iconSpriteArray[1];
                            break;
                        case RoomScript.RoomEnum.Boss:
                            mapObjArray[mapArrayPos].GetComponent<Image>().sprite = iconSpriteArray[2];
                            break;
                        default:
                            mapObjArray[mapArrayPos].GetComponent<Image>().sprite = iconSpriteArray[0];
                            break;
                    }
                }
                mapArrayPos++;
            }
        }
        floor.GetRoomAtAdjacentPos();
    }

    public void SetVisibility(bool newValue)
    {
        foreach (GameObject room in mapObjArray)
        {
            room.SetActive(newValue);
        }
    }
}
