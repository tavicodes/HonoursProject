using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject[] weaponArray;
    private float updateTimer = 5f;
    // Start is called before the first frame update
    void Start()
    {
        UpdateWeapons(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTimer > 0f) updateTimer -= Time.deltaTime;
        else
        {
            updateTimer = 7f;
            UpdateWeapons(false);
        } 
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameplayScene");
    }

    public void GoToOptions()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void UpdateWeapons(bool all)
    {
        foreach (GameObject weapon in weaponArray)
        {
            if (all || (Random.Range(0, 100) % 2 == 0)) weapon.GetComponent<WeaponPlayerScript>().Regenerate(Random.Range(1,4));
        }
    }
}
