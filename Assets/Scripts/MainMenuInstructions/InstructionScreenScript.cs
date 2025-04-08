using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionScreenScript : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SceneManager.LoadScene("Level 1");
        }
    }

}