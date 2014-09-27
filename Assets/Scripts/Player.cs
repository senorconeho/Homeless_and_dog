using UnityEngine;
using System.Collections;

/// <summary>
///
/// </summary>
public class Player : MonoBehaviour {

	public MainGame.ePlayerType playerType;	//< (from MainGame)
	public bool 								bnPickedUp = false;
	public GameHUD							hudScript;	//< The in-game HUD
	public Transform						trItemPicked;	//< Have we picked some item?
	public Transform						trItemOver;

	private Animator animator;
	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// 
	void Awake() {

		// Get the HUD script
		hudScript = GetComponent<GameHUD>();
	}

	// Use this for initialization
	void Start () {
	
		animator = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		// FIXME
		CheckInput();
	
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * COLLISION WITH ANOTHER PLAYER
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Dog over the dude
	/// </summary>
	public void OverDudeEnter() {

		// Dog with dude? 
		if(playerType == MainGame.ePlayerType.DOG) {

			// Updates the HUD
			hudScript.uiButtonALabel.text = "JUMP ON";
		}
	}

	/// <summary>
	/// Dog over the dude
	/// </summary>
	public void OverDudeExit() {

		// Dog with dude? 
		if(playerType == MainGame.ePlayerType.DOG) {

			// Updates the HUD
			hudScript.uiButtonALabel.text = "";
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * COLLISION WITH ITEMS PROCESSING
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// We are over some 
	/// <summary>
	public void OverItemEnter(Transform trItem) {

		// Do we have an item already?
		if(trItemPicked != null)
			return;

		trItemOver = trItem;
		// Updates the HUD
		hudScript.uiButtonBLabel.text = "PICK";
	}

	/// <summary>
	/// We are NOT over some item anymore
	/// <summary>
	/// <param name="trItem">The Transform of the item</param>
	public void OverItemExit(Transform trItem) {

		// Do we have an item already?
		if(trItemPicked != null)
			return;

		// Updates the HUD
		if(trItem == trItemOver) {

			hudScript.uiButtonBLabel.text = "";
			trItemOver = null;
		}
	}

	/// <summary>
	///
	/// </summary>
	void PickItem() {

		if(trItemOver != null && trItemPicked == null) {

			trItemPicked = trItemOver;
			trItemOver = null;

			// Updates the HUD
			hudScript.uiButtonBLabel.text = "DROP";

			// Change the animation
			if (animator != null) {

				animator.SetBool("bnPickedItem", true);
			}
			// Tell the item that we picked it up
			Item itemScript = trItemPicked.gameObject.GetComponent<Item>();
			itemScript.PickedUp(this.transform);
		}
	}

	/// <summary>
	///
	/// </summary>
	void DropItem() {

		if(trItemPicked == null) 
			return;

		// Tell the item that we are dropping it
		Item itemScript = trItemPicked.gameObject.GetComponent<Item>();
		itemScript.Dropped(this.transform);

		trItemPicked = null;

		// Change the animation
		if (animator != null) {

			animator.SetBool("bnPickedItem", false);
		}

		// Updates the HUD
		hudScript.uiButtonBLabel.text = "";
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * INPUT: MOVE TO ANOTHER SCRIPT, PLEASE!
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Check the player input
	/// <summary>
	void CheckInput() {

		if(Input.GetKeyUp(KeyCode.K)) { // Dog Button B

			// Are we holding an item?
			if(trItemPicked != null) {
				// Drop it
				DropItem();
			}
			else if(trItemOver != null) {
				// Pick the item
				PickItem();
			}
		}
	}
}
