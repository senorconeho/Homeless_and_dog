using UnityEngine;
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
	public string			stRoomName;		//< Name of the room
	Transform					trWindow;			//< Window of the room
	Window						windowScript;	//< pointer to the window script

	// Resident stuff
	public Transform	trResidentSpawnPoint;				//< Object pointing where to generate a resident
	public Transform	trResidentPrefab;						//< Prefab of the resident itself
	Transform	trResident = null;
	float			fResidentMinTimeToAppear = 3.0f;		//< The resident timer works this way: the room will randomize a value between min and max. When the timer is over, the resident will swipe the room and disappear. The game will randomize a new value and so forth
	float			fResidentMaxTimeToAppear = 7.5f;
	public float			fResidentCountdownTimer;
	bool			bnResidentIn = false;	//< is the resident in the room?

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

		trWindow = FindChildrenByTag("InsideWindow", this.transform);
		if(trWindow != null) {

			windowScript = trWindow.gameObject.GetComponent<Window>();
		}

		trResidentSpawnPoint = transform.Find("ResidentSpawn");
		
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
		// Randomize the timer value
		fResidentCountdownTimer = Random.Range(fResidentMinTimeToAppear, fResidentMaxTimeToAppear);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
		TickTimers();
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>
	void TickTimers() {

		if(bnResidentIn == true)
			return;

		fResidentCountdownTimer -= Time.deltaTime;

		if(fResidentCountdownTimer <= 0.0f) {

			// Regenerate the timer
			fResidentCountdownTimer = Random.Range(fResidentMinTimeToAppear, fResidentMaxTimeToAppear);
			// TODO: generate the resident and make him swipe the room
			EnableResident();
		}
	}

	/// <summary>
	/// <\summary>
	public Transform FindChildrenByTag(string stTag, Transform tr) {
		// Get the window
		foreach(Transform child in tr) {

			if(child.gameObject.tag == stTag)
				return child;
		}

		return null;
	}

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

	/// <summary>
	///
	/// </summary>
	public Transform GetWindowObject() {

		return trWindow;
	}
}
