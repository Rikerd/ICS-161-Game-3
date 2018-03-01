using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateManager : MonoBehaviour {
    public GameObject player;

    [Range(1, 10)]
    public int numberOfIterations;

    public float setTurnTimer;
    public float setTurnTimerAfterShooting;

    public Text timerText;

    public Text player1HealthText;
    public Text player1MovementText;
    public Text player2HealthText;
    public Text player2MovementText;

    public Text spawnPrompter;

    public float promptTime;
    public float promptFadeDuration;
    public Text prompter;
    
    private PlayerController player1Controller;
    private PlayerController player2Controller;
    private TileGenerator tileGenerator;
    private float turnTimer;

    private float fadeTimePassed;
    private string fadeAnimationState;

    private bool timerReset;

    private Color prompterOriginalColor;
    private Color prompterEndColor;

    private bool initialGameOver;

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
        initialGameOver = true;
    }

    private void Start()
    {
        prompterOriginalColor = prompter.color;
        prompterEndColor = new Color(prompter.color.r, prompter.color.g, prompter.color.b, 1);

        spawnPrompter.text = "";
    }

    private void Update () {
        print(currentState);

        // Checks if game is over
        if (checkedPlayersDead() && checkIfInitialStates())
        {
            currentState = BattleStates.GameOver;
        }

        // Update UI Accordingly
        if (player1Controller != null)
        {
            player1HealthText.text = "Health: " + player1Controller.hp;
            player1MovementText.text = "Movement: " + player1Controller.getMoveLimit().ToString("F2");
        }
        else
        {
            player1HealthText.text = "Health: 0";
            player1MovementText.text = "Movement: 0.00";
        }

        if (player2Controller != null)
        {
            player2HealthText.text = "Health: " + player2Controller.hp;
            player2MovementText.text = "Movement: " + player2Controller.getMoveLimit().ToString("F2");
        }
        else
        {
            player2HealthText.text = "Health: 0";
            player1MovementText.text = "Movement: 0.00";
        }

        timerText.text = turnTimer.ToString("F2");

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
                    GameObject player1 = Instantiate(player, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                    player1.layer = 9;
                    player1.GetComponent<SpriteRenderer>().color = new Color(0, 171, 255);
                    player1Controller = player1.GetComponent<PlayerController>();

                    currentState = BattleStates.SpawnPlayer2;
                }
                break;
            case (BattleStates.SpawnPlayer2):
                // Spawn Player 2
                spawnPrompter.text = "SPAWN PLAYER 2";

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    GameObject player2 = Instantiate(player, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                    player2.layer = 10;
                    player2.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                    player2Controller = player2.GetComponent<PlayerController>();

                    spawnPrompter.text = "";
                    currentState = BattleStates.StartGame;
                }
                break;
            case (BattleStates.StartGame):
                player1Controller.gameStart(true);
                player2Controller.gameStart(false);

                currentState = BattleStates.StartPlayer1Turn;
                break;
            case (BattleStates.StartPlayer1Turn):
                // Start Player 1 Turn
                prompter.text = "PLAYER 1 TURN";

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
                    player1Controller.startTurn();
                    fadeAnimationState = "Fade In";
                    fadeTimePassed = 0f;

                    currentState = BattleStates.Player1Turn;
                }
                break;
            case (BattleStates.Player1Turn):
                turnTimer -= Time.deltaTime;

                if (player1Controller.didShoot() && !timerReset)
                {
                    if (player1Controller.getMoveLimit() == 0f)
                    {
                        turnTimer = 0f;
                    } else
                    {
                        turnTimer = setTurnTimerAfterShooting;
                    }

                    timerReset = true;
                }

                if (turnTimer <= 0f)
                {
                    timerReset = false;
                    currentState = BattleStates.EndPlayer1Turn;
                }
                break;
            case (BattleStates.EndPlayer1Turn):
                // End Player 1 Turn
                player1Controller.endTurn();
                turnTimer = setTurnTimer;

                currentState = BattleStates.StartPlayer2Turn;
                break;
            case (BattleStates.StartPlayer2Turn):
                // Start Player 2 Turn
                prompter.text = "PLAYER 2 TURN";

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
                    player2Controller.startTurn();
                    fadeAnimationState = "Fade In";
                    fadeTimePassed = 0f;

                    currentState = BattleStates.Player2Turn;
                }
                break;
            case (BattleStates.Player2Turn):
                turnTimer -= Time.deltaTime;

                if (player2Controller.didShoot() && !timerReset)
                {
                    if (player2Controller.getMoveLimit() == 0f)
                    {
                        turnTimer = 0f;
                    }
                    else
                    {
                        turnTimer = setTurnTimerAfterShooting;
                    }

                    timerReset = true;
                }

                if (turnTimer <= 0f)
                {
                    timerReset = false;
                    currentState = BattleStates.EndPlayer2Turn;
                }
                break;
            case (BattleStates.EndPlayer2Turn):
                // End Player 2 Turn
                player2Controller.endTurn();
                turnTimer = setTurnTimer;

                currentState = BattleStates.StartPlayer1Turn;
                break;
            case (BattleStates.GameOver):
                if (initialGameOver)
                {
                    prompter.color = prompterOriginalColor;
                    initialGameOver = false;
                }

                if (player1Controller != null)
                {
                    player1Controller.endTurn();
                    prompter.text = "Player 1 Wins!";
                } else if (player2Controller != null)
                {
                    player2Controller.endTurn();
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
        return (player1Controller == null || player2Controller == null);
    }

    public bool checkIfInitialStates()
    {
        return (currentState != BattleStates.GenerateMap && currentState != BattleStates.SpawnPlayer1 && currentState != BattleStates.SpawnPlayer2);
    }
}