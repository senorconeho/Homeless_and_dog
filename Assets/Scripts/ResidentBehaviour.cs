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

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

		// Get the movement script
		movementScript = GetComponent<SimpleMoveRigidBody2D>();
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
		if(movementScript != null) {

			movementScript.SetNPCMovementDirection(-1);
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// Makes the character stay a little at the window
	/// <\summary>
	IEnumerator WaitOnTheWindow(float fWaitTime) {

		movementScript.SetNPCMovementDirection(0);
		yield return new WaitForSeconds(fWaitTime);

		// Change movement direction
		movementScript.SetNPCMovementDirection(1);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		// Check if collided with an window
		if(col.gameObject.layer == MainGame.nWindowsLayer) {

			StartCoroutine(WaitOnTheWindow(1.5f));
		}
	}
}
