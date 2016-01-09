using UnityEngine;
using System.Collections;

/// <summary>
/// Class description
/// </summary>
public class StartScreenCredits : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public string[] stCredits = {
		"Art & Design: Ricardo Bencz", 
		"Programming & Design: Alexandre Ramos", 
		"Music: ??",
		"2014"
	};
	public string stStartMessageDog = "- Press <enter> -";		//FIXME
	public string stStartMessageDude = "- Press <spacebar> -"; //FIXME

	MainGame	gameScript;
	float	fChangeTextTime = 4.5f;
	int nCreditIdx = 0;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */


	/// <summary>
	/// <\summary>
	void Awake() {

		// Get the main game object
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
		// Randomize where to start showing the credits
		nCreditIdx = Random.Range(0, stCredits.Length-1);

		StartCoroutine(SetupCameras()); // Need to wait a little because the MainGame script forces the camera to the players
		StartCoroutine(DelayedStart());
	}

	IEnumerator DelayedStart() {

			yield return new WaitForSeconds(0.5f);
			StartCoroutine(WaitAndChangeCredits(fChangeTextTime));
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>
	IEnumerator SetupCameras() {

		// FIXME
		yield return new WaitForSeconds(.150f);

		// Center the camera on the barrel
		Transform trBarrel = gameScript.GetBarrel();

		gameScript.dogCameraScript.SetCameraTarget(trBarrel);
		gameScript.dogCameraScript.FocusCameraOnTarget();
		gameScript.dudeCameraScript.SetCameraTarget(trBarrel);
		gameScript.dudeCameraScript.FocusCameraOnTarget();
	}
	
	/// <summary>
	/// <\summary>
	IEnumerator WaitAndChangeCredits(float fWaitTime) {

		string stTextDog = stStartMessageDog;
		string stTextDude = stStartMessageDude;
		bool bnShowStartMessage = true;

		while(true) {

			if(gameScript.hudDudeScript == null || gameScript.hudDogScript == null) {
				continue;
			}

			if(gameScript.hudDudeScript != null && gameScript.hudDogScript != null) {
				gameScript.hudDudeScript.SetBottomScreenText(stTextDude);
				gameScript.hudDogScript.SetBottomScreenText(stTextDog);
			}
				yield return new WaitForSeconds(fWaitTime);

				bnShowStartMessage =!bnShowStartMessage;

				if(!bnShowStartMessage) {
					stTextDude = stCredits[nCreditIdx];
					stTextDog = stCredits[nCreditIdx];

					// Skip to the next credit
					nCreditIdx++;
					if(nCreditIdx > stCredits.Length-1)
						nCreditIdx = 0;
				}
				else {
					stTextDog = stStartMessageDog;
					stTextDude = stStartMessageDude;
				}
			}
		}
}
