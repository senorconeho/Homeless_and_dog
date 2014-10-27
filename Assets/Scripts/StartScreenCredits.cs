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
	public string stStartMessage = "- Press Start -";

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
	
	/// <summary>90610-280
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
		// Randomize where to start showing the credits
		nCreditIdx = Random.Range(0, stCredits.Length-1);

		StartCoroutine(WaitAndChangeCredits(fChangeTextTime));
	}
	
	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>
	IEnumerator WaitAndChangeCredits(float fWaitTime) {

		string stText = stStartMessage;
		bool bnShowStartMessage = true;

		while(true) {

			if(gameScript.hudDudeScript == null || gameScript.hudDogScript == null)
				continue;

			gameScript.hudDudeScript.SetBottomScreenText(stText);
			gameScript.hudDogScript.SetBottomScreenText(stText);
			yield return new WaitForSeconds(fWaitTime);

			bnShowStartMessage =!bnShowStartMessage;

			if(!bnShowStartMessage) {
				stText = stCredits[nCreditIdx];

				// Skip to the next credit
				nCreditIdx++;
				if(nCreditIdx > stCredits.Length-1)
					nCreditIdx = 0;
			}
			else
				stText = stStartMessage;

		}
	}

}
