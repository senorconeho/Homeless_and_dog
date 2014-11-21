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
public class ScriptedFeedFire : MonoBehaviour {

	MainGame								gameScript;		//< Main game script
	Transform								trBarrel;			//< Transform of the barrel (placed on the center of the screen)
	Barrel									barrelScript;	//< Script of the barrel (to check the fire level)

	public Transform 				trSpawnPoint;				//< where the character starts and wait until the fire drop it's level
	public Transform				trItem;							//< put the item here. The next one will be generated in the same place
	ItemSpawner							itemSpawnerScript;	//< script of the item spawner object

	Transform								trCharacter;				//< Dog or dude
	Player									playerScript;				//< player script
	SimpleMoveRigidBody2D		movementScript;			//< movement script
	public Transform 				trTarget = null;		//< current target	

	public Transform[]			trWaypoints;				//< Array of waypoints (will be populated in the code)
	public int 							nWaypointIndex = 0;	//< Index of the current waypoint

	/* -----------------------------------------------------------------------------------------------------------
	 * MAIN UNITY LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	void Start() {

		// Get the main game object
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();

		// Find the 'barrel'
		trBarrel = gameScript.GetBarrel();
		if(trBarrel != null) {

			barrelScript = trBarrel.gameObject.GetComponent<Barrel>();
		}

		trCharacter = this.transform;
		playerScript = GetComponent<Player>();
		movementScript = GetComponent<SimpleMoveRigidBody2D>();
		trSpawnPoint = playerScript.trSpawnPoint;

		GameObject goItemSpawner = FindNearestItemSpawner();
		itemSpawnerScript = goItemSpawner.GetComponent<ItemSpawner>();

		// Generate the item
		//itemSpawnerScript.GenerateItem();
		//trItem = itemSpawnerScript.GetItemGenerated();
		trItem = itemSpawnerScript.GenerateNewItem();

		// Create the character path
		trWaypoints =  new Transform[] { trItem, trBarrel, trSpawnPoint };
	}

	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
	
		if(barrelScript.GetFireHealth() < 0.9f) {	// FIXME

			GetAnItemAndFeedTheFire();
		}

		if(trTarget != null) {
			// We have a target, so walk to it
			CheckCurrentTarget();
		}
	}

	/// <summary>
	/// Check which target we have reached and what to do next
	/// </summary>
	void CheckCurrentTarget() {

		if(trTarget == null)
			return;

		if(trTarget.tag == "Item") {	// 1 - find and pick the item

			// Reached the item
			if(playerScript.trItemOver != null) {

				movementScript.SetNPCMovementDirection(0);
				// Pick the item...
				playerScript.PickItem();
				// Picked an item? Regenerate!
				trItem = itemSpawnerScript.GenerateNewItem();
				// Update the waypoint array
				trWaypoints[0] = trItem;

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
				movementScript.HaltCharacter();
				movementScript.FaceObject(trBarrel);

				// resets the target and waypoint
				nWaypointIndex = 0;
				trTarget = null;	

				// TODO: make the NPC enjoy the fire
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

	/// <summary>
	/// Pick the item and walk torwards the fire barrel
	/// </summary>
	void GetAnItemAndFeedTheFire() {

		if(trTarget != null)
			return;

		// Reset the target to the first waypoint
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
	/// Calculate the direction of movement and sets the character on motion
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
	
	/// <summary>
	/// </summary>
	GameObject FindNearestItemSpawner() {

		GameObject[] goItemSpawnPoints;
		float fDistance = Mathf.Infinity;
		GameObject goNearest = null;

		goItemSpawnPoints = GameObject.FindGameObjectsWithTag("ItemSpawn");
		
		foreach(GameObject go in goItemSpawnPoints) {

			float fCurrentDistance = (go.transform.position - this.transform.position).sqrMagnitude;

			if(fCurrentDistance < fDistance) {

				fDistance = fCurrentDistance;
				goNearest = go;
			}
		}

		return goNearest;
	}

	/// <summary>
	///
	/// </summary>
	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.01f);
	}
}
