using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardManager;
    public int foodPoints;

    [HideInInspector] public bool playersTurn = true;

    private Text levelText;
    private GameObject levelImage;
    private GameObject pauseMenu;

    private Button pauseButton;
    private Button resumeButon;
    private Button quitButton;
    private Button restartGameButton;
    private Button gameOverRestartGameButton;
    private Button gameOverQuitGameButton;

    private int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;
    private bool isPaused = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardManager = GetComponent<BoardManager>();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        InitGame();

        if(SoundManager.instance != null)
        {
            if (!SoundManager.instance.musicSource.isPlaying)
                SoundManager.instance.musicSource.Play();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void InitGame()
    {
        if (SceneManager.GetActiveScene().name != "Main")
            return;

        doingSetup = true;
        GetReferenceToGameObjects();

        //Hide the level image after 2 seconds
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardManager.SetupScene(level);

        //This is used to account for restarting the game
        if(level == 1)
        {
            foodPoints = 100;
        }
    }

    /// <summary>
    /// Get references to UI objects
    /// </summary>
    public void GetReferenceToGameObjects()
    {
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);

        pauseMenu = GameObject.Find("PauseMenu");

        pauseButton = GameObject.Find("btnPause").GetComponent<Button>();
        pauseButton.onClick.AddListener(PauseGame);

        resumeButon = GameObject.Find("btnResume").GetComponent<Button>();
        resumeButon.onClick.AddListener(ResumeGame);

        quitButton = GameObject.Find("btnQuit").GetComponent<Button>();
        quitButton.onClick.AddListener(ReturnToStartScreen);

        restartGameButton = GameObject.Find("btnRestart").GetComponent<Button>();
        restartGameButton.onClick.AddListener(RestartGame);

        gameOverRestartGameButton = GameObject.Find("btnGameOverRestart").GetComponent<Button>();
        gameOverRestartGameButton.onClick.AddListener(RestartGame);
        gameOverRestartGameButton.gameObject.SetActive(false);

        gameOverQuitGameButton = GameObject.Find("btnGameOverQuit").GetComponent<Button>();
        gameOverQuitGameButton.onClick.AddListener(ReturnToStartScreen);
        gameOverQuitGameButton.gameObject.SetActive(false);

        //don't deactivate the pause menu until we've gotten references to all its children
        pauseMenu.SetActive(false);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        gameOverRestartGameButton.gameObject.SetActive(true);
        gameOverQuitGameButton.gameObject.SetActive(true);
        //enabled = false;
    }

    /// <summary>
    /// Update function runs on every screen
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

        if(playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    /// <summary>
    /// Moves enemies on the board
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void RestartGame()
    {
        level = 1;
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Main");
        SoundManager.instance.musicSource.Play();
    }

    public void ReturnToStartScreen()
    {
        level = 1;
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("StartScreen");
        SoundManager.instance.musicSource.Stop();
    }

    public void IncrementLevel()
    {
        level++;
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
    }

    public bool LevelTransitionScreenActive
    {
        get
        {
            return levelImage.activeSelf;
        }
    }

    private void OnApplicationPause(bool pause)
    {

    }
}
