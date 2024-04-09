using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemScript : MonoBehaviour
{
    public Sprite[] itemSpriteArray;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(int fillLevel)
    {
        gameObject.GetComponent<Image>().sprite = itemSpriteArray[fillLevel];
    }
}
