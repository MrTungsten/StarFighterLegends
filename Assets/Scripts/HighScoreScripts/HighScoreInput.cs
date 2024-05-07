using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreInput : MonoBehaviour
{

    public enum CurrentInitial
    {
        First,
        Second,
        Third
    }

    private HighScoreTableScript highScoreTableScript;
    private GameObject table;
    private Transform playerNameInput;
    private CurrentInitial currentInitial = CurrentInitial.First;
    private GameObject playerFirstInitial;
    private GameObject playerSecondInitial;
    private GameObject playerThirdInitial;
    private TextMeshProUGUI playerFirstInitialText;
    private TextMeshProUGUI playerSecondInitialText;
    private TextMeshProUGUI playerThirdInitialText;
    private TextMeshProUGUI currentCharText;
    private TextMeshProUGUI previousCharText;
    private TextMeshProUGUI nextCharText;
    private char[] listOfAlphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    private char currentlySelectedChar = 'A';
    private string playerName = "";
    private float intiialTimer = 0f;
    private float initalScrollCooldown = 0.5f;
    private float scrollTimer = 0f;
    private float scrollCooldown = 0.15f;

    private void Awake()
    {

        playerNameInput = transform.Find("NameInputScreen");

        highScoreTableScript = GameObject.FindAnyObjectByType<HighScoreTableScript>();
        table = GameObject.Find("Table");

        if (highScoreTableScript.ReturnValidHighScore())
        {
            table.SetActive(false);
            playerFirstInitial = GameObject.Find("FirstNameInitial");
            playerSecondInitial = GameObject.Find("SecondNameInitial");
            playerThirdInitial = GameObject.Find("ThirdNameInitial");
            playerFirstInitialText = playerFirstInitial.transform.Find("FirstInitialText").GetComponent<TextMeshProUGUI>();
            playerSecondInitialText = playerSecondInitial.transform.Find("SecondInitialText").GetComponent<TextMeshProUGUI>();
            playerThirdInitialText = playerThirdInitial.transform.Find("ThirdInitialText").GetComponent<TextMeshProUGUI>();
            currentCharText = playerFirstInitialText;
        }
        else
        {
            table.SetActive(true);
            highScoreTableScript.SetIsViewing();
            gameObject.SetActive(false);
        }
        
    }

    private void Update()
    {
        CurrentAlpha();
    }

    private void CurrentAlpha()
    {

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentlySelectedChar = GetAlpha(currentlySelectedChar, "previous");
            UpdateText(currentCharText, false);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            if (intiialTimer > initalScrollCooldown)
            {
                if (scrollTimer > scrollCooldown)
                {
                    currentlySelectedChar = GetAlpha(currentlySelectedChar, "previous");
                    UpdateText(currentCharText, false);
                    scrollTimer = 0f;
                }
                else
                {
                    scrollTimer += Time.deltaTime;
                }
            }
            else
            {
                intiialTimer += Time.deltaTime;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentlySelectedChar = GetAlpha(currentlySelectedChar, "next");
            UpdateText(currentCharText, false);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            if (intiialTimer > initalScrollCooldown)
            {
                if (scrollTimer > scrollCooldown)
                {
                    currentlySelectedChar = GetAlpha(currentlySelectedChar, "next");
                    UpdateText(currentCharText, false);
                    scrollTimer = 0f;
                }
                else
                {
                    scrollTimer += Time.deltaTime;
                }
            }
            else
            {
                intiialTimer += Time.deltaTime;
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            playerName += currentlySelectedChar;
            Debug.Log("Player name: " + playerName);
            UpdateText(currentCharText, true);
        }
        if (Input.anyKeyDown)
        {
            intiialTimer = 0f;
            scrollTimer = 0f;
        }
    }

    private char GetAlpha(char letter, string mode)
    {
        int indexOfLetter = Array.IndexOf(listOfAlphabet, letter);

        if (mode == "previous")
        {
            if (indexOfLetter == 0)
            {
                return 'Z';
            }
            else
            {
                return listOfAlphabet[indexOfLetter - 1];
            }
        }
        else if (mode == "next")
        {
            if (indexOfLetter == listOfAlphabet.Length - 1)
            {
                return 'A';
            }
            else
            {
                return listOfAlphabet[indexOfLetter + 1];
            }
        }
        else
            return '0';
    }

    private void UpdateText(TextMeshProUGUI charToUpdate, bool submission)
    {
        if (submission == true)
        {

            currentlySelectedChar = 'A';

            switch (currentInitial)
            {
                case CurrentInitial.First:
                    playerFirstInitialText.color = new Color(0, 0, 0, 1);
                    currentInitial = CurrentInitial.Second;
                    playerSecondInitialText.color = new Color(0, 0, 1, 1);
                    break;
                case CurrentInitial.Second:
                    playerSecondInitialText.color = new Color(0, 0, 0, 1);
                    currentInitial = CurrentInitial.Third;
                    playerThirdInitialText.color = new Color(0, 0, 1, 1);
                    break;
                case CurrentInitial.Third:
                    ActivateHighScoreTable();
                    break;
            }
        }
        else
        {
            switch (currentInitial)
            {
                case CurrentInitial.First:
                    charToUpdate = playerFirstInitialText;
                    previousCharText = playerFirstInitial.transform.Find("PreviousLetter").GetComponent<TextMeshProUGUI>();
                    nextCharText = playerFirstInitial.transform.Find("NextLetter").GetComponent<TextMeshProUGUI>();
                    charToUpdate.text = Convert.ToString(currentlySelectedChar);
                    previousCharText.text = Convert.ToString(GetAlpha(Convert.ToChar(charToUpdate.text), "previous"));
                    nextCharText.text = Convert.ToString(GetAlpha(Convert.ToChar(charToUpdate.text), "next"));
                    break;
                case CurrentInitial.Second:
                    charToUpdate = playerSecondInitialText;
                    previousCharText = playerSecondInitial.transform.Find("PreviousLetter").GetComponent<TextMeshProUGUI>();
                    nextCharText = playerSecondInitial.transform.Find("NextLetter").GetComponent<TextMeshProUGUI>();
                    charToUpdate.text = Convert.ToString(currentlySelectedChar);
                    previousCharText.text = Convert.ToString(GetAlpha(Convert.ToChar(charToUpdate.text), "previous"));
                    nextCharText.text = Convert.ToString(GetAlpha(Convert.ToChar(charToUpdate.text), "next"));
                    break;
                case CurrentInitial.Third:
                    charToUpdate = playerThirdInitialText;
                    previousCharText = playerThirdInitial.transform.Find("PreviousLetter").GetComponent<TextMeshProUGUI>();
                    nextCharText = playerThirdInitial.transform.Find("NextLetter").GetComponent<TextMeshProUGUI>();
                    charToUpdate.text = Convert.ToString(currentlySelectedChar);
                    previousCharText.text = Convert.ToString(GetAlpha(Convert.ToChar(charToUpdate.text), "previous"));
                    nextCharText.text = Convert.ToString(GetAlpha(Convert.ToChar(charToUpdate.text), "next"));
                    break;
            }
        }
    }

    private void ActivateHighScoreTable()
    {
        table.SetActive(true);
        highScoreTableScript.AddHighScoreEntry(ScoreManagerScript.Instance.GetTotalScore(), playerName);
        highScoreTableScript.SetIsViewing();
        gameObject.SetActive(false);
    }
}