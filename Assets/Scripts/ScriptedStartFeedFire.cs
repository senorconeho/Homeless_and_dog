using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Trying to emulate the Super Mario World intro, where the characters show all moves and mechanics 
/// of the game
/// Scene setup:
/// - the screen will have the fire barrel in the center of screen
/// - will have a item in each side of the screen. When the item is picked, another one will immediately
///	appear
/// - The dog and the homeless will be placed each at one side of the barrel
/// Flow:
/// - Fire starts at 100%.
/// - When it drop belows 90%, each character will move to the item and pick it up;
/// - the character will move towards the barrel until the item is deployed;
/// - the character will move back to the start point and turn itself to the barrel
/// - Repeat
/// </summary>
public class ScriptedStartFeedFire : MonoBehaviour {

	public Transform	trBarrel;
	Barrel			barrelScript;
	MainGame		gameScript;

	public Transform	prefabItem;
	public Transform 	trSpawnPoint;
	public Transform	trItemPoint;

	public Transform	trCharacter;	//< Dog or dude
	Player	playerScript;
	
	// From the ResidentBehaviour script
	SimpleMoveRigidBody2D		movementScript;
	public Transform 				trTarget = null;

	public int 							nWaypointIndex = 0;	//< Which waypoint are we?
	public Transform[]			trWaypoints;


	/* -----------------------------------------------------------------------------------------------------------
	 * MAIN UNITY LOOP
	 * ------------------------------------------------------------------k-----------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	void Awake() {

		// Get the main game object
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();

		trBarrel = GameObject.Find("Barrel").transform;
		if(trBarrel != null) {

			barrelScript = trBarrel.gameObject.GetComponent<Barrel>();
		}

		if(trCharacter != null) {

			playerScript = trCharacter.gameObject.GetComponent<Player>();
			movementScript = trCharacter.gameObject.GetComponent<SimpleMoveRigidBody2D>();
		}
	}


	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
		// Create the character path
		trWaypoints =  new Transform[] { trItemPoint, trBarrel, trSpawnPoint };

	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
	
		if(barrelScript.GetFireHealth() < 0.9f) {
			GetAnItemAndFeedTheFire();
		}
			if(trTarget != null) {
				// We have a target, so walk to it
				CheckCurrentTarget();
			}
	}

	void CheckCurrentTarget() {

		if(trTarget == null)
			return;

		if(trTarget.tag == "Item") {	// 1 - find and pick the item

			// Reached the item
			if(playerScript.trItemOver != null) {

				movementScript.SetNPCMovementDirection(0);
				// Pick the item...
				playerScript.PickItem();
				// Pick an item? Regenerate!
				GenerateItem();
				// ... and proceed to the the next target: the barrel
				trTarget = GetWaypointObject(++nWaypointIndex);
				SetMovementToTarget();
			}
		}
		else if(trTarget.tag == "Barrel") {	// 2 - Bring the item to the fire

			if(playerScript.trItemPicked == null) {

				trTarget = GetWaypointObject(++nWaypointIndex);
				SetMovementToTarget();
			}
		}
		else if(trTarget.tag == "SpawnPoint") { // Move back to the start point

			if(isEqual(trCharacter.transform.position.x, trTarget.transform.position.x)) {
				movementScript.SetNPCMovementDirection(0);	// Stop

				// resets the target and waypoint
				nWaypointIndex = 0;
				trTarget = null;	
			}
		}
	}

	/// <summary>
	/// Return the waypoint object
	/// </summary>
	/// <param name="nIndex">Index of the waypoint in the array</param>
	/// <returns>The transform of the waypoint at the index, or null if the index value is invalid</returns>
	public Transform GetWaypointObject(int nIndex) {

		if(nIndex > trWaypoints.Length)
			return null;

		return trWaypoints[nIndex];
	}

	/// <summary>
	///  Return the number of the waypoints in the room
	/// </summary>
	/// <returns>The total number of waypoints on the array</returns>
	public int GetNumberOfWaypoints() {

		return trWaypoints.Length;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * MOVEMENT
	 * -----------------------------------------------------------------------------------------------------------
	 */

	void GetAnItemAndFeedTheFire() {

		if(trTarget != null)
			return;

		trTarget = trWaypoints[0];

		if(movementScript != null) {

			int nDirection = CheckDirectionToTheTarget();
			movementScript.SetNPCMovementDirection(nDirection);	// Walk to the target
		}
	}

	/// <summary>
	/// Makes the character stay a little at the current position
	/// </summary>
	IEnumerator WaitHere(float fWaitTime) {

		movementScript.SetNPCMovementDirection(0);
		yield return new WaitForSeconds(fWaitTime);

		SetMovementToTarget();
	}

	/// <summary>
	///
	/// </summary>
	void SetMovementToTarget() {

		// Change movement direction
		int nDirection = CheckDirectionToTheTarget();
		movementScript.SetNPCMovementDirection(nDirection);	// Walk to the target
	}

	/// <summary>
	/// Check of a target exist, and then return the direction to it
	/// </summary>
	/// <returns> 1 if the target is to the right, -1 if it is to the left, 0 if no target exists</returns>
	int CheckDirectionToTheTarget() {

		if(trTarget == null) 
			return 0;

		return (Math.Sign(trTarget.position.x - trCharacter.position.x));
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * HELPER FUNCTIONS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	void GenerateItem() {

		if(prefabItem != null) {

			Transform tr = Instantiate(prefabItem, trItemPoint.position, prefabItem.transform.rotation) as Transform;
			trItemPoint = tr;
			trWaypoints[0] = trItemPoint;
		}
	}

	/// <summary>
	///
	/// </summary>
	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.01f);
	}
}
