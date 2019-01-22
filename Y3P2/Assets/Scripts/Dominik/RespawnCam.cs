using UnityEngine;
using TMPro;

public class RespawnCam : MonoBehaviour
{

    private float respawnTime;

    private GameObject respawnCamObject;
    public GameObject RespawnCamObject { get { return respawnCamObject; } }

    [SerializeField] private float timeTillRespawn = 3f;
    [SerializeField] private TextMeshProUGUI respawnText;
    [SerializeField] private TextMeshProUGUI timeTillRespawnText;

    private void Awake()
    {
        respawnCamObject = GetComponentInChildren<Camera>(true).gameObject;
        respawnCamObject.SetActive(false);

        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
        DB.MenuPack.SceneManager.OnGamePaused += SceneManager_OnGamePaused;
    }

    private void Update()
    {
        if (GameManager.CurrentGameSate == GameManager.GameState.Respawning)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (Time.time > respawnTime)
                {
                    GameManager.CurrentGameSate = GameManager.GameState.Playing;
                }
            }

            respawnText.text = Time.time > respawnTime ? "Press <color=#00FFFF>SPACE</color> to respawn!" : "Press <color=#00FFFF>SPACE</color> to respawn in";
            timeTillRespawnText.text = Time.time > respawnTime ? "" : Mathf.Clamp((respawnTime - Time.time), 0, timeTillRespawn).ToString("F2");
        }
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Respawning)
        {
            ToggleChildren(true);
        }
        else
        {
            ToggleChildren(false);
        }

        respawnTime = newState == GameManager.GameState.Respawning ? Time.time + timeTillRespawn : respawnTime;
    }

    private void SceneManager_OnGamePaused(bool toggle)
    {
        if (GameManager.CurrentGameSate == GameManager.GameState.Respawning)
        {
            ToggleChildren(!toggle);
            respawnCamObject.SetActive(true);
        }
    }

    private void ToggleChildren(bool toggle)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(toggle);
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
        DB.MenuPack.SceneManager.OnGamePaused -= SceneManager_OnGamePaused;
    }
}
