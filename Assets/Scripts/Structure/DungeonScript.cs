using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonScript : MonoBehaviour
{
    private enum GameStateEnum
    {
        Spawning,
        Loading,
        Ready
    }
    public GameObject playerPrefab;
    public GameObject floorPrefab;
    public GameObject aiPrefab;
    public GameObject playerCamera;
    public GameObject mapUI;
    public GameObject healthUI;
    public GameObject itemUI;
    public GameObject weaponUI;
    public GameObject pauseOverlay;
    public GameObject victoryOverlay;
    public GameObject defeatOverlay;
    private GameStateEnum gameState;
    private GameObject playerObj;
    private GameObject aiObj;
    private GameObject currentFloor;
    private int floorCount;
    private bool pauseState;

    // Start is called before the first frame update
    void Start()
    {
        floorCount = 0;
        pauseState = false;

        playerObj = Instantiate(playerPrefab, transform);
        playerObj.GetComponent<PlayerScript>().SetOwningDungeon(this);
        playerObj.GetComponent<PlayerScript>().SetUIHealth(healthUI.GetComponent<UIHealthScript>());
        playerObj.GetComponent<PlayerScript>().SetUIItem(itemUI.GetComponent<UIItemScript>());
        playerObj.GetComponent<PlayerScript>().SetUIWeapon(weaponUI.GetComponent<UIWeaponScript>());
        playerCamera.GetComponent<CameraScript>().SetPlayerTransform(playerObj.transform);

        aiObj = Instantiate(aiPrefab, transform);
        aiObj.GetComponent<AIDirectorScript>().SetPlayerObj(playerObj);

        gameState = GameStateEnum.Spawning;
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameStateEnum.Spawning:
                if (playerObj != null) CreateFloor();
                break;
            case GameStateEnum.Loading:
                FloorScript tempFloor = currentFloor.GetComponent<FloorScript>();
                tempFloor.SetRoomTypes(floorCount);
                gameState = GameStateEnum.Ready;
                break;
            case GameStateEnum.Ready:
            default:
                break;
        }
    }

    public void IncFloor()
    {
        floorCount++;
        Destroy(currentFloor);
        gameState = GameStateEnum.Spawning;
        if (floorCount == 5) ShowOverlay(true);
        
        Debug.Log("Reached floor: " + floorCount.ToString());
    }
    
    public int GetFloorCount()
    {
        return floorCount;
    }

    public int GetFloorSideSize()
    {
        return 5 + Mathf.FloorToInt(floorCount * 0.5f);
    }

    private void CreateFloor()
    {
        currentFloor = Instantiate(floorPrefab, transform);

        FloorScript tempFloor = currentFloor.GetComponent<FloorScript>();
        tempFloor.SetDungeon(this);
        tempFloor.SetPlayer(playerObj.transform);
        tempFloor.SetUIMap(mapUI.GetComponent<UIMapScript>());
        tempFloor.GenerateRoomArray(floorCount);

        gameState = GameStateEnum.Loading;
    }

    public void TogglePause()
    {
        pauseState = !pauseState;
        pauseOverlay.SetActive(pauseState);
        if (pauseState) Time.timeScale = 0;
        else Time.timeScale = 1;
    }

    public void ShowOverlay(bool victory)
    {
        pauseState = true;
        if (victory) victoryOverlay.SetActive(true);
        else defeatOverlay.SetActive(true);
    }

    public void ChangeScene(bool quit)
    {
        Time.timeScale = 1;
        if (quit) SceneManager.LoadScene("MainMenuScene");
        else SceneManager.LoadScene("GameplayScene");
    }

    public bool GetPauseState()
    {
        return pauseState;
    }

    public AIDirectorScript GetAIScript()
    {
        return aiObj.GetComponent<AIDirectorScript>();
    }

    public PlayerScript GetPlayerScript()
    {
        return playerObj.GetComponent<PlayerScript>();
    }

    public void SetUIWeaponVisibility(bool visible)
    {
        weaponUI.SetActive(visible);
    }
}
