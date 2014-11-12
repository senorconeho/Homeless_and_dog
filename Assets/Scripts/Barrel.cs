using UnityEngine;
using System.Collections;

/// <summary>
/// The barrel
/// Behaviour:
/// - when an burnable item touches it, it increases the fire
/// - when the player touches it, it shows it's health
/// </summary>
public class Barrel : MonoBehaviour {

	public float 			fFireHealth = 1;
	float 						fBurnTime;
	float 						fDefaultBurnTime;	//< How many seconds to burn and extinguish all the fire?
	float 						fBurnSpeed = 1;
	public Transform 	trFireFlame;
	Vector3 					vFireFlameStartPosition;

	MainGame 					gameScript;
	MusicManager			musicScript;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Awake () {

		// Get the scripts
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
		GameObject goMusicManager = GameObject.Find("MusicManager");

		if(goMusicManager != null)
			musicScript = goMusicManager.GetComponent<MusicManager>();

		fBurnTime = fDefaultBurnTime;
		fBurnSpeed = 1f/fBurnTime;

		if(trFireFlame != null) {
		
			vFireFlameStartPosition = trFireFlame.position;
		}
	}

	/// <summary>
	///
	/// </summary>
	void Start() {

		UpdateFlameHeight();
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		if(gameScript.GetCurrentGameStatus() != MainGame.eGameStatus.GAME_PLAY &&
				gameScript.GetCurrentGameStatus() != MainGame.eGameStatus.GAME_START_SCREEN)
			return;

		if(fFireHealth >= 1.0f && gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_PLAY)  {

			// Change the current game state: we won!
			gameScript.ChangeStatusToGameWonLevel();
		}
		else if(fFireHealth > 0) {
				
			fFireHealth -= Time.deltaTime * fBurnSpeed;
			fFireHealth = Mathf.Clamp01(fFireHealth);

			// Move the flame down
			UpdateFlameHeight();

			UpdateMusicPitchFromFireLevel();
		}
		else {

			gameScript.ChangeStatusToGameOver();
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * CLASS METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Adds 'health' to this fire
	/// </summary>
	public void UpdateFlameHeight() {

		if(trFireFlame != null) {

			trFireFlame.transform.position = new Vector3(vFireFlameStartPosition.x, 
					vFireFlameStartPosition.y + 0.16f * fFireHealth,
					0f);	
		}
	}
	
	/// <summary>
	/// Adds 'health' to this fire
	/// </summary>
	public void AddHealthToFire(float fHealthValue) {

		if(fFireHealth >= 0)
			fFireHealth += fHealthValue;

		fFireHealth = Mathf.Clamp01(fFireHealth);
	}

	/// <summary>
	///
	/// </summary>
	public void UpdateMusicPitchFromFireLevel() {

		if(musicScript != null) {

			// Only changes the music's pitch if the fire is lower than 50%
			if(fFireHealth < .5f) {

				float fNewPitch = 1 + (1-fFireHealth)/10; 
				musicScript.AdjustMusicPitch(fNewPitch);
			}
		}
	}

	/// <summary>
	/// Get the current fire health
	/// </summary>
	/// <returns> A float with the current fire health </returns>
	public float GetFireHealth() {

		return fFireHealth;
	}

	/// <summary>
	/// Restore the fire rate to it's default value
	/// </summary>
	public void RestoreDefaultFireRate() {

		fBurnTime = fDefaultBurnTime;
		// Updates the burn speed
		fBurnSpeed = 1f/fBurnTime;
	}

	/// <summary>
	/// Set the fire rate, changing the time to extinguish the fire
	/// </summary>
	/// <param name="fNewTime"> New time, in seconds, to burn all the fire </param>
	public void SetFireRate(float fNewTime) {

		fBurnTime = fNewTime;
		// Updates the burn speed
		fBurnSpeed = 1f/fBurnTime;

		if(fDefaultBurnTime == 0.0f)
			fDefaultBurnTime = fBurnTime;
	}

	/// <summary>
	/// <\summary>
	public float GetFireRate() {

		return fBurnTime;
	}

	/// <summary>
	/// Set this barrel's fire level
	/// </summary>
	/// <param name="fNewLevel">A float with the new level, between 0 and 1</param>
	public void SetFireLevel(float fNewLevel) {

		fNewLevel = Mathf.Clamp01(fNewLevel);
		fFireHealth = fNewLevel;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * COLLISION STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		if(col.gameObject.layer == MainGame.nPlayerLayer)
			Debug.Log(this.transform + " Health: " + fFireHealth * 100);
	}
}
