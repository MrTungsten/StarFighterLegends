using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreTableScript : MonoBehaviour
{

    private Transform entryContainer;
    private Transform entryTemplate;
    private List<Transform> highScoreTransformList;
    private HighScores highScores = new HighScores();
    private Transform tableTransform;
    private TextMeshProUGUI instructionText;
    private float timer = 0f;
    private float delayTime = 3f;
    private bool isViewing = false;

    private void Awake()
    {
        instructionText = transform.Find("Table").transform.Find("Text").transform.Find("InstructionsText").GetComponent<TextMeshProUGUI>();
        instructionText.enabled = false;

        tableTransform = transform.Find("Table").transform;
        entryContainer = tableTransform.Find("HighScoreEntryContainer");
        entryTemplate = entryContainer.Find("HighScoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        highScores = GetHighScoreJson(highScores);

        SortHighScoreList();

        CreateHighScoreTable();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && (timer >= delayTime))
        {
            SceneManager.LoadScene(SceneUtility.GetScenePathByBuildIndex(1));
        }
        else
        {
            if (timer >= delayTime)
            {
                instructionText.enabled = true;
            }

            if (isViewing)
            {
                timer += Time.deltaTime;
            }
        }
    }

    public bool ReturnValidHighScore()
    {
        if (ScoreManagerScript.Instance == null)
        {
            return false;
        }

        if (ScoreManagerScript.Instance.GetTotalScore() == 0)
        {
            return false;
        }

        SortHighScoreList();

        if (highScores.highScoreEntryList.Count >= 10)
        {
            if (ScoreManagerScript.Instance.GetTotalScore() > highScores.highScoreEntryList[9].score)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
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

    private void CreateHighScoreTable()
    {
        if (highScoreTransformList != null)
        {
            foreach (Transform transform in highScoreTransformList)
            {
                Destroy(transform.gameObject);
            }
        }

        highScoreTransformList = new List<Transform>();
        foreach (HighScoreEntry highScoreEntry in highScores.highScoreEntryList)
        {
            CreateHighScoreEntryTransform(highScoreEntry, entryContainer, highScoreTransformList);
        }
    }

    private void CreateHighScoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 65f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }
        entryTransform.Find("PosText").GetComponent<TextMeshProUGUI>().text = rankString;

        int score = highScoreEntry.score;
        entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().text = score.ToString();

        string name = highScoreEntry.name;
        entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().text = name;

        entryTransform.Find("Background").gameObject.SetActive(rank % 2 != 0);

        if (rank == 1)
        {
            entryTransform.Find("PosText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            entryTransform.Find("ScoreText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
            entryTransform.Find("NameText").GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }

        transformList.Add(entryTransform);
    }

    public void AddHighScoreEntry(int score, string name)
    {
        HighScoreEntry highScoreEntry = new HighScoreEntry { score = score, name = name };
        highScores = GetHighScoreJson(highScores);
        highScores.highScoreEntryList.Add(highScoreEntry);
        SetHighScoreJson(highScores);
        Debug.Log($"Added {name} with a score of {score}");

        SortHighScoreList();

        CreateHighScoreTable();
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

    private void SetHighScoreJson(HighScores highScores)
    {
        string json = JsonUtility.ToJson(highScores);
        string filePath = Application.persistentDataPath + "/HighScoreData.json";
        Debug.Log("Successfully set Json!");
        Debug.Log(filePath);
        System.IO.File.WriteAllText(filePath, json);
    }

    private HighScores GetHighScoreJson(HighScores highScores)
    {
        string filePath = Application.persistentDataPath + "/HighScoreData.json";

        if (System.IO.File.Exists(filePath))
        {
            string json = System.IO.File.ReadAllText(filePath);
            highScores = JsonUtility.FromJson<HighScores>(json);
            Debug.Log("Successfully get Json!");
            Debug.Log(filePath);

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
    }

    public void SetIsViewing()
    {
        isViewing = true;
    }

}