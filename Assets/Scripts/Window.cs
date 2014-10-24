using UnityEngine;
using System.Collections;

/// <summary>
/// Window logic
/// </summary>
public class Window : MonoBehaviour {

	Player						dogScript = null;
	MainGame					gameScript = null;
	public Transform	trWindowOtherSide;			//< The other 'side' of this window
	public Window			windowOtherSideScript;
	public Transform	trBasicRoom;
	public BasicRoom	basicRoomScript;	//< the basic room which this window belongs

	public enum eWindowStatusType {

		CLOSED,
		OPEN
	};

	public eWindowStatusType windowStatus;

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
		windowOtherSideScript = trWindowOtherSide.gameObject.GetComponent<Window>();
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

		// Update the window sprite
		if(windowStatus == eWindowStatusType.OPEN) {

			// Update the sprite
			spriteRenderer.sprite = spriteWindowOpen;
			// Enables the collider
			col.enabled = true ;
			// Enables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = true;
		}
		else if(windowStatus == eWindowStatusType.CLOSED) {

			// Update the sprite
			spriteRenderer.sprite = spriteWindowClosed;
			// Disables the collider
			col.enabled = false;
			// Disables the curtains
			spriteCurtains.enabled = false;
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
			spriteRenderer.sprite = spriteWindowClosed;
			// Disables the collider
			col.enabled = false;
			// Disables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = false;
		}
	}

	/// <summary>
	/// Open the window
	/// </summary>
	public void OpenWindow() {

		if(windowStatus == eWindowStatusType.CLOSED) {

			windowStatus = eWindowStatusType.OPEN;
			// Update the sprite
			spriteRenderer.sprite = spriteWindowOpen;
			// Enables the collider
			col.enabled = true;
			// Enables the curtains
			if(spriteCurtains != null)
				spriteCurtains.enabled = true;
		}
	}

	/// <summary>
	/// Change the sprite for the lit up representation
	/// </summary>
	public void LightTurnOn() {

		if(windowStatus == eWindowStatusType.CLOSED) {

			spriteRenderer.sprite = spriteWindowClosedLightOn;
		}
		else {
			spriteRenderer.sprite = spriteWindowOpenLightOn;
		}

		if(transform.tag == "OutsideWindow") {

			// Enable the reflection
			spriteLightOnReflectionOnWall.enabled = true;
		}
	}

	/// <summary>
	/// Change the sprite for the lit up representation
	/// </summary>
	public void LightTurnOff() {

		if(windowStatus == eWindowStatusType.CLOSED) {

			spriteRenderer.sprite = spriteWindowClosed;
		}
		else {
			spriteRenderer.sprite = spriteWindowOpen;
		}

		if(transform.tag == "OutsideWindow") {

			// Enable the reflection
			spriteLightOnReflectionOnWall.enabled = false;
		}
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
		if(trCol.transform.tag == "Dog" && dogScript != null) {

			dogScript.OverWindowExit(this.transform);
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
