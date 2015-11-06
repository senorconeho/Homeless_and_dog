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

	// Possible game status
	public enum eGameStatus {

		GAME_START_SCREEN, 
		GAME_PLAY,				// Game executing
		GAME_PAUSE,				// Game paused: ignore input from the player
		GAME_WON_LEVEL_SCREEN,
		GAME_OVER
	};

	public eGameStatus gameStatus = eGameStatus.GAME_START_SCREEN;

	// Possible item types
	public enum eItemTypes {
	
		ITEM_DOG,		//< For animation purposes, the dog is an item too (it can be carryed by the dude)	
		ITEM_CHAIR,
		ITEM_GARBAGE,
		ITEM_LAMP,
		ITEM_PILLOW_BOOK_PAPER,
		ITEM_SHOES,
		ITEM_FROZEN_DUDE
	}

	public Transform 		trDog;
	public Transform		trDude;

	[HideInInspector] public Player				dudeScript;
	[HideInInspector] public Player				dogScript;
	[HideInInspector] public GameHUD			hudDudeScript;
	[HideInInspector] public GameHUD			hudDogScript;
	[HideInInspector] public bool					bnDogEnteredTheGame = false;
	[HideInInspector] public bool					bnDudeEnteredTheGame = false;

	public float				fNoiseMade = 0.0f;

	public Font					fontInGame;

	public Transform		trDogCamera;
	public Transform		trDudeCamera;
	public CameraFollowTarget2D dogCameraScript;
	public CameraFollowTarget2D dudeCameraScript;

	PlayerSpawner	playerSpawnerScript;
	LevelControl	levelControlScript;

	public Color[] gameboyColorPalette = new Color[] { 
		new Color(8/255f, 24/255f, 32/255f),
		new Color(48/255f, 104/255f, 80/255f),
		new Color(136/255f, 195/255f, 112/255f),
		new Color(224/255f, 248/255f, 208/255f)
 	};

	[Header("Items stuff")]
	public float	fItemDroppedTimeThreshold = 0.3f;	//< for how long the item must fall to be considered 'crashed' when touching the ground

	public static MainGame instance;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */

	void Awake() {

		instance = this;

		// Find the game cameras
		trDogCamera = transform.Find("/Cameras/CameraDog");
		trDudeCamera = transform.Find("/Cameras/CameraDude");

		dogCameraScript = trDogCamera.gameObject.GetComponent<CameraFollowTarget2D>();
		dudeCameraScript = trDudeCamera.gameObject.GetComponent<CameraFollowTarget2D>();

		playerSpawnerScript = GetComponent<PlayerSpawner>();

		if(fontInGame != null) {
			// Make the font use the 'point' filter. This can't be done in the inspector
			fontInGame.material.mainTexture.filterMode = FilterMode.Point;
			fontInGame.material.color = gameboyColorPalette[3];
		}
	}

	// Use this for initialization
	void Start () {
	
		// Spawn the players in the game
		playerSpawnerScript.SpawnPlayers();
		StartCoroutine(SetupDog());
		StartCoroutine(SetupDude());
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
	/// </summary>
	public void ChangeStatusToGameWonLevel() {

		// Change the status
		gameStatus = eGameStatus.GAME_WON_LEVEL_SCREEN;
		// Activate the message on the screens
		dudeScript.ActivateGameWonLevel();
		dogScript.ActivateGameWonLevel();

		// Stop the rain effects
		levelControlScript.StopThunderEffects();
	}

	/// <summary>
	///	Return the current game status
	/// </summary>
	public eGameStatus GetCurrentGameStatus() {

		return gameStatus;
	}

	/// <summary>
	/// Return the next level to be played, relative to the current level
	/// </summary>
	/// <returns> </returns>
	public int GetNextLevel() {

		// FIXME
		int nNextLevel = 2;

		return nNextLevel;
	}

	/// <summary>
	/// Supposedly we already have the scripts, cameras, etc
	/// </summary>
	IEnumerator SetupDog() {

		// FIXME
		yield return new WaitForSeconds(.050f);

		dogCameraScript.SetCameraTarget(trDog);
		dogScript.RegisterCamera(trDogCamera, dogCameraScript);
	}

	/// <summary>
	/// Supposedly we already have the scripts, cameras, etc
	/// </summary>
	 IEnumerator SetupDude() {

		// FIXME
		yield return new WaitForSeconds(.050f);
		dudeCameraScript.SetCameraTarget(trDude);
		dudeScript.RegisterCamera(trDudeCamera, dudeCameraScript);
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

	/// <summary>
	/// Return the object that holds the spawner for a particular type of player. Useful to use it as waypoint
	/// or character placement in game
	/// </summary>
	/// <param name="playerType">An ePlayerType enum with the type of the player</param>
	/// <returns></returns>
	public Transform GetPlayerSpawner(ePlayerType playerType) {

		Transform rv = null;

		if(playerType == ePlayerType.DOG)
			rv = playerSpawnerScript.GetDogSpawnPoint();
		if(playerType == ePlayerType.DUDE)
			rv = playerSpawnerScript.GetDudeSpawnPoint();

		return rv;
	}
	/// <summary>
	/// Called from LevelControl: register itself with the main game manager
	/// </summary>
	public void RegisterLevelController(Transform trLevelController, LevelControl levelControllerScript) {

		levelControlScript = levelControllerScript;
	}

	public Transform GetBarrel() {

		return levelControlScript.GetBarrel();
	}

	/// <summary>
	/// Check if the dude is frozen
	/// </summary>
	public bool IsTheDudeFrozen() {

		return (dudeScript.FSMGetCurrentState() == Player.eFSMState.FROZEN);
	}
}
