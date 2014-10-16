using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// The resident, when enabled, will walk through the room to the window
/// 1 - Walk to the the left until hit the window
/// </summary>
public class ResidentBehaviour : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	SimpleMoveRigidBody2D	movementScript;
	public Transform trTarget = null;
	public Transform trSpawnPoint = null;
	public Room	roomScript = null;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY MAIN LOOP
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void OnEnable() {

		trTarget = null;
		if(movementScript != null) {

			movementScript.SetNPCMovementDirection(-1);	// Walk to the left
		}
	}

	/// <summary>
	/// <\summary>
	void OnDisable() {

		trTarget = null;	// Clear the target
	}

	/// <summary>
	/// <\summary>
	void Awake() {

		// Get the movement script
		movementScript = GetComponent<SimpleMoveRigidBody2D>();
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
		if(trTarget != null) {
			// We have a target, so walk to it
			if(isEqual(transform.position.x, trTarget.position.x)) {

				CheckTarget();
			}
		}	
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	///
	/// </summary>
	/// <param name="tr">Transform of the spawn point object</param>
	public void SetSpawnPoint(Transform tr) {

		trSpawnPoint = tr;
	}

	public void SetRoomScript(Room script) {

		roomScript = script;
	}

	/// <summary>
	///
	/// </summary>
	void CheckTarget() {

		if(trTarget == null)
			return;

		// Have we reached a window?
		if(trTarget.tag == "InsideWindow") {
			// Now we go back to the spawn point
			trTarget = trSpawnPoint;
			StartCoroutine(WaitOnTheWindow(3));
		}
		else if(trTarget.tag == "Dog") {
			// Have we reached the dog
			// TODO: disable or destroy this object

		}
		else if(trTarget.tag == "SpawnPoint") {

			movementScript.SetNPCMovementDirection(0);	// Stop
			// We have reached the spawn point, so disable ourself
			roomScript.ResidentReachedBackToSpawnPoint();
		}
	}

	/// <summary>
	/// Makes the character stay a little at the window
	/// <\summary>
	IEnumerator WaitOnTheWindow(float fWaitTime) {

		movementScript.SetNPCMovementDirection(0);
		yield return new WaitForSeconds(fWaitTime);

		// Change movement direction
		movementScript.SetNPCMovementDirection(1);
	}

	/// <summary>
	///
	/// </summary>
	public void GotTheDog(Transform trDog) {

		// DEBUG
		Debug.Log("Got the dog!");
	}

	/// <summary>
	///
	/// </summary>
	public void SpottedAWindow(Transform trWindow) {
		// Spotted the window, so walk to it
		if(trTarget != null)
			return;	// we already have a target: or the window, or the dog!

		trTarget = trWindow;
	}

	/// <summary>
	///
	/// </summary>
	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.01f);
	}
	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	//public void OnTriggerEnter2D(Collider2D col) {

	//	// Check if collided with an window
	//	if(col.gameObject.layer == MainGame.nWindowsLayer) {

	//		StartCoroutine(WaitOnTheWindow(1.5f));
	//	}
	//}
}
