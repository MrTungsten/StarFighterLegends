using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gameOverScreenWin;
    [SerializeField] private GameObject gameOverScreenLoss;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private TextMeshProUGUI scoreTimeBonusText;
    private GameObject player;
    private bool isGameActive = true;
    private bool victory = false;
    private int initialTotalScore = 0;
    private float escapeTimer = 0;
    private float levelTransTimer = 4f;
    private float playerLifeTime = 0f;
    private float scoreTimeLimit = 0f;
    private int scoreTimeBonus = 0;
    private int sceneIndex = 0;
    private bool hasIncreasedScore = false;
    public bool isPlayingAnimation = false;

    private void Start()
    {

        player = GameObject.Find("Player");

        sceneIndex = SceneManager.GetActiveScene().buildIndex - 2;

        if (SceneUtility.GetScenePathByBuildIndex(3) == SceneManager.GetActiveScene().path)
        {
            ScoreManagerScript.Instance.ResetTotalScore();

            PlayerStatsManager.Instance.ResetStats();
        }

        if (sceneIndex <= 5)
        {
            scoreTimeLimit = 120f;
        }
        else
        {
            scoreTimeLimit = 240f;
        }

        ScoreManagerScript.Instance.ResetCurrentScore();
        ScoreManagerScript.Instance.UpdateScoreText(scoreText, totalScoreText, highScoreText);

        PlayerStatsManager.Instance.SetStats();

        initialTotalScore = ScoreManagerScript.Instance.GetTotalScore();

        levelText.text = string.Format("Level\n{0}/{1}", SceneManager.GetActiveScene().buildIndex - 2, SceneManager.sceneCountInBuildSettings - 4);
    }

    private void Update()
    {

        playerLifeTime += Time.unscaledDeltaTime;

        ScoreManagerScript.Instance.UpdateScoreText(scoreText, totalScoreText, highScoreText);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyPlane").ToArray();
        enemies = enemies.Concat(GameObject.FindGameObjectsWithTag("EnemyTank")).ToArray();
        enemies = enemies.Concat(GameObject.FindGameObjectsWithTag("EnemyTurret")).ToArray();
        enemies = enemies.Concat(GameObject.FindGameObjectsWithTag("EnemyDiver")).ToArray();
        enemies = enemies.Concat(GameObject.FindGameObjectsWithTag("EnemySine")).ToArray();
        enemies = enemies.Concat(GameObject.FindGameObjectsWithTag("EnemyDelayed")).ToArray();

        if (Input.GetKeyDown(KeyCode.O) && isGameActive)
        {
            enemies = new GameObject[0];
        }
        else if (Input.GetKeyDown(KeyCode.P) && isGameActive)
        {
            gameObject.AddComponent<BoxCollider2D>();
            player.GetComponent<PlayerScript>().HitByObject(gameObject.GetComponent<BoxCollider2D>(), true);
        }

        int numOfEnemies = enemies.Length;

        if (numOfEnemies == 0 && isGameActive)
        {
            Debug.Log("The player has won!");
            GameOver(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            if (escapeTimer > 3f)
            {
                MainMenu();
            }
            else
            {
                escapeTimer += Time.unscaledDeltaTime;
            }
        }
        else
        {
            escapeTimer = 0f;
        }

        if (!isGameActive && !isPlayingAnimation)
        {
            if (!hasIncreasedScore)
            {
                if (SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 2)
                {
                    ScoreManagerScript.Instance.IncrementScore(500);
                    ScoreManagerScript.Instance.IncrementScore(PlayerStatsManager.Instance.GetStats()[0] * 50);
                }

                if ( (playerLifeTime < scoreTimeLimit) && victory)
                {
                    scoreTimeBonus = ((int)((scoreTimeLimit - playerLifeTime) / 5)) * 5;
                    ScoreManagerScript.Instance.IncrementScore(scoreTimeBonus);
                    scoreTimeBonusText.text = $"Time Bonus: {scoreTimeBonus}";
                }

                ScoreManagerScript.Instance.UpdateScoreText(scoreText, totalScoreText, highScoreText);

                hasIncreasedScore = true;
            }

            if (levelTransTimer <= 0 && victory && !isPlayingAnimation)
            {
                player.GetComponent<PlayerScript>().OutroAnimation();
                gameOverScreen.SetActive(false);
            }
            else if (levelTransTimer <= 0 && !victory)
            {
                SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(SceneManager.sceneCountInBuildSettings - 1));
            }
            else
            {
                countdownText.text = $"Continuing in {Mathf.Floor(levelTransTimer)}";

                levelTransTimer -= Time.deltaTime;
            }
        }
    }

    public void GameOver(bool _victory = false)
    {
        gameOverScreen.SetActive(true);

        victory = _victory;

        if (victory)
        {
            GameOverWin();
        }
        else
        {
            GameOverLoss();
        }

        isGameActive = false;
    }

    public void GameActive()
    {
        isGameActive = true;
        isPlayingAnimation = false;
    }

    public void GameInactive()
    {
        isGameActive = false;
        isPlayingAnimation = true;
    }

    private void GameOverWin()
    {
        gameOverScreenWin.SetActive(true);
    }
    
    private void GameOverLoss()
    {
        gameOverScreenLoss.SetActive(true);
        scoreTimeBonusText.enabled = false;
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void NextLevel()
    {
        PlayerStatsManager.Instance.UpdatePlayerStats();
        int sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(sceneIndex));
    }

    public void Restart()
    {
        ScoreManagerScript.Instance.ResetSceneScore(initialTotalScore);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(1));
    }

}