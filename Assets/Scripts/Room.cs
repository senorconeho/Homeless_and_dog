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
	public Transform	trResidentGenerator;				//< Object pointing where to generate a resident
	public Transform	trResidentPrefab;						//< Prefab of the resident itself
	float			fResidentMinTimeToAppear = 3.0f;		//< The resident timer works this way: the room will randomize a value between min and max. When the timer is over, the resident will swipe the room and disappear. The game will randomize a new value and so forth
	float			fResidentMaxTimeToAppear = 7.5f;
	public float			fResidentCountdownTimer;

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

		trResidentGenerator = transform.Find("ResidentGenerator");
		
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

	}

}
