using UnityEngine;
using System.Collections;

/// <summary>
/// Main game script
/// </summary>
public class MusicManager : MonoBehaviour {

	public AudioClip	musicInGameSong;
	AudioSource				musicSource;
	public float 			fMusicPitch;
	MainGame					gameScript;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	///
	/// </summary>
	void Awake () {

		musicSource = GetComponent<AudioSource>();	
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
	
	}

	/* -----------------------------------------------------------------------------------------------------------
	 *                 
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void AdjustMusicPitch(float fNewPitch) {

		musicSource.pitch = fNewPitch;
	}
}
