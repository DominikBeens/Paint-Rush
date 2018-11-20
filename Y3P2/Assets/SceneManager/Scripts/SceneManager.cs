using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DB.MenuPack
{
    public class SceneManager : MonoBehaviour
    {

        public static SceneManager instance;

        public static bool transitionComplete;
        public static bool isLoadingLevel;
        public static bool canPause = true;
        public static bool isPaused;

        private CursorLockMode prePauseCursorLockMode;
        private bool prePauseCursorVisibility;

        [Header("Scene Transitions")]
        [SerializeField] private Animator transitionAnim;
        [SerializeField] private bool pickRandomTransition;
        [SerializeField] private List<RuntimeAnimatorController> transitionAnimators = new List<RuntimeAnimatorController>();

        [Space(10)]

        [SerializeField] private float tSpeedIn = 1f;
        [SerializeField] private float tSpeedOut = 1f;

        [Header("Loading Bar")]
        [SerializeField] private bool showLoadingBar = true;
        [SerializeField] private GameObject loadingBar;
        [SerializeField] private Image loadingBarFill;

        [Header("Pause Game")]
        [SerializeField] private string pauseInputButton = "Cancel";
        [SerializeField] private bool canPauseWithSpecifiedButton = true;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private List<int> cannotPauseScenes = new List<int>();

        public event Action OnLevelLoaded = delegate { };

        private void Awake()
        {
            // Classic singleton with a DontDestroyOnLoad.
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            // If canPauseWithSpecifiedButton is set to false or the specified input button does not exist theres no reason to keep running the rest of the Update code.
            if (!canPauseWithSpecifiedButton || !DoesInputButtonExist(pauseInputButton))
            {
                return;
            }

            if (Input.GetButtonDown(pauseInputButton))
            {
                // If canPauseWithSpecifiedButton is set to true and the player pressed Escape, pause the game.
                if (canPauseWithSpecifiedButton)
                {
                    if (!cannotPauseScenes.Contains(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex))
                    {
                        PauseGame();
                    }
                }
            }
        }

        public void PauseGame()
        {
            // You cant pause the game if a level is loading or if canPause is set to false.
            if (isLoadingLevel || !canPause)
            {
                return;
            }

            isPaused = !isPaused;

            pausePanel.SetActive(isPaused);
            SettingsManager.instance.ToggleSettingsPanel(false);

            // Saving cursor mode pre pause.
            if (isPaused)
            {
                prePauseCursorLockMode = Cursor.lockState;
                prePauseCursorVisibility = Cursor.visible;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            // Applying saved cursor mode after unpause.
            else
            {
                Cursor.lockState = prePauseCursorLockMode;
                Cursor.visible = prePauseCursorVisibility;
            }
        }

        public void LoadScene(int index, bool stopTimeWhenLoading)
        {
            // Prevents the level from loading if theres already another level loading or if the game is paused. 
            if (isLoadingLevel || pausePanel.activeInHierarchy)
            {
                return;
            }

            StartCoroutine(LoadSceneAsync(index, stopTimeWhenLoading));
        }

        public void LoadSceneInstant(int index)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(index);
        }

        private IEnumerator LoadSceneAsync(int index, bool stopTimeWhenLoading)
        {
            // Bool prevents the game from loading a level twice or multiple levels at the same time.
            isLoadingLevel = true;
            // Stop the time?
            Time.timeScale = stopTimeWhenLoading ? 0 : Time.timeScale;

            // Picks a random transition from the list of transitions.
            if (pickRandomTransition)
            {
                RuntimeAnimatorController randomTransitionAnim = transitionAnimators[UnityEngine.Random.Range(0, transitionAnimators.Count)];
                transitionAnim.runtimeAnimatorController = randomTransitionAnim;
            }

            // Set the transition speed and display a transition animation.
            transitionAnim.speed = tSpeedIn;
            transitionAnim.Play("Transition In");

            // Wait for the transition to complete.
            while (!transitionComplete)
            {
                yield return null;
            }
            transitionComplete = false;

            // Display the loading bar is showLoadingBar is set to true.
            if (showLoadingBar)
            {
                loadingBar.SetActive(true);
            }

            // Load the level asynchronously and output the progress to the loading bar.
            AsyncOperation loadScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(index);
            while (!loadScene.isDone)
            {
                loadingBarFill.fillAmount = (loadScene.progress < 0.9f) ? loadScene.progress : 1;
                yield return null;
            }

            // Deactivate the loading bar.
            loadingBar.SetActive(false);

            // Set the transition speed and display another transition animation.
            transitionAnim.speed = tSpeedOut;
            transitionAnim.Play("Transition Out");

            // Wait for the transition to complete.
            while (!transitionComplete)
            {
                yield return null;
            }
            transitionComplete = false;

            // If the time was stopped, set it to one. If it wasn't stopped leave it as it is.
            Time.timeScale = stopTimeWhenLoading ? 1 : Time.timeScale;
            // Level has finished loading, another level can start loading if necessary.
            isLoadingLevel = false;
            // Fires an OnLevelLoaded event so that all subscribed scripts can start doing their thing.
            OnLevelLoaded();
        }

        private bool DoesInputButtonExist(string buttonName)
        {
            try
            {
                Input.GetButton(buttonName);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public void ReturnToMainMenu()
        {
            PauseGame();
            GameManager.instance.LeaveRoom();
        }

        public void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
