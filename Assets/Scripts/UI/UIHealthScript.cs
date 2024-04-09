using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealthScript : MonoBehaviour
{
    public GameObject[] heartObjArray;
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
        foreach (GameObject heart in heartObjArray)
        {
            heart.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        }
    }

    public void UpdateHealth(float newHealth)
    {
        int currentHeart = 0;
        int loopCount = 1;
        bool inLoop = true;

        Debug.Log("New Health: " + newHealth.ToString());
        
        foreach (GameObject heart in heartObjArray)
        {
            heart.GetComponent<Image>().sprite = iconSpriteArray[2];
            heart.GetComponent<Image>().color = new Color(1, 1, 1, 0f);
        }
        
        while (inLoop)
        {
            if (newHealth >= (loopCount * 10))
            {
                heartObjArray[currentHeart].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                loopCount++;
            }
            else if (newHealth % 10 > 5)
            {
                heartObjArray[currentHeart].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                inLoop = false;
            }
            else if (newHealth % 10 == 0)
            {
                inLoop = false;
            }
            else
            {
                heartObjArray[currentHeart].GetComponent<Image>().sprite = iconSpriteArray[1];
                heartObjArray[currentHeart].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                inLoop = false;
            }
            currentHeart++;
        }
    }
}
