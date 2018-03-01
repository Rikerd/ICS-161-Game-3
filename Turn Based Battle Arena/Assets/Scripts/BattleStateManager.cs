using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleStateManager : MonoBehaviour {
    public GameObject player;

    [Range(1, 10)]
    public int numberOfIterations;

    public float setTurnTimer;

    public Text player1HealthText;
    public Text player2HealthText;

    public float promptTime;
    public float promptFadeDuration;
    public Text player1TurnPrompt;
    public Text player2TurnPrompt;
    
    private PlayerController player1Controller;
    private PlayerController player2Controller;
    private TileGenerator tileGenerator;
    private float turnTimer;

    private float fadeTimePassed;
    private string fadeAnimationState;

    private bool coroutineStarted;

    private Color player1TurnPromptOriginalColor;
    private Color player1TurnPromptEndColor;
    private Color player2TurnPromptOriginalColor;
    private Color player2TurnPromptEndColor;

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
    }

    private void Start()
    {
        player1TurnPromptOriginalColor = player1TurnPrompt.color;
        player1TurnPromptEndColor = new Color(player1TurnPrompt.color.r, player1TurnPrompt.color.g, player1TurnPrompt.color.b, 1);

        player2TurnPromptOriginalColor = player2TurnPrompt.color;
        player2TurnPromptEndColor = new Color(player2TurnPrompt.color.r, player2TurnPrompt.color.g, player2TurnPrompt.color.b, 1);
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
        }
        else
        {
            player1HealthText.text = "Health: 0";
        }

        if (player2Controller != null)
        {
            player2HealthText.text = "Health: " + player2Controller.hp;
        }
        else
        {
            player2HealthText.text = "Health: 0";
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
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    GameObject player2 = Instantiate(player, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
                    player2.layer = 10;
                    player2.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                    player2Controller = player2.GetComponent<PlayerController>();

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
                if (fadeAnimationState == "Fade In")
                {
                    fadeTimePassed += Time.deltaTime;
                    player1TurnPrompt.color = Color.Lerp(player1TurnPromptOriginalColor, player1TurnPromptEndColor, fadeTimePassed / promptFadeDuration);
                }
                else if (fadeAnimationState == "Display Prompt")
                {
                    fadeTimePassed += Time.deltaTime;
                }
                else if (fadeAnimationState == "Fade Out")
                {
                    fadeTimePassed += Time.deltaTime;
                    player1TurnPrompt.color = Color.Lerp(player1TurnPromptEndColor, player1TurnPromptOriginalColor, fadeTimePassed / promptFadeDuration);
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

                if (turnTimer <= 0f || player1Controller.didShoot())
                {
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
                if (fadeAnimationState == "Fade In")
                {
                    fadeTimePassed += Time.deltaTime;
                    player2TurnPrompt.color = Color.Lerp(player2TurnPromptOriginalColor, player2TurnPromptEndColor, fadeTimePassed / promptFadeDuration);
                }
                else if (fadeAnimationState == "Display Prompt")
                {
                    fadeTimePassed += Time.deltaTime;
                }
                else if (fadeAnimationState == "Fade Out")
                {
                    fadeTimePassed += Time.deltaTime;
                    player2TurnPrompt.color = Color.Lerp(player2TurnPromptEndColor, player2TurnPromptOriginalColor, fadeTimePassed / promptFadeDuration);
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

                if (turnTimer <= 0f || player2Controller.didShoot())
                {
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
                if (player1Controller != null)
                {
                    player1Controller.endTurn();
                } else if (player2Controller != null)
                {
                    player2Controller.endTurn();
                }

                player1TurnPrompt.color = player1TurnPromptOriginalColor;
                player2TurnPrompt.color = player2TurnPromptOriginalColor;

                Debug.Log("VICTORY");
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

    IEnumerator PromptPlayerTurn()
    {
        yield return new WaitForSeconds(promptFadeDuration);
    }
}