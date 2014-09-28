using UnityEngine;
using System.Collections;

/// <summary>
///
/// </summary>
public class Player : MonoBehaviour {

	public MainGame.ePlayerType		playerType;	//< (from MainGame)
	public bool										bnPickedUp = false;
	public bool										bnCarryingItem = false;
	public GameHUD								hudScript;	//< The in-game HUD
	public Transform							trItemPicked;	//< Have we picked some item?
	public Transform							trItemOver;
	public SimpleMoveRigidBody2D	movementScript;

	private Animator animator;

	float		fCarryingItemSpeed = 0.5f;
	float		fRunningSpeed = 1.0f;
	
	/// Enumeration with all the possible states
	public enum eFSMState { 
		IDLE,										// 0
		RUNNING,								// 1 - Walking, actually
		IDLE_CARRYING_ITEM,			// 2 - 
		RUNNING_CARRYING_ITEM,	// 3 -
		THROW,									// 4 - Dude: throwing the dog
		ON_AIR,									// 5 - Dog: on air (falling or being throwed)
		DOG_ON_LAP,							// 6 - Dude: ready to throw the dog; Dog: cannot move
		STATE_NULL 					/// null
	};
	public eFSMState currentState;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// 
	void Awake() {

		// Get the HUD script
		hudScript = GetComponent<GameHUD>();
		movementScript = GetComponent<SimpleMoveRigidBody2D>();
		animator = this.GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
	
		// Initializes the FSM
		currentState = eFSMState.IDLE;
		FSMEnterNewState(currentState);
	}
	
	// Update is called once per frame
	void Update () {
		
		// FIXME
		CheckInput();
		FSMExecuteCurrentState();
	
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

				//animator.SetBool("bnPickedItem", true);
			}
			// Tell the item that we picked it up
			Item itemScript = trItemPicked.gameObject.GetComponent<Item>();
			itemScript.PickedUp(this.transform);

			// When carrying an item, the player will move slowly
			//movementScript.fMaxSpeed = fCarryingItemSpeed;
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

		// When carrying an item, the player will move slowly
		movementScript.fMaxSpeed = fRunningSpeed;
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
	
	/* ====================================================================================================
	 * FINITE STATE MACHINE DEFINITIONS
	 * ====================================================================================================
	 */

	/// <summary>
	/// Returns the FSM current state
	/// </summary>
	/// <returns>
	/// A <see cref="eFSMState"/>
	/// </returns>
	public eFSMState FSMGetCurrentState() {

		return currentState;
	}
	
	/// <summary>
	/// Puts the FSM in a new state
	/// </summary>
	/// <param name="eNewState">
	/// A <see cref="eFSMState"/>
	/// </param>
	public void FSMEnterNewState(eFSMState eNewState) {

		// Exits the current state
		FSMLeaveCurrentState();

    // Changes the state
    currentState = eNewState;

		// Set the animator
		if (animator != null) {
			animator.SetInteger ("FSMState", (int)currentState);
		}

		// Selects the new machine state
		switch(FSMGetCurrentState()) {

			case eFSMState.IDLE:
				movementScript.fMaxSpeed = fRunningSpeed;
				break;

			case eFSMState.RUNNING:
				movementScript.fMaxSpeed = fRunningSpeed;
				break;

			case eFSMState.IDLE_CARRYING_ITEM:
				movementScript.fMaxSpeed = fCarryingItemSpeed;
				break;

			case eFSMState.RUNNING_CARRYING_ITEM:
				movementScript.fMaxSpeed = fCarryingItemSpeed;
				break;

			case eFSMState.ON_AIR:
				movementScript.bnCanMoveHorizontally = false;
				break;

			default:
				Debug.LogError("I shouldn't be here.");
				break;
		}
	}
	
	/// <summary>
	/// Executes the FSM current state
	/// </summary>
	public void FSMExecuteCurrentState() {

		switch(FSMGetCurrentState()) {

			case eFSMState.IDLE:
				// Check if the character started to move
				if(Mathf.Abs(rigidbody2D.velocity.x) > 0.1f)
					FSMEnterNewState(eFSMState.RUNNING);

				if(Mathf.Abs(rigidbody2D.velocity.y) > 0.1f)
					FSMEnterNewState(eFSMState.ON_AIR);

				// Check if we didn't picked an item
				if(trItemPicked != null) 
					FSMEnterNewState(eFSMState.IDLE_CARRYING_ITEM);
				break;

			case eFSMState.RUNNING:
				// Check if the character stopped
				if(Mathf.Abs(rigidbody2D.velocity.x) < 0.2f)
					FSMEnterNewState(eFSMState.IDLE);

				// Check if we didn't picked an item
				if(trItemPicked != null) 
					FSMEnterNewState(eFSMState.RUNNING_CARRYING_ITEM);
				break;

			case eFSMState.IDLE_CARRYING_ITEM:
				// Check if the character started to move
				if(Mathf.Abs(rigidbody2D.velocity.x) > 0.1f) {
					if(trItemPicked != null)
						FSMEnterNewState(eFSMState.RUNNING_CARRYING_ITEM);
					else
						FSMEnterNewState(eFSMState.RUNNING);
				}
				else if(trItemPicked == null) {

					FSMEnterNewState(eFSMState.IDLE);
				}
				break;

			case eFSMState.RUNNING_CARRYING_ITEM:
				// Check if the character stopped to move
				if(Mathf.Abs(rigidbody2D.velocity.x) < 0.1f) {
					if(trItemPicked != null)
						FSMEnterNewState(eFSMState.IDLE_CARRYING_ITEM);
					else
						FSMEnterNewState(eFSMState.IDLE);
				}
				else if(trItemPicked == null) {
						FSMEnterNewState(eFSMState.RUNNING);
				}
				break;

			case eFSMState.ON_AIR:
				// Check if the character stopped falling
				if(Mathf.Abs(rigidbody2D.velocity.y) < 0.1f)
					FSMEnterNewState(eFSMState.IDLE);
				break;

			default:
				Debug.LogError("I shouldn't be here.");
				break;
		}
	}
	
	/// <summary>
	/// Leaves the current state. Used whenever the FSM enters a new state
	/// </summary>
	public void FSMLeaveCurrentState() {

		switch(FSMGetCurrentState()) {

			case eFSMState.ON_AIR:
				movementScript.bnCanMoveHorizontally = true;
				break;
			default:
				//Debug.LogError("I shouldn't be here.");
				break;
		}
	}

}
