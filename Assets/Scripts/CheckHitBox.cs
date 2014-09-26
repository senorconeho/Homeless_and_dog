using UnityEngine;
using System.Collections;

public class CheckHitBox : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CheckTriggerHitBox(Transform trCol) {

		if(this.transform.tag == "Dude" && trCol.tag == "Dog") {

			// Collision between the dog and the dude
			Debug.Log("Dude and Dog");
			// TODO: send an event or something like that for both actors
		}

	}

	/// <summary>
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		// Check if this collider have a parent
		if(col.transform.parent != null) {
			Debug.Log(this.transform.parent.transform + " Hitbox " + col.transform);
		}

		CheckTriggerHitBox(col.transform);
	}

}
