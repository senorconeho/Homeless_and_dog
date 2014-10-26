﻿using UnityEngine;
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
	static public int	nGroundCheckLayer = 12;
	static public int nWindowsLayer = 13;
	static public int nBalconyGroundLayer = 14;
	static public int nItemGroundLayer = 15;

	// Players types
	public enum ePlayerType {

		DOG,
		DUDE,
		NPC
	};

	public enum eGameStatus {

		GAME_START_SCREEN, 
		GAME_PLAY,				// Game executing
		GAME_PAUSE,				// Game paused: ignore input from the player
		GAME_OVER
	};

	public eGameStatus gameStatus = eGameStatus.GAME_START_SCREEN;

	[SerializeField]
	public Transform 		trDog;
	[SerializeField]
	public Transform		trDude;
	public Player				dudeScript;
	public Player				dogScript;
	public GameHUD			hudDudeScript;
	public GameHUD			hudDogScript;
	public bool					bnDogEnteredTheGame = false;
	public bool					bnDudeEnteredTheGame = false;

	public float				fNoiseMade = 0.0f;

	public Font					fontInGame;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// Use this for initialization
	void Start () {
	
		if(fontInGame != null) {
			// Make the font use the 'point' filter. This can't be done in the inspector
			fontInGame.material.mainTexture.filterMode = FilterMode.Point;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/* -----------------------------------------------------------------------------------------------------------
	 *                 
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Add noise made by the players to the 'noise meter'
	/// </summary>
	/// <param name="fNoise"> Value of the noise </param>
	public void AddNoise(float fNoise) {

		fNoiseMade += fNoise;
		fNoiseMade = Mathf.Clamp01(fNoiseMade);	

		// Update the HUD
		hudDudeScript.NoiseBarUpdate(fNoiseMade);
		hudDogScript.NoiseBarUpdate(fNoiseMade);
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

		bnDogEnteredTheGame = true;

		if(bnDudeEnteredTheGame) {

			// TODO: start game!
			Application.LoadLevel(Application.loadedLevel+1);
		}

	}
	/// <summary>
	/// On the start screen, the dog entered the game
	/// </summary>
	public void DudeEnteredTheGame() {

		bnDudeEnteredTheGame = true;

		if(bnDogEnteredTheGame) {

			// TODO: start game!
			Application.LoadLevel(Application.loadedLevel+1);
		}
	}
}
