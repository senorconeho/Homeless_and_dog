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
	public float 			fBurnTime = 15;	//< How many seconds to burn all the fire?
	float 						fBurnSpeed = 1;
	public Transform 	trFireFlame;
	Vector3 					vFireFlameStartPosition;

	MainGame 					gameScript;
	MusicManager			musicScript;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Awake () {

		// Get the scripts
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
		GameObject goMusicManager = GameObject.Find("MusicManager");

		if(goMusicManager != null)
			musicScript = goMusicManager.GetComponent<MusicManager>();

		fBurnSpeed = 1f/fBurnTime;

		if(trFireFlame != null) {
		
			vFireFlameStartPosition = trFireFlame.position;
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		if(fFireHealth > 0) {
				
			fFireHealth -= Time.deltaTime * fBurnSpeed;
			fFireHealth = Mathf.Clamp01(fFireHealth);

			// Move the flame down
			if(trFireFlame != null) {
			
				trFireFlame.transform.position = new Vector3(vFireFlameStartPosition.x, 
						vFireFlameStartPosition.y + 0.16f * fFireHealth,
						0f);	
			}

			UpdateMusicPitchFromFireLevel();
		}
		else {

			gameScript.ChangeStatusToGameOver();
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
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		if(col.gameObject.layer == MainGame.nPlayerLayer)
			Debug.Log(this.transform + " Health: " + fFireHealth * 100);
	}
}
