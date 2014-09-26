using UnityEngine;
using System.Collections;

/// <summary>
/// Burnable item:
/// - adds to the fire
/// - can be picked-up by the player (as a dog or as a person)
/// <summary>
public class Item : MonoBehaviour {

	public bool		bnPickedUp = false;	//< Is this item picked by somebody?
	public float 	fBurnValue = 0.6f;	//< how much adds to the flame when dropped in the barrel (0..1 max)

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Item touched with barrel: add 'health' to the fire, destroy the item
	/// </summary>
	void TouchWithBarrel(GameObject goBarrel) {

		Barrel barrelScript = goBarrel.GetComponent<Barrel>();

		if(barrelScript != null) {

			barrelScript.AddHealthToFire(fBurnValue);
			Die();
		}
	}
	
	/// <summary>
	/// Check for collisions
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		if(col.gameObject.layer == MainGame.nItemsLayer)
			Debug.Log(this.transform + " Triggered by " + col.transform);
		if(col.gameObject.layer == MainGame.nPlayerLayer)
			Debug.Log(this.transform + " Health: " +  100);
		// Collision witg the barrel?
		if(col.gameObject.layer == MainGame.nBarrelLayer)
			TouchWithBarrel(col.gameObject);
	}

	/// <summary>
	/// Destroys this object
	/// </summary>
	void Die() {

		if(gameObject != null) {

			Destroy(this.gameObject);
		}
	}
}
