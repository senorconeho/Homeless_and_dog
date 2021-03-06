﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Window logic
/// </summary>
public class Window : MonoBehaviour {

	Player						dogScript = null;
	MainGame					gameScript = null;
	public Transform	trWindowOtherSide;			//< The other 'side' of this window
	[HideInInspector] public Window			windowOtherSideScript;
	[HideInInspector] public Transform	trBasicRoom;
	[HideInInspector] public BasicRoom	basicRoomScript;	//< the basic room which this window belongs
	public Animator animator;

	public enum eWindowStatusType {

		CLOSED,
		OPEN
	};

	public eWindowStatusType windowStatus;
	public bool bnDetectCollisionWithDog = true;	//< allow the dog to enter? Will be false on the first tutorial level

	[SerializeField]
	public Sprite			spriteWindowOpen;
	[SerializeField]
	public Sprite			spriteWindowClosed;
	[SerializeField]
	public Sprite			spriteWindowOpenLightOn;
	[SerializeField]
	public Sprite			spriteWindowClosedLightOn;

	SpriteRenderer		spriteRenderer;
	SpriteRenderer		spriteCurtains;	//< The curtains sprite
	SpriteRenderer		spriteLightOnReflectionOnWall;
	BoxCollider2D			col;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// </summary>
	void Awake() {

		spriteRenderer = GetComponent<SpriteRenderer>();
		// Set by room.cs
		//windowOtherSideScript = trWindowOtherSide.gameObject.GetComponent<Window>();
		col = GetComponent<BoxCollider2D>();
		spriteCurtains = transform.Find("Curtains").gameObject.GetComponent<SpriteRenderer>();

		// HACK!!!
		if(transform.tag == "OutsideWindow") {
			// Street: have only outside windows, and in it's hierarchy:
			// Street
			// - Balcony
			// -- Window
			// So we need to get the parent of our parent to reach the main object
			trBasicRoom = this.transform.parent.transform.parent;
			basicRoomScript = trBasicRoom.gameObject.GetComponent<BasicRoom>();

			animator = GetComponent<Animator>();

			// Outside window only
			Transform trLightOnWall = transform.Find("LightOnWall");

			if(trLightOnWall != null)
				spriteLightOnReflectionOnWall = trLightOnWall.gameObject.GetComponent<SpriteRenderer>();

			if(spriteLightOnReflectionOnWall != null)
				spriteLightOnReflectionOnWall.enabled = false;
		}
		else {

			trBasicRoom = this.transform.parent;
			basicRoomScript = trBasicRoom.gameObject.GetComponent<BasicRoom>();
		}
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
		dogScript = gameScript.dogScript; 

		if(animator != null) {

				animator.SetInteger("windowStatus", (int)windowStatus);
		}

		if(!bnDetectCollisionWithDog) {

			col.enabled = false;	// disable the collision detection with the dog
		}

		// Update the window sprite
		if(windowStatus == eWindowStatusType.OPEN) {

			if(bnDetectCollisionWithDog)
				col.enabled = true;

			// Enables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = true;

			// Set the animator state
			if(animator != null) {

				animator.SetInteger("windowStatus", (int)windowStatus);
			}
		}
		else if(windowStatus == eWindowStatusType.CLOSED) {

			// Disables the collider
			if(bnDetectCollisionWithDog)
				col.enabled = false;

			// Disables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = false;
			
			// Set the animator state
			if(animator != null) {

				animator.SetInteger("windowStatus", (int)windowStatus);
			}
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * 
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Check if the window is open, i.e., the dog can enter it
	/// </summary>
	/// <returns> A boolean if the window is open </returns>
	public bool IsTheWindowOpen() {

		return (windowStatus == eWindowStatusType.OPEN);
	}
	
	/// <summary>
	/// 
	/// </summary>
	public void CloseWindow() {

		if(windowStatus == eWindowStatusType.OPEN) {

			windowStatus = eWindowStatusType.CLOSED;
			// Update the sprite
			//spriteRenderer.sprite = spriteWindowClosed;
			// Disables the collider
			if(bnDetectCollisionWithDog)
				col.enabled = false;
			// Disables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = false;
			
			if(animator != null) {

				animator.SetInteger("windowStatus", (int)windowStatus);
			}
		}
	}

	/// <summary>
	/// Open the window
	/// </summary>
	public void OpenWindow() {

		if(windowStatus == eWindowStatusType.CLOSED) {

			windowStatus = eWindowStatusType.OPEN;
			// Update the sprite
		//	spriteRenderer.sprite = spriteWindowOpen;
			// Enables the collider
			if(bnDetectCollisionWithDog)
				col.enabled = true;

			// Enables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = true;

			if(animator != null) {

				animator.SetInteger("windowStatus", (int)windowStatus);
			}
		}
	}

	/// <summary>
	/// Change the sprite for the lit up representation. It only works for the outside window (no point
	/// drawing a lit window in the inside)
	/// </summary>
	public void LightTurnOn() {

		if(animator != null) {

			animator.SetBool("bnLightIsOn", true);
		}

		if(transform.tag == "OutsideWindow") {

			//if(windowStatus == eWindowStatusType.CLOSED) {

			//	spriteRenderer.sprite = spriteWindowClosedLightOn;
			//}
			//else {
			//	spriteRenderer.sprite = spriteWindowOpenLightOn;
			//}
			// Enable the reflection
			spriteLightOnReflectionOnWall.enabled = true;

		}
		else {

			// Inside window: call the script on the other side of the window
			windowOtherSideScript.LightTurnOn();
			
		}

	}

	/// <summary>
	/// Change the sprite for the lit up representation
	/// </summary>
	public void LightTurnOff() {

		if(animator != null) {

			animator.SetBool("bnLightIsOn", false);
		}

		//if(windowStatus == eWindowStatusType.CLOSED) {

		//	spriteRenderer.sprite = spriteWindowClosed;
		//}
		//else {
		//	spriteRenderer.sprite = spriteWindowOpen;
		//}

		if(transform.tag == "OutsideWindow") {

			// Enable the reflection
			spriteLightOnReflectionOnWall.enabled = false;

		}
		else {

			// Inside window: call the script on the other side of the window
			windowOtherSideScript.LightTurnOff();
			
		}
	}

	/// <summary>
	/// Called from Room.cs: set the other side window stuff
	/// </summary>
	public void SetupOtherSideWindow(Transform trWindow, Window script) {

		trWindowOtherSide = trWindow;
		windowOtherSideScript = script;
	}
	/* -----------------------------------------------------------------------------------------------------------
	 * PHYSICS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Someone entered the hit box trigger
	/// </summary>
	/// <param name="trCol"> The Transform of the object that entered the trigger</param>
	void CheckEnterTriggerHitBox(Transform trCol) {

		// Dog collisions with...
		if(trCol.transform.tag == "Dog") {

			if(dogScript == null)
				dogScript = gameScript.dogScript;

			if(dogScript != null && windowStatus != eWindowStatusType.CLOSED)
				dogScript.OverWindowEnter(this.transform, trWindowOtherSide);
		}
	}

	/// <summary>
	/// Someone exited the hit box trigger
	/// </summary>
	/// <param name="trCol"> The Transform of the object that exited the trigger</param>
	void CheckExitTriggerHitBox(Transform trCol) {

		// Dog exited collisions with...
		if(trCol.transform.tag == "Dog") {

			if(dogScript != null) {

				dogScript.OverWindowExit(this.transform);
			}
		}
	}

	/// <summary>
	///	Trigger entered
	/// </summary>
	public void OnTriggerEnter2D(Collider2D col) {

		CheckEnterTriggerHitBox(col.transform);
	}

	/// <summary>
	/// Trigger exited
	/// </summary>
	public void OnTriggerExit2D(Collider2D col) {

		CheckExitTriggerHitBox(col.transform);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * DEBUG STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */
	void OnDrawGizmos() {

		if(trWindowOtherSide != null) {

			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, trWindowOtherSide.transform.position);
		}
	}
}
