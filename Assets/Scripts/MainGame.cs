using UnityEngine;
using System.Collections;

/// <summary>
/// Main game script
/// </summary>
public class MainGame : MonoBehaviour {

	// Game layers
	static public int	nGroundLayer = 8;
	static public int	nPlayerLayer = 9;
	static public int	nItemsLayer = 10;
	static public int	nBarrelLayer = 11;
	static public int nWindowsLayer = 12;
	static public int nBalconyGroundLayer = 13;
	static public int nItemGroundLayer = 14;

	// Players types
	public enum ePlayerType {

		DOG,
		DUDE
	};

	public enum eGameStatus {

		GAME_START_SCREEN, 
		GAME_PLAY,			// Game executing
		GAME_PAUSE,			// Game paused: ignore input from the player
		GAME_OVER
	};

	public eGameStatus gameStatus = eGameStatus.GAME_PLAY;

	public Transform 		trDog;
	public Transform		trDude;
	public Player				dudeScript;
	public Player				dogScript;

	public float				fNoiseMade = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	///
	/// </summary>
	public void AddNoise(float fNoise) {

		fNoiseMade += fNoise;	
	}

	/// <summary>
	/// </summary>
	public void ChangeStatusToGameOver() {
		// Change the status
		gameStatus = eGameStatus.GAME_OVER;
		// Activate the message on the screens
		dudeScript.ActivateGameOver();
		dogScript.ActivateGameOver();
	}

	/// <summary>
	///	Return the current game status
	/// </summary>
	public eGameStatus GetCurrentGameStatus() {

		return gameStatus;
	}

	/// <summary>
	/// On the start screen, the dog entered the game
	/// </summary>
	public void DogEnteredTheGame() {

	}
}
