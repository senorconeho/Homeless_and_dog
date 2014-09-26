using UnityEngine;
using System.Collections;

/// <summary>
/// Main game script
/// </summary>
public class MainGame : MonoBehaviour {

	// Game layers
	static public int	nPlayerLayer = 9;
	static public int	nItemsLayer = 10;
	static public int	nBarrelLayer = 11;

	// Players types
	public enum ePlayerType {

		DOG,
		DUDE
	};

	public enum eGameStatus {

		GAME_PLAY,
		GAME_OVER
	};

	public eGameStatus gameStatus = eGameStatus.GAME_PLAY;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
