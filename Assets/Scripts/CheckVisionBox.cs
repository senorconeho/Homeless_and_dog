using UnityEngine;
using System.Collections;

/// <summary>
/// Class description
/// </summary>
public class CheckVisionBox : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	ResidentBehaviour	residentScript = null;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

		residentScript = this.transform.parent.transform.gameObject.GetComponent<ResidentBehaviour>();
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
	}
	
	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		// Check if we hit the dog
		if(col.transform.tag == "Dog") {
			// DEBUG
			Debug.Log("STARTED Collision with the dog");
			// Gotcha!
			if(residentScript != null) {
				Debug.Log("Spotted");
				//residentScript.SpottedTheDog(col.transform);
				// FIXME
				residentScript.GotTheDog(col.transform);
			}
		}
		//else if(col.transform.gameObject.layer == MainGame.nWindowsLayer) {

		//	if(residentScript != null)
		//		residentScript.SpottedAWindow(col.transform);
		//}
	}

	/// <summary>
	///
	/// </summary>
	public void OnTriggerExit2D(Collider2D col) {

		// Check if we hit the dog
		if(col.transform.tag == "Dog") {
			// Gotcha!
			Debug.Log("EXITED Collision with the dog");
			if(residentScript != null) {

				Debug.Log("Lost");
				residentScript.LostTheDog(col.transform);
			}
		}
	}

}
