using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class BattleStateManager : MonoBehaviour {
    public GameObject player;
    public int numberOfCharacter;
    public float setMovementLimit;
    public float cameraTrackingDuration;

    [HideInInspector]
    public float movementLimit;
    [HideInInspector]
    public bool shot;

    [Range(1, 10)]
    public int numberOfIterations;
    public GridLayout grid;
    public Tilemap tilemap;

    public float setTurnTimer;
    public float setTurnTimerWithNoMovement;
    public float setTurnTimerAfterShooting;

    public Text timerText;
    
    public Text player1MovementText;
    public Text player2MovementText;

    public Text spawnPrompter;

    public float promptTime;
    public float promptFadeDuration;
    public Text prompter;

    public CameraScript cameraScript;
    public float originalZoom;
    public float playerZoom;

    private PlayerController currentCharacter;
    private int currentCharacterIndex;
    public List<PlayerController> player1Controllers;
    public List<PlayerController> player2Controllers;

    private TileGenerator tileGenerator;
    private float turnTimer;

    private float fadeTimePassed;
    private string fadeAnimationState;

    private bool timerReset;

    private Color prompterOriginalColor;
    private Color prompterEndColor;

    private bool initialGameOver;

    private bool cameraMoving;

    public enum BattleStates
    {
        GenerateMap,
        SpawnPlayer1,
        SpawnPlayer2,
        StartGame,
        StartPlayer1Turn,
        Player1Turn,
        EndPlayer1Turn,
        StartPlayer2Turn,
        Player2Turn,
        EndPlayer2Turn,
        GameOver
    }

    public static BattleStates currentState = BattleStates.GenerateMap;
    
    private void Awake () {
        tileGenerator = GetComponent<TileGenerator>();
		currentState = BattleStates.GenerateMap;
        turnTimer = setTurnTimer;
        fadeAnimationState = "Fade In";
        fadeTimePassed = 0f;
        timerReset = false;
        shot = false;
        initialGameOver = true;
        cameraMoving = true;

        player1Controllers = new List<PlayerController>();
        player2Controllers = new List<PlayerController>();
    }

    private void Start()
    {
        prompterOriginalColor = prompter.color;
        prompterEndColor = new Color(prompter.color.r, prompter.color.g, prompter.color.b, 1);

        spawnPrompter.text = "";
    }

    private void Update () {
        // Checks if game is over
        if (checkedPlayersDead() && checkIfInitialStates())
        {
            currentState = BattleStates.GameOver;
        }

        // Update UI
        if (currentState == BattleStates.Player1Turn)
        {
            if (movementLimit > 0f)
            {
                player1MovementText.text = "Movement: " + movementLimit.ToString("F2");
            } else
            {
                player1MovementText.text = "Movement: 0.00";
            }
        } else
        {
            player1MovementText.text = "Movement: --";
        }

        if (currentState == BattleStates.Player2Turn)
        {
            if (movementLimit > 0f)
            {
                player2MovementText.text = "Movement: " + movementLimit.ToString("F2");
            }
            else
            {
                player2MovementText.text = "Movement: 0.00";
            }
        }
        else
        {
            player2MovementText.text = "Movement: --";
        }

        if (turnTimer > 0f)
        {
            timerText.text = turnTimer.ToString("F2");
        } else
        {
            timerText.text = ("0.00");
        }

        // Lock Camera
        if (currentCharacter != null && Input.GetKeyDown(KeyCode.LeftControl))
        {
            cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, true);
            cameraScript.SetCurrentCharacter(currentCharacter.transform);
        }

        // Checks and perform the proper state
        switch (currentState)
        {
            case (BattleStates.GenerateMap):
                tileGenerator.doSimulation(numberOfIterations);
                currentState = BattleStates.SpawnPlayer1;
                break;
            case (BattleStates.SpawnPlayer1):
                // Spawn Player 1
                spawnPrompter.text = "SPAWN PLAYER 1";

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    Vector3Int tileCheckPosition = grid.WorldToCell(mousePos);

                    if (!tilemap.HasTile(tileCheckPosition))
                    {
                        GameObject player1 = Instantiate(player, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                        player1.layer = 9;
                        player1.GetComponent<SpriteRenderer>().color = new Color(0, 171, 255);

                        player1Controllers.Add(player1.GetComponent<PlayerController>());
                        
                        currentState = BattleStates.SpawnPlayer2;
                    }
                }
                break;
            case (BattleStates.SpawnPlayer2):
                // Spawn Player 2
                spawnPrompter.text = "SPAWN PLAYER 2";

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    Vector3Int tileCheckPosition = grid.WorldToCell(mousePos);

                    if (!tilemap.HasTile(tileCheckPosition))
                    {
                        GameObject player2 = Instantiate(player, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                        player2.layer = 10;
                        player2.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);

                        player2Controllers.Add(player2.GetComponent<PlayerController>());

                        if (player2Controllers.Count == numberOfCharacter)
                        {
                            spawnPrompter.text = "";
                            currentState = BattleStates.StartGame;
                        } else
                        {
                            currentState = BattleStates.SpawnPlayer1;
                        }
                    }
                }
                break;
            case (BattleStates.StartGame):
                foreach (PlayerController player1Controller in player1Controllers)
                {
                    player1Controller.gameStart(true);
                }

                foreach (PlayerController player2Controller in player2Controllers)
                {
                    player2Controller.gameStart(false);
                }

                cameraMoving = true;

                currentState = BattleStates.StartPlayer1Turn;
                break;
            case (BattleStates.StartPlayer1Turn):
                // Start Player 1 Turn
                prompter.text = "PLAYER 1 TURN";

                if (cameraMoving)
                {
                    cameraScript.CameraMovement(new Vector3(0, 0, 0), originalZoom, promptFadeDuration, false);
                    cameraMoving = false;
                }

                if (fadeAnimationState == "Fade In")
                {
                    fadeTimePassed += Time.deltaTime;
                    prompter.color = Color.Lerp(prompterOriginalColor, prompterEndColor, fadeTimePassed / promptFadeDuration);
                }
                else if (fadeAnimationState == "Display Prompt")
                {
                    fadeTimePassed += Time.deltaTime;
                }
                else if (fadeAnimationState == "Fade Out")
                {
                    fadeTimePassed += Time.deltaTime;
                    prompter.color = Color.Lerp(prompterEndColor, prompterOriginalColor, fadeTimePassed / promptFadeDuration);
                }

                if (fadeTimePassed > promptFadeDuration && fadeAnimationState == "Fade In")
                {
                    fadeAnimationState = "Display Prompt";
                    fadeTimePassed = 0f;
                }
                else if (fadeTimePassed > promptTime && fadeAnimationState == "Display Prompt")
                {
                    fadeAnimationState = "Fade Out";
                    fadeTimePassed = 0f;
                }
                else if (fadeTimePassed > promptFadeDuration && fadeAnimationState == "Fade Out")
                {
                    fadeAnimationState = "Fade In";
                    fadeTimePassed = 0f;

                    movementLimit = setMovementLimit;
                    shot = false;

                    currentCharacter = player1Controllers[0];
                    currentCharacterIndex = 0;
                    currentCharacter.activateCharacter();

                    cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);

                    currentState = BattleStates.Player1Turn;
                }
                break;
            case (BattleStates.Player1Turn):
                turnTimer -= Time.deltaTime;
                
                if (shot && !timerReset)
                {
                    if (movementLimit <= 0f)
                    {
                        turnTimer = setTurnTimerWithNoMovement;
                    } else
                    {
                        turnTimer = setTurnTimerAfterShooting;
                    }

                    timerReset = true;
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    cameraScript.deactiveCameraLock();
                    currentCharacter.deactivateCharacter();
                    currentCharacterIndex++;

                    if (currentCharacterIndex == player1Controllers.Count)
                    {
                        currentCharacterIndex = 0;
                        currentCharacter = player1Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                    else
                    {
                        currentCharacter = player1Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                } else if (Input.GetKeyDown(KeyCode.S))
                {
                    cameraScript.deactiveCameraLock();
                    currentCharacter.deactivateCharacter();
                    currentCharacterIndex--;

                    if (currentCharacterIndex < 0)
                    {
                        currentCharacterIndex = player1Controllers.Count - 1;
                        currentCharacter = player1Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                    else
                    {
                        currentCharacter = player1Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                }

                if (turnTimer <= 0f)
                {
                    timerReset = false;
                    currentState = BattleStates.EndPlayer1Turn;
                }
                break;
            case (BattleStates.EndPlayer1Turn):
                // End Player 1 Turn
                cameraScript.deactiveCameraLock();
                currentCharacter.deactivateCharacter();
                currentCharacter = null;
                turnTimer = setTurnTimer;
                cameraMoving = true;

                currentState = BattleStates.StartPlayer2Turn;
                break;
            case (BattleStates.StartPlayer2Turn):
                // Start Player 2 Turn
                prompter.text = "PLAYER 2 TURN";

                if (cameraMoving)
                {
                    cameraScript.CameraMovement(new Vector3(0, 0, 0), originalZoom, promptFadeDuration, false);
                    cameraMoving = false;
                }

                if (fadeAnimationState == "Fade In")
                {
                    fadeTimePassed += Time.deltaTime;
                    prompter.color = Color.Lerp(prompterOriginalColor, prompterEndColor, fadeTimePassed / promptFadeDuration);
                }
                else if (fadeAnimationState == "Display Prompt")
                {
                    fadeTimePassed += Time.deltaTime;
                }
                else if (fadeAnimationState == "Fade Out")
                {
                    fadeTimePassed += Time.deltaTime;
                    prompter.color = Color.Lerp(prompterEndColor, prompterOriginalColor, fadeTimePassed / promptFadeDuration);
                }

                if (fadeTimePassed > promptFadeDuration && fadeAnimationState == "Fade In")
                {
                    fadeAnimationState = "Display Prompt";
                    fadeTimePassed = 0f;
                }
                else if (fadeTimePassed > promptTime && fadeAnimationState == "Display Prompt")
                {
                    fadeAnimationState = "Fade Out";
                    fadeTimePassed = 0f;
                }
                else if (fadeTimePassed > promptFadeDuration && fadeAnimationState == "Fade Out")
                {
                    fadeAnimationState = "Fade In";
                    fadeTimePassed = 0f;

                    movementLimit = setMovementLimit;
                    shot = false;

                    currentCharacter = player2Controllers[0];
                    currentCharacterIndex = 0;
                    currentCharacter.activateCharacter();

                    cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);

                    currentState = BattleStates.Player2Turn;
                }
                break;
            case (BattleStates.Player2Turn):
                turnTimer -= Time.deltaTime;
                
                if (shot && !timerReset)
                {
                    if (movementLimit <= 0f)
                    {
                        turnTimer = setTurnTimerWithNoMovement;
                    }
                    else
                    {
                        turnTimer = setTurnTimerAfterShooting;
                    }

                    timerReset = true;
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    cameraScript.deactiveCameraLock();
                    currentCharacter.deactivateCharacter();
                    currentCharacterIndex++;

                    if (currentCharacterIndex == player2Controllers.Count)
                    {
                        currentCharacterIndex = 0;
                        currentCharacter = player2Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                    else
                    {
                        currentCharacter = player2Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.S))
                {
                    cameraScript.deactiveCameraLock();
                    currentCharacter.deactivateCharacter();
                    currentCharacterIndex--;

                    if (currentCharacterIndex < 0)
                    {
                        currentCharacterIndex = player2Controllers.Count - 1;
                        currentCharacter = player2Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                    else
                    {
                        currentCharacter = player2Controllers[currentCharacterIndex];
                        currentCharacter.activateCharacter();

                        cameraScript.CameraMovement(currentCharacter.transform.position, playerZoom, cameraTrackingDuration, false);
                    }
                }

                if (turnTimer <= 0f)
                {
                    timerReset = false;
                    currentState = BattleStates.EndPlayer2Turn;
                }
                break;
            case (BattleStates.EndPlayer2Turn):
                // End Player 2 Turn
                cameraScript.deactiveCameraLock();
                currentCharacter.deactivateCharacter();
                currentCharacter = null;
                turnTimer = setTurnTimer;
                cameraMoving = true;

                currentState = BattleStates.StartPlayer1Turn;
                break;
            case (BattleStates.GameOver):
                if (initialGameOver)
                {
                    cameraScript.deactiveCameraLock();
                    prompter.color = prompterOriginalColor;
                    initialGameOver = false;
                }

                if (currentCharacter != null)
                {
                    currentCharacter.deactivateCharacter();
                    currentCharacter = null;
                }

                if (player1Controllers.Count != 0)
                {
                    prompter.text = "Player 1 Wins!";
                } else if (player2Controllers.Count != 0)
                {
                    prompter.text = "Player 2 Wins!";
                }

                if (fadeTimePassed < promptFadeDuration)
                {
                    fadeTimePassed += Time.deltaTime;
                    prompter.color = Color.Lerp(prompterOriginalColor, prompterEndColor, fadeTimePassed / promptFadeDuration);
                }

                break;
        }
    }
    
    public bool checkedPlayersDead()
    {
        return (player1Controllers.Count == 0 || player2Controllers.Count == 0);
    }

    public bool checkIfInitialStates()
    {
        return (currentState != BattleStates.GenerateMap && currentState != BattleStates.SpawnPlayer1 && currentState != BattleStates.SpawnPlayer2);
    }

    public void deleteCharacter(bool isPlayer1, PlayerController controller)
    {
        if (isPlayer1)
        {
            player1Controllers.Remove(controller);
        } else
        {
            player2Controllers.Remove(controller);
        }
    }
}