using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour {

	Player		dogScript = null;
	MainGame	gameScript = null;
	public Transform	trWindowOtherSide;

	// DEBUG STUFF

	// Use this for initialization
	void Start () {
	
		gameScript = GameObject.Find("Game").gameObject.GetComponent<MainGame>();
		dogScript = gameScript.dogScript; 
	}
	
	/// <summary>
	/// Someone entered the hit box trigger
	/// </summary>
	/// <param name="trCol"> The Transform of the object that entered the trigger</param>
	void CheckEnterTriggerHitBox(Transform trCol) {

		// Dog collisions with...
		if(trCol.transform.tag == "Dog") {

			if(dogScript == null)
				dogScript = gameScript.dogScript;

			if(dogScript != null)
				dogScript.OverWindowEnter(this.transform, trWindowOtherSide);
		}
	}

	/// <summary>
	/// Someone exited the hit box trigger
	/// </summary>
	/// <param name="trCol"> The Transform of the object that exited the trigger</param>
	void CheckExitTriggerHitBox(Transform trCol) {

		// Dog exited collisions with...
		if(trCol.transform.tag == "Dog" && dogScript != null) {

			dogScript.OverWindowExit(this.transform);
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		CheckEnterTriggerHitBox(col.transform);
	}

	/// <summary>
	///
	/// </summary>
	public void OnTriggerExit2D(Collider2D col) {

		CheckExitTriggerHitBox(col.transform);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * DEBUG STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	void OnDrawGizmos() {

		if(trWindowOtherSide != null) {

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, trWindowOtherSide.transform.position);
		}
	}

}
