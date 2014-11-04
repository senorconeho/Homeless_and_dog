using UnityEngine;
using System.Collections;

/// <summary>
/// Class description
/// </summary>
public class PlayerSpawner : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	[SerializeField]	public Transform	dogPrefab;
	[SerializeField]	public Transform	dudePrefab;
	public MainGame.ePlayerType playerType;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

		if(playerType == MainGame.ePlayerType.DOG && dogPrefab != null) {

			// Instantiate the dog object in the game
			Instantiate(dogPrefab, transform.position, dogPrefab.rotation);
		}
		if(playerType == MainGame.ePlayerType.DUDE && dudePrefab != null) {

			// Instantiate the dog object in the game
			Instantiate(dudePrefab, transform.position, dudePrefab.rotation);
		}

	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>

}
