﻿using UnityEngine;
using System.Collections;

/// <summary>
///	Check the hit (trigger) between the various actors in the game
/// </summary>
public class CheckHitBox : MonoBehaviour {

	Player	playerScript = null;
	BoxCollider2D col;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	//
	void Awake() {

		col = GetComponent<BoxCollider2D>();

		// Get the player script, if any
		if(this.transform.parent.transform.gameObject.layer == MainGame.nPlayerLayer) {

			playerScript = this.transform.parent.transform.gameObject.GetComponent<Player>();
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * TRIGGERS PROCESSING
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Someone entered the hit box trigger
	/// </summary>
	/// <param name="trCol"> The Transform of the object that entered the trigger</param>
	void CheckEnterTriggerHitBox(Transform trCol) {

		// Dog collisions with...
		if(this.transform.tag == "Dog" || this.transform.tag == "Dude") {

			// ... an item
			if(trCol.gameObject.layer == MainGame.nItemsLayer) {
				
				// is this item not picked yet?
				if(trCol.tag != "Picked") {

					playerScript.OverItemEnter(trCol);
				}
				
			}
			else if(trCol.tag == "Dude") {
				// Over the Homeless Dude
				playerScript.OverDudeEnter();
			}
		}

		// Check if entered the barrel's heat radius
		if(trCol.gameObject.layer == MainGame.nBarrelLayer && trCol.tag == "BarrelHeat") {

			playerScript.SetAroundTheFire(true);
		}
	}

	/// <summary>
	/// Someone exited the hit box trigger
	/// </summary>
	/// <param name="trCol"> The Transform of the object that exited the trigger</param>
	void CheckExitTriggerHitBox(Transform trCol) {

		// Dog exited collisions with...
		if(this.transform.tag == "Dog" || this.transform.tag == "Dude") {

			// ... an item
			if(trCol.gameObject.layer == MainGame.nItemsLayer) {
			
				// is this item not picked yet?
				if(trCol.tag != "Picked") {

					playerScript.OverItemExit(trCol);
				}
			}
			else if(trCol.tag == "Dude") {
				// Over the Homeless Dude
				playerScript.OverDudeExit();
			}
		}

		// Check if exited the barrel's heat radius
		if(trCol.gameObject.layer == MainGame.nBarrelLayer && trCol.tag == "BarrelHeat") {

				playerScript.SetAroundTheFire(false);
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void RefreshCollider() {

		col.enabled = false;
		col.enabled = true;
	}


	/// <summary>
	///
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		CheckEnterTriggerHitBox(col.transform);
	}

	/// <summary>
	///
	/// </summary>
	public void OnTriggerExit2D(Collider2D col) {

		CheckExitTriggerHitBox(col.transform);
	}
}
