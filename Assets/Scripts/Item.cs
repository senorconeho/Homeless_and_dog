using UnityEngine;
using System.Collections;

/// <summary>
/// Burnable item:
/// - adds to the fire
/// - can be picked-up by the player (as a dog or as a person)
/// <summary>
public class Item : MonoBehaviour {

	public bool						bnPickedUp = false;	//< Is this item picked by somebody?
	public float 					fBurnValue = 0.6f;	//< how much adds to the flame when dropped in the barrel (0..1 max)
	public Transform 			trPickedBy;					//< Who picked us up?
	public BoxCollider2D	col;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// Use this for initialization
	void Start () {
	
		col = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void LateUpdate() {

		if(trPickedBy != null) {

			this.transform.position = trPickedBy.transform.position;
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
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
	/// What to do when this item is picked by someone
	/// </summary>
	public void PickedUp(Transform trPicker) {

		bnPickedUp = true;
		transform.tag = "Picked";
		trPickedBy = trPicker;
		// disable all collisions
		col.enabled = false;
	}

	/// <summary>
	/// What to do when this item is dropped by someone
	/// </summary>
	public void Dropped(Transform trPicker) {

		bnPickedUp = false;
		transform.tag = "Untagged";
		trPickedBy = null;
		// disable all collisions
		col.enabled = true;
	}

	/// <summary>
	/// Destroys this object
	/// </summary>
	void Die() {

		if(gameObject != null) {

			Destroy(this.gameObject);
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Check for collisions
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		// Collision with the barrel?
		if(col.gameObject.layer == MainGame.nBarrelLayer)
			TouchWithBarrel(col.gameObject);
	}
}
