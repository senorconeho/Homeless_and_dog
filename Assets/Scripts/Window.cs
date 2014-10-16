using UnityEngine;
using System.Collections;

/// <summary>
/// Window logic
/// </summary>
public class Window : MonoBehaviour {

	Player						dogScript = null;
	MainGame					gameScript = null;
	public Transform	trWindowOtherSide;		//< The other 'side' of this window
	public Window			windowOtherSideScript;

	public enum eWindowStatusType {

		CLOSED,
		OPEN
	};

	public eWindowStatusType windowStatus;

	public Sprite			spriteWindowOpen;
	public Sprite			spriteWindowClosed;

	SpriteRenderer		spriteRenderer;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// </summary>
	void Awake() {

		spriteRenderer = GetComponent<SpriteRenderer>();
		windowOtherSideScript = trWindowOtherSide.gameObject.GetComponent<Window>();
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
		}
		else if(windowStatus == eWindowStatusType.CLOSED) {

			// Update the sprite
			spriteRenderer.sprite = spriteWindowClosed;
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
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public void OpenWindow() {

		if(windowStatus == eWindowStatusType.CLOSED) {

			windowStatus = eWindowStatusType.OPEN;
			// Update the sprite
			spriteRenderer.sprite = spriteWindowOpen;
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
