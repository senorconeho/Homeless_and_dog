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
	public Player					pickedByScript;
	public BoxCollider2D	col;
	public bool						bnCrashed;					//< Have this item crashed on the ground?
	SpriteRenderer				sr;
	public float					fDroppedTimer;

	public MainGame.eItemTypes	itemType;			//<

	AudioClip	sfxItemPicked;	//< item picked by the player
	AudioClip	sfxItemDropped;	//< item dropped by the player
	AudioClip	sfxItemBurned;	//< item delivered in the fire barrel
	AudioClip	sfxItemCrashed;	//< Item crashed on the ground

	public Animation	clipPickAnimation;	//< animation to be played when this item is picked

	SoundEffectsManager sfxScript;
	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */

	void Awake() {

		// Get the sound effects
		sfxScript = GameObject.Find("GameManager").gameObject.GetComponent<SoundEffectsManager>();
		sfxItemPicked =		sfxScript.sfxItemPicked;	//< item picked by the player
		sfxItemDropped =	sfxScript.sfxItemDropped;	//< item dropped by the player
		sfxItemBurned =		sfxScript.sfxItemBurned;	//< item delivered in the fire barrel
		sfxItemCrashed =	sfxScript.sfxItemCrashed;	//< Item crashed on the ground
	}

	// Use this for initialization
	void Start () {
	
		col = GetComponent<BoxCollider2D>();
		sr = GetComponent<SpriteRenderer>();
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

			// Was somebody holding me?
			if(bnPickedUp) {

				if(trPickedBy != null && pickedByScript != null) {

					pickedByScript.BurnItem();
				}
			}
			
			// Play a sound
			if(sfxItemBurned != null) {

				audio.PlayOneShot(sfxItemBurned);
				// Disables the collider and sprite renderer, so the object doesn't affect the game until is destroyed
				col.enabled = false;
				sr.enabled = false;
				StartCoroutine(WaitAndThenDie(sfxItemBurned.length));
			}
			else {
				// Not playing anything?
				Die();
			}
		}
	}
	
	/// <summary>
	/// What to do when this item is picked by someone
	/// </summary>
	public void PickedUp(Transform trPicker, Player pickerScript) {

		if(clipPickAnimation != null) {

			// TODO: make the player wait until the end of animation
		}
		
		// If we're picked by the homeless dude, the object don't follow it, rather we player
		// another special animation
		if(pickerScript.playerType == MainGame.ePlayerType.DUDE) {

			// Disable the sprite renderer, because the item is already draw in the animation
			sr.enabled = false;
		}

		bnPickedUp = true;
		transform.tag = "Picked";
		trPickedBy = trPicker;
		pickedByScript = pickerScript;
		// disable all collisions
		//col.enabled = false;
		if(sfxItemPicked != null) {

			audio.PlayOneShot(sfxItemPicked);
		}
	}

	/// <summary>
	/// What to do when this item is dropped by someone
	/// </summary>
	public void Dropped(Transform trPicker) {

		if(pickedByScript.playerType == MainGame.ePlayerType.DUDE) {

			// Disable the sprite renderer, because the item is already draw in the animation
			sr.enabled = true;
		}

		bnPickedUp = false;
		transform.tag = "Item";
		trPickedBy = null;
		pickedByScript = null;
		// disable all collisions
		col.enabled = false;
		col.enabled = true;

		if(sfxItemDropped != null) {

			audio.PlayOneShot(sfxItemDropped);
		}

		// Count how much time this item spends falling
		StartDroppedTimer();
	}

	/// <summary>
	/// This item fell from some height into the ground (adding noise and crashing)
	/// </summary>
	public void Crashed() {

		if(!bnCrashed)
			bnCrashed = true;
	}

	/// <summary>
	/// Destroys this object
	/// </summary>
	void Die() {

		if(gameObject != null) {

			// FIXME: should I check if someone is holding me?
			Destroy(this.gameObject);
		}
	}

	/// <summary>
	///
	/// </summary>
	IEnumerator WaitAndThenDie(float fWaitTime) {

		yield return new WaitForSeconds(fWaitTime);
		Die();
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
		if(col.gameObject.layer == MainGame.nBarrelLayer) {
		
			TouchWithBarrel(col.gameObject);
		}
		// Collision with window (the inside window, i.e., the on inside the apartment 
		if(col.gameObject.layer == MainGame.nWindowsLayer && col.transform.tag == "WindowInside") {

			// DEBUG
			Debug.Log("Collision with window");
		}
	}

	/// <summary>
	/// Counts the time this object is falling from 'dropped' until hit the ground or is picked
	/// </summary>
	void StartDroppedTimer() {

		fDroppedTimer = Time.time;
	}

	/// <summary>
	///
	/// </summary>
	public void StopDroppedTimer() {

		fDroppedTimer = Time.time - fDroppedTimer;
	}

	public float GetDroppedTime() {

		return fDroppedTimer;
	}
}
