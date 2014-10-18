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

	// Item
	public AudioClip	sfxItemPicked;	//< item picked by the player
	public AudioClip	sfxItemDropped;	//< item dropped by the player
	public AudioClip	sfxItemBurned;	//< item delivered in the fire barrel
	public AudioClip	sfxItemCrashed;

	// Dog
	public AudioClip	sfxJump;							//< Jumped on the homeless lap
	public AudioClip	sfxWindowEnter;				//< Passed through a window
	public AudioClip	sfxWindowEnterDenied;	//< Trying to enter a closed window
	
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
