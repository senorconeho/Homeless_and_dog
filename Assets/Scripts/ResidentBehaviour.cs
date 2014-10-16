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
	public Transform	trWindow;	//< Window object of the room
	public Room	roomScript = null;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY MAIN LOOP
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void OnEnable() {

		if(roomScript != null) {

			trWindow = roomScript.GetWindowObject();
		}

		// The first target is the window
		trTarget = trWindow;

		if(movementScript != null) {

			int nDirection = CheckDirectionToTheTarget();
			movementScript.SetNPCMovementDirection(nDirection);	// Walk to the target
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

	/// <summary>
	/// Set the first variables of the resident. Called after the object is instantiated
	/// </summary>
	public void SetRoomScript(Room script) {

		roomScript = script;

		if(trWindow == null) {

			trWindow = roomScript.GetWindowObject();
			trTarget = trWindow;
		}
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
			movementScript.SetNPCMovementDirection(0);	// Stop
			GotTheDog(trTarget);
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
		int nDirection = CheckDirectionToTheTarget();
		movementScript.SetNPCMovementDirection(nDirection);	// Walk to the target
	}


	/// <summary>
	/// What to do when the dog get out of the field of view (escaped, perhaps?)
	/// </summary>
	public void LostTheDog(Transform trDog) {

		// The target now is the dog
		trTarget = trWindow;
	}

	/// <summary>
	///
	/// </summary>
	public void SpottedTheDog(Transform trDog) {

		// The target now is the dog
		trTarget = trDog;
	}

	/// <summary>
	///
	/// </summary>
	public void GotTheDog(Transform trDog) {

		roomScript.DogCatched();
	}

	/// <summary>
	/// Check of a target exist, and then return the direction to it
	/// </summary>
	/// <returns> 1 if the target is to the right, -1 if it is to the left, 0 if no target exists</returns>
	int CheckDirectionToTheTarget() {

		if(trTarget == null) 
			return 0;

		return (Math.Sign(trTarget.position.x - transform.position.x));
	}

	/// <summary>
	///
	/// </summary>
	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.01f);
	}
}
