using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int targetFps = 60;


    // References

    UIManager uiManager;
    PlayerCombat playerCombat;
    BuildingManager buildingManager;

    GameObject playerGO;
    public Transform respawnPoint;
    void Awake()
    {
        Application.targetFrameRate = targetFps;

        uiManager = GameObject.Find("/Managers/UI Manager").GetComponent<UIManager>();
        buildingManager = GameObject.Find("/Managers/Building Manager").GetComponent<BuildingManager>();
        playerCombat = GameObject.FindWithTag("Player").GetComponent<PlayerCombat>();
        playerGO = GameObject.FindWithTag("Player");
    }

    void Start()
    {
        ResetPlayerPosition();
    }
    
    public void ResetPlayerPosition()
    {
        playerGO.transform.position = respawnPoint.position;
        playerGO.transform.localEulerAngles = new Vector3(0,0,0);
    }

    public void GameOverState ()
    {
        Debug.Log("Game Over");

        // Disable all other UI
        uiManager.TogglePauseMenu(false);
        buildingManager.ToggleBuildingMenu(false);

        // Call UI
        uiManager.GameOverMenu(true);
    }

    public void ExitGame ()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false; // Unity Editor Only
    }
}
