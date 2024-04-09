using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponScript : MonoBehaviour
{
    public GameObject[] weaponAttrObjArray;
    public GameObject[] pointsObjArray;
    public Sprite[] iconSpriteArray;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject point in pointsObjArray)
        {
            point.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetColor()
    {
        foreach (GameObject point in pointsObjArray)
        {
            point.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void SetPoints(PointsStruct pointVals)
    {
        int currentPointObj = 0;
        int loopCount;
        bool inLoop;
        ResetColor();
        for (int i = 0; i < weaponAttrObjArray.Length; i++)
        {
            loopCount = 1;
            inLoop = true;
            while (inLoop)
            {
                float coordVal = ((loopCount * (-50f)) - 20f);
                pointsObjArray[currentPointObj].transform.position = new Vector3(
                    weaponAttrObjArray[i].transform.position.x + coordVal, 
                    weaponAttrObjArray[i].transform.position.y);

                if (pointVals.GetAtPos(i) >= (loopCount * 4))
                {
                    pointsObjArray[currentPointObj].GetComponent<Image>().sprite = iconSpriteArray[4];
                    loopCount++;
                }
                else
                {
                    pointsObjArray[currentPointObj].GetComponent<Image>().sprite = iconSpriteArray[(pointVals.GetAtPos(i) % 4)];
                    if (pointsObjArray[currentPointObj].GetComponent<Image>().sprite == null) pointsObjArray[currentPointObj].GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    inLoop = false;
                }
                currentPointObj++;
            }
        }
        for (int point = currentPointObj; point < pointsObjArray.Length; point++)
        {
            pointsObjArray[point].GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }
}
