using UnityEngine;
using TMPro; // Add this line for TextMeshPro
using System.Collections.Generic;

public class ScoreManagerScript : MonoBehaviour
{

    // Dictionary to store score values for different GameObject types
    private Dictionary<string, int> scoreValues;

    // Variable to hold the current score
    private int currentScore = 0;

    // Variable to hold the total score
    private static int totalScore;

    private int highScore = 0;

    // Singleton pattern to ensure only one instance of ScoreManagerScript exists
    public static ScoreManagerScript Instance { get; private set; }

    private HighScores highScores = new HighScores();

    private void Awake()
    {
        // Singleton pattern
        CreateSingleton();

        // Initialize the score values for different GameObject types
        scoreValues = new Dictionary<string, int>();
        scoreValues["EnemyDiver"] = 15;
        scoreValues["EnemyPlane"] = 10;
        scoreValues["EnemyTurret"] = 50;
        scoreValues["EnemyTank"] = 10;
        scoreValues["EnemySine"] = 15;
        scoreValues["EnemyDelayed"] = 20;

        highScore = GetHighestScore();
    }

    private void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Method to increment the score when a GameObject is destroyed
    public void IncrementScore(string gameObjectType)
    {
        if (scoreValues.ContainsKey(gameObjectType))
        {
            currentScore += scoreValues[gameObjectType];
            totalScore += scoreValues[gameObjectType];
            CheckAndUpdateHighScore(); // Update the high score
        }
    }

    public void IncrementScore(int scoreToAdd)
    {
        currentScore += scoreToAdd;
        totalScore += scoreToAdd;
        CheckAndUpdateHighScore(); // Update the high score
    }

    // Method to increment the score when the player picks up the score powerup
    public void ScorePowerup()
    {
        currentScore += 100;
        totalScore += 100;
        CheckAndUpdateHighScore(); // Update the high score
    }

    // Method to check and update high score
    public void CheckAndUpdateHighScore()
    {
        if (totalScore > highScore)
        {
            highScore = totalScore;
        }
    }

    // Method to reset score when the player restarts
    public void ResetCurrentScore()
    {
        currentScore = 0;
    }

    // Method to reset total score when the player restarts from the first scene
    public void ResetTotalScore()
    {
        totalScore = 0;
    }

    // Method to update the score text
    public void UpdateScoreText(TextMeshProUGUI scoreText, TextMeshProUGUI totalScoreText, TextMeshProUGUI highScoreText)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score\n" + currentScore.ToString();
        }
        else
        {
            Debug.LogWarning("Score text reference is not set.");
        }

        if (totalScoreText != null)
        {
            totalScoreText.text = "Total\n" + totalScore.ToString();
        }
        else
        {
            Debug.LogWarning("TotalScore text reference is not set.");
        }

        if (highScoreText != null)
        {
            highScoreText.text = "High\n" + highScore.ToString();
        }
        else
        {
            Debug.LogWarning("HighScore text reference is not set.");
        }
    }

    // Method to reset total score when the player restarts
    public void ResetSceneScore(int initialTotalScore)
    {
        totalScore = initialTotalScore;
    }

    public int GetTotalScore()
    {
        return totalScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    private void SetHighScoreJson(HighScores highScores)
    {
        string json = JsonUtility.ToJson(highScores);
        string filePath = Application.persistentDataPath + "/HighScoreData.json";
        System.IO.File.WriteAllText(filePath, json);
        Debug.Log("Set Json: " + filePath);
    }

    private HighScores GetHighScoreJson(HighScores highScores)
    {
        string filePath = Application.persistentDataPath + "/HighScoreData.json";

        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            highScores = JsonUtility.FromJson<HighScores>(json);

            if (highScores == null)
                highScores = new HighScores();

            return highScores;
        }
        else
        {
            SetHighScoreJson(highScores);

            if (highScores == null)
                highScores = new HighScores();

            return highScores;
        }

        Debug.Log("Get Json: " + filePath);
    }

    private void SortHighScoreList()
    {

        highScores = GetHighScoreJson(highScores);

        for (int i = 0; i < highScores.highScoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highScores.highScoreEntryList.Count; j++)
            {
                if (highScores.highScoreEntryList[j].score > highScores.highScoreEntryList[i].score)
                {
                    HighScoreEntry tmp = highScores.highScoreEntryList[i];
                    highScores.highScoreEntryList[i] = highScores.highScoreEntryList[j];
                    highScores.highScoreEntryList[j] = tmp;
                }
            }
        }

        if (highScores.highScoreEntryList.Count > 10)
        {
            for (int i = 0; i < highScores.highScoreEntryList.Count; i++)
            {
                if (i >= 10)
                {
                    highScores.highScoreEntryList.RemoveAt(i);
                }
            }
        }

        SetHighScoreJson(highScores);
    }

    private int GetHighestScore()
    {
        SortHighScoreList();
        if (highScores.highScoreEntryList.Count != 0)
        {
            return highScores.highScoreEntryList[0].score;
        }
        else
        {
            return 0;
        }
        
    }

    private class HighScores
    {
        public List<HighScoreEntry> highScoreEntryList = new List<HighScoreEntry>();
    }

    [System.Serializable]
    private class HighScoreEntry
    {
        public int score;
        public string name;
    }

}