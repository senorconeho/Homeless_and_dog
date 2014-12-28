using UnityEngine;
using System.Collections;

/// <summary>
/// Manage all sound effects from the game
/// </summary>
public class SoundEffectsManager : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC

	[Header("Items effects")]
	[SerializeField] public AudioClip	sfxItemPicked;	//< item picked by the player
	[SerializeField] public AudioClip	sfxItemDropped;	//< item dropped by the player
	[SerializeField] public AudioClip	sfxItemBurned;	//< item delivered in the fire barrel
	[SerializeField] public AudioClip	sfxItemCrashed;

	[Header("Dog effects")]
	[SerializeField] public AudioClip	sfxThrowedThroughTheWindow;	//< Dog throwed through the window
	[SerializeField] public AudioClip	sfxDogCatched;	//< Dog was catched by the apartment resident
	
	[Header("Environment effects")]
	[SerializeField] public AudioClip	sfxThunder;
	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

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
