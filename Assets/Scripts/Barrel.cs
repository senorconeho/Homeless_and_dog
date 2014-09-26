using UnityEngine;
using System.Collections;

/// <summary>
/// The barrel
/// Behaviour:
/// - when an burnable item touches it, it increases the fire
/// - when the player touches it, it shows it's health
/// </summary>
public class Barrel : MonoBehaviour {

	public float fFireHealth = 1;
	public float fBurnTime = 15;	//< How many seconds to burn all the fire?
	float fBurnSpeed = 1;
	public Transform trFireFlame;
	Vector3 vFireFlameStartPosition;

	MainGame gameScript;

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Awake () {

		gameScript = GameObject.Find("Game").gameObject.GetComponent<MainGame>();
		fBurnSpeed = 1f/fBurnTime;

		if(trFireFlame != null) {
		
			vFireFlameStartPosition = trFireFlame.position;
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		if(fFireHealth >= 0) {
				
			fFireHealth -= Time.deltaTime * fBurnSpeed;
			fFireHealth = Mathf.Clamp01(fFireHealth);

			// Move the flame down
			if(trFireFlame != null) {
			
				trFireFlame.transform.position = new Vector3(vFireFlameStartPosition.x, 
						vFireFlameStartPosition.y + 0.16f * fFireHealth,
						0f);	
			}
		}
		else {

			gameScript.gameStatus = MainGame.eGameStatus.GAME_OVER;
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
	public void OnTriggerEnter2D(Collider2D col) {

		if(col.gameObject.layer == MainGame.nPlayerLayer)
			Debug.Log(this.transform + " Health: " + fFireHealth * 100);
	}
}
