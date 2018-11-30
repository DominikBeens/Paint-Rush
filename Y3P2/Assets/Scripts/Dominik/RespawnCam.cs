using UnityEngine;
using TMPro;

public class RespawnCam : MonoBehaviour
{

    private float respawnTime;

    private GameObject respawnCamObject;
    public GameObject RespawnCamObject { get { return respawnCamObject; } }

    [SerializeField] private float timeTillRespawn = 3f;
    [SerializeField] private TextMeshProUGUI timeTillRespawnText;

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
                if (Time.time > respawnTime)
                {
                    GameManager.CurrentGameSate = GameManager.GameState.Playing;
                }
            }

            timeTillRespawnText.text = Mathf.Clamp((respawnTime - Time.time), 0, timeTillRespawn).ToString("F2");
        }
    }

    private void GameManager_OnGameStateChanged(GameManager.GameState newState)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(newState == GameManager.GameState.Respawning ? true : false);
        }

        respawnTime = newState == GameManager.GameState.Respawning ? Time.time + timeTillRespawn : respawnTime;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }
}
