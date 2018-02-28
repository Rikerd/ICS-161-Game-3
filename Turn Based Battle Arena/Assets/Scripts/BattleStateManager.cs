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
    
    private PlayerController player1Controller;
    private PlayerController player2Controller;
    private TileGenerator tileGenerator;
    private float turnTimer;

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
                player1Controller.startTurn();

                currentState = BattleStates.Player1Turn;
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
                player2Controller.startTurn();

                currentState = BattleStates.Player2Turn;
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
}