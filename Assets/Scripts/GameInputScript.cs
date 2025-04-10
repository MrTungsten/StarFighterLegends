using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputScript : MonoBehaviour
{
    public static GameInputScript Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        CreateSingleton();
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
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

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }

    public string PlayerCommands()
    {
        string playerCommand = "";

        if (playerInputActions.Player.Blaster.IsPressed()) playerCommand = "blaster";
        if (playerInputActions.Player.Bomb.IsPressed()) playerCommand = "bomb";
        if (playerInputActions.Player.Laser.IsPressed()) playerCommand = "laser";
        if (playerInputActions.Player.SlowTime.IsPressed()) playerCommand = "slowTime";

        return playerCommand;
    }

    public string DebugCommand()
    {
        string debugCommand = "";

        if (Input.GetKeyDown(KeyCode.P)) debugCommand = "killAll";
        if (Input.GetKeyDown(KeyCode.O)) debugCommand = "killSelf";
        if (Input.GetKeyDown(KeyCode.RightControl)) debugCommand = "pause";

        return debugCommand;

    }

    public bool MovePressedThisFrame()
    {
        return playerInputActions.Player.Move.WasPressedThisFrame();
    }

    public bool FirePressedThisFrame()
    {
        return playerInputActions.Player.Blaster.WasPressedThisFrame();
    }
}
