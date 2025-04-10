using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionScreenScript : MonoBehaviour
{

    private float instructionsTimer = 0.25f;

    private void Update()
    {
        if (Time.timeSinceLevelLoadAsDouble > instructionsTimer)
        {
            if (GameInputScript.Instance.PlayerCommands().Equals("blaster"))
            {
                SceneManager.LoadScene("Level 1");
            }
        }
    }

}