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
	public string			stRoomName;
	Transform					trWindow;			//< Window of the room
	Window						windowScript;	//< pointer to the window script

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
		
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
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
	/// <\summary>
	public Transform FindChildrenByTag(string stTag, Transform tr) {
		// Get the window
		foreach(Transform child in tr) {

			if(child.gameObject.tag == stTag)
				return child;
		}

		return null;
	}

}
