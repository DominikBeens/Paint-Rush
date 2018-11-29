using UnityEngine;

public class RespawnCam : MonoBehaviour
{

    private GameObject respawnCamObject;
    public GameObject RespawnCamObject { get { return respawnCamObject; } }

    private void Awake()
    {
        respawnCamObject = GetComponentInChildren<Camera>().gameObject;
        respawnCamObject.SetActive(false);

        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        if (GameManager.CurrentGameSate == GameManager.GameState.Respawning)
        {
            if (Input.GetButtonDown("Jump"))
            {
                GameManager.CurrentGameSate = GameManager.GameState.Playing;
            }
        }
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newState == GameManager.GameState.Respawning ? true : false);
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
