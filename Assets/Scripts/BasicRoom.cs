using UnityEngine;
using System.Collections;

/// <summary>
/// Basic class for any kind of room, holds the name, screen limits
/// As 'room', we will assume an apartment, or even the outside street.
/// </summary>
public class BasicRoom : MonoBehaviour {

	public string	stRoomName;		//< Name of the room to be shown on the screen when the player enter it
	public Transform	trLeftLimit;
	public Transform	trRightLimit;

	// Use this for initialization
	public void Start () {

		// Find the limits of this room	
		trLeftLimit = this.transform.Find("Limits/LeftRoomLimit");
		trRightLimit = this.transform.Find("Limits/RightRoomLimit");
	}

	/// <summary>
	///
	/// </summary>
	/// <returns> </returns>
	public Transform GetLeftLimit() {

		return trLeftLimit;
	}

	/// <summary>
	///
	/// </summary>
	/// <returns> </returns>
	public Transform GetRightLimit() {

		return trRightLimit;
	}

	/// <summary>
	///
	/// </summary>
	public void EnteredRoom() {

	}
}
