﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Behaviour of a room in the game
/// </summary>
public class Room : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public string				stRoomName;		//< Name of the room
	MainGame						gameScript;
	Transform						trWindow;			//< Window of the room
	Transform 					trWaypoint;
	Window							windowScript;	//< pointer to the window script
	public Transform[]	trResidentWaypoints;	//< Waypoints array

	// Resident stuff
	public Transform	trResidentSpawnPoint;				//< Object pointing where to generate a resident
	public Transform	trResidentPrefab;						//< Prefab of the resident itself
	Transform					trResident = null;
	float							fResidentMinTimeToAppear = 3.0f;		//< The resident timer works this way: the room will randomize a value between min and max. When the timer is over, the resident will swipe the room and disappear. The game will randomize a new value and so forth
	float							fResidentMaxTimeToAppear = 7.5f;
	public float			fResidentCountdownTimer;
	bool							bnResidentIn = false;	//< is the resident in the room?

	// Window stuff
	float							fReopenWindowMinTime = 5.0f;
	float							fReopenWindowMaxTime = 10.0f;
	public float			fReopenWindowTimer;

	/* ==========================================================================================================
	 * UNITY MAIN LOOP
	 * ==========================================================================================================
	 */

	/// <summary>
	/// </summary>
	void Awake() {

		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();

		trWindow = FindChildrenByTag("InsideWindow", this.transform);
		if(trWindow != null) {

			windowScript = trWindow.gameObject.GetComponent<Window>();
		}

		// HACK ALERT!!!!
		if(trResidentWaypoints.Length == 0) {
			// The waypoint was not populated by the user, so we gonna make the default
			// 1 - window, 2 - Left Side, 3 - SpawnPoint
			trWaypoint = FindChildrenByTag("Waypoint", this.transform);
			trResidentSpawnPoint = transform.Find("ResidentSpawn");
			trResidentWaypoints = new Transform[3];
			trResidentWaypoints[0] = trWindow;
			trResidentWaypoints[1] = trWaypoint;
			trResidentWaypoints[2] = trResidentSpawnPoint;
		}
	}
	
	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
		// Randomize the timer value
		fResidentCountdownTimer = Random.Range(fResidentMinTimeToAppear, fResidentMaxTimeToAppear);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
	
		TickTimers();
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// </summary>
	void TickTimers() {

		fReopenWindowTimer -= Time.deltaTime;

		if(bnResidentIn == true) {

			return;
		}

		fResidentCountdownTimer -= Time.deltaTime;

		if(fResidentCountdownTimer <= 0.0f) {

			// Regenerate the timer
			fResidentCountdownTimer = Random.Range(fResidentMinTimeToAppear, fResidentMaxTimeToAppear);
			EnableResident();
		}

	}

	/* -----------------------------------------------------------------------------------------------------------
	 * RESIDENT STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Enable the resident object
	/// </summary>
	public void EnableResident() {

		if(trResident == null) {

			// Generate the resident for the first time
			trResident = Instantiate(trResidentPrefab, trResidentSpawnPoint.position, trResidentPrefab.rotation) as Transform;
			trResident.gameObject.SetActive(false);
			ResidentBehaviour residentScript = trResident.gameObject.GetComponent<ResidentBehaviour>();
			residentScript.SetSpawnPoint(trResidentSpawnPoint);
			residentScript.SetRoomScript(this);
			trResident.gameObject.SetActive(true);

		}
		else {

			// Reactivate the object if it already exists
			trResident.gameObject.SetActive(true);
		}
		bnResidentIn = true;
	}

	/// <summary>
	/// Called from ResidentBehaviour
	/// </summary>
	public void ResidentReachedBackToSpawnPoint() {

		// Disable the resident object
		trResident.gameObject.SetActive(false);
		bnResidentIn = false;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * DOG STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	///
	/// </summary>
	public void DogCatched() {

		// Make the dog drop it's item
		gameScript.dogScript.DogCatched();
		// Wait a little
		// Close the window, so the dog can't enter back
		CloseWindow();
		// Make the dog appear outside the window
		gameScript.dogScript.ThrowTheDogOutOfTheWindow(trWindow);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * HELPER METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	///
	/// </summary>
	/// <returns>The transform of the window in this room</returns>
	public Transform GetWindowObject() {

		return trWindow;
	}

	/// <summary>
	/// Return the waypoint object
	/// </summary>
	/// <param name="nIndex">Index of the waypoint in the array</param>
	/// <returns>The transform of the waypoint at the index, or null if the index value is invalid</returns>
	public Transform GetWaypointObject(int nIndex) {

		if(nIndex > trResidentWaypoints.Length)
			return null;

		return trResidentWaypoints[nIndex];
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>The total number of waypoints on the array</returns>
	public int GetNumberOfWaypoints() {

		return trResidentWaypoints.Length;
	}

	/// <summary>
	/// The room makes the windows closes. Called from ResidentBehaviour when the dog is caught
	/// </summary>
	public void CloseWindow() {

		// Close this side...
		windowScript.CloseWindow();
		// ...and the other too ...
		windowScript.windowOtherSideScript.CloseWindow();
		// ...and starts the cooldown timer to reopen the window
		fReopenWindowTimer = Random.Range(fReopenWindowMinTime, fReopenWindowMaxTime);
	}

	/// <summary>
	/// The room makes the windows open again. Called from ResidentBehaviour when the window is opened again
	/// </summary>
	public void OpenWindow() {

		// Close this side...
		windowScript.OpenWindow();
		// ...and the other too ...
		windowScript.windowOtherSideScript.OpenWindow();
	}

	public bool CanTheWindowBeReopened() {

		// Is this window already open?
		if(windowScript.IsTheWindowOpen())
			return false;

		// No, it's closed. Check the 'cooldown' timer
		if(fReopenWindowTimer <= 0.0f)
			return true;

		// None of the above
		return false;
	}

	/// <summary>
	///
	/// </summary>
	public Transform FindChildrenByTag(string stTag, Transform tr) {
		// Get the window
		foreach(Transform child in tr) {

			if(child.gameObject.tag == stTag)
				return child;
		}

		return null;
	}
}