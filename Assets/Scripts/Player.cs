using UnityEngine;
using System.Collections;

/// <summary>
///
/// </summary>
public class Player : MonoBehaviour {

	public MainGame.ePlayerType		playerType;	//< (from MainGame)
	public bool										bnPickedUp = false;
	public bool										bnCarryingItem = false;
	public bool										bnCollisionDogAndDude = false;
	public GameHUD								hudScript;		//< The in-game HUD
	public Transform							trItemPicked;	//< Have we picked some item?
	public Transform							trItemOver;		//< Transform of the item we are over
	public Transform							trCarrier;		//< Put the object that indicates where the items must be placed (usually CarrierPosition)
	public Transform							trThrowCursor; 	//< Transform of the 'ThrowCursor' object. Only need for the homeless dude
	public SimpleMoveRigidBody2D	movementScript;
	public MainGame								gameScript;

	private Animator animator;

	float		fCarryingItemSpeed = 0.5f;
	float		fRunningSpeed = 1.0f;
	float		fHorizontalSpeedThreshold = 0.1f;
	float		fVerticalSpeedThreshold = 0.3f;
	
	// Throw bar stuff
	float		fThrowBarValue = 0.0f;
	float		fLastKeyUp = 0.0f;

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
		gameScript = GameObject.Find("Game").gameObject.GetComponent<MainGame>();

		// Get the objects for each type of player
		if(playerType == MainGame.ePlayerType.DUDE) {

			// Throw cursor object
			trThrowCursor = transform.Find("ThrowCursor");
			if(trThrowCursor != null) {

				trThrowCursor.gameObject.SetActive(false);
			}
		}
	}

	// Use this for initialization
	void Start () {
	
		// Initializes the FSM
		currentState = eFSMState.IDLE;
		FSMEnterNewState(currentState);

		//
		if(playerType == MainGame.ePlayerType.DUDE) {

			gameScript.dudeScript = this;
		}
		if(playerType == MainGame.ePlayerType.DOG) {

			gameScript.dogScript = this;
		}
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

			if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) {
				//
				hudScript.uiButtonALabel.text = "JUMP OFF";
			}
			else {
				// Updates the HUD
				hudScript.uiButtonALabel.text = "JUMP ON";
				bnCollisionDogAndDude = true;
			}
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
			bnCollisionDogAndDude = false;
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

			// Tell the item that we picked it up
			Item itemScript = trItemPicked.gameObject.GetComponent<Item>();
			//itemScript.PickedUp(this.transform);
			itemScript.PickedUp(trCarrier);

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
		//itemScript.Dropped(this.transform);
		itemScript.Dropped(trCarrier);

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

	/// <summary>
	//
	/// </summary>
	public void DogJumpedOnMyLap() {

		FSMEnterNewState(eFSMState.DOG_ON_LAP);
	}

	/// <summary>
	///
	/// <summary>
	public void ThrowDog() {

		float fMaxThrowForce = 5.0f;

		Vector2 vThrowForceDirection = Vector2.one * fMaxThrowForce;
		gameScript.dogScript.ThrowDog(vThrowForceDirection);
	}

	/// <summary>
	///
	/// <summary>
	public void ThrowDog(Vector2 vThrowForce) {

		FSMEnterNewState(eFSMState.ON_AIR);
		rigidbody2D.AddForce(vThrowForce, ForceMode2D.Impulse);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * INPUT: MOVE TO ANOTHER SCRIPT, PLEASE!
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Check the player input
	/// <summary>
	void CheckInput() {

		// DOG STUFF
		if(playerType == MainGame.ePlayerType.DOG) {

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
			if(Input.GetKeyUp(KeyCode.L)) { // Dog Button A

				// Are we in touch with the dude?
				if(bnCollisionDogAndDude) {
					// Jump on the lap of the dude
					FSMEnterNewState(eFSMState.DOG_ON_LAP);
					// TODO: if holding an item, drop it immediately
				}
				else if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) {

					// Jump off the lap of the dude
					FSMEnterNewState(eFSMState.ON_AIR);
				}
			}
		}
		// DUDE STUFF
		if(playerType == MainGame.ePlayerType.DUDE) {

			if(Input.GetKeyUp(KeyCode.Y)) { // Dude Button B

				if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) { // Throw the dog?

					ThrowDog();
				}
				else {
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
			if(Input.GetKeyUp(KeyCode.U)) { // Dude Button A

				if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) {
					// Pump the throw bar
					//if((Time.time - fLastKeyUp) > 0.15f) {

						fThrowBarValue += 16 * Time.deltaTime;
						fThrowBarValue = Mathf.Clamp01(fThrowBarValue);
					//}
				}
				fLastKeyUp = Time.time;
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

			case eFSMState.DOG_ON_LAP:
				if(playerType == MainGame.ePlayerType.DOG) {
					// Dog: while on the lap, the dog cannot move
					movementScript.bnCanMoveHorizontally = false;
					// Updates the HUD
					hudScript.uiButtonALabel.text = "JUMP OFF";
					// Tell the Dude object that the dog is in his lap
					gameScript.dudeScript.DogJumpedOnMyLap();
				}

				if(playerType == MainGame.ePlayerType.DUDE) {
					// Dude: enable the throw
					trThrowCursor.gameObject.SetActive(true);
					// Updates the HUD
					hudScript.uiButtonBLabel.text = "THROW";
					hudScript.uiButtonALabel.text = "";
					// Tell the player that he must press the button
					hudScript.ButtonAAnimate(true);
					hudScript.ThrowBarActivate();
				}
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
				if(Mathf.Abs(rigidbody2D.velocity.x) > fHorizontalSpeedThreshold)
					FSMEnterNewState(eFSMState.RUNNING);

				if(Mathf.Abs(rigidbody2D.velocity.y) > fVerticalSpeedThreshold)
					FSMEnterNewState(eFSMState.ON_AIR);

				// Check if we didn't picked an item
				if(trItemPicked != null) 
					FSMEnterNewState(eFSMState.IDLE_CARRYING_ITEM);
				break;

			case eFSMState.RUNNING:
				// Check if the character stopped
				if(Mathf.Abs(rigidbody2D.velocity.x) < fHorizontalSpeedThreshold)
					FSMEnterNewState(eFSMState.IDLE);

				if(Mathf.Abs(rigidbody2D.velocity.y) > fVerticalSpeedThreshold)
					FSMEnterNewState(eFSMState.ON_AIR);

				// Check if we didn't picked an item
				if(trItemPicked != null) 
					FSMEnterNewState(eFSMState.RUNNING_CARRYING_ITEM);
				break;

			case eFSMState.IDLE_CARRYING_ITEM:
				// Check if the character started to move
				if(Mathf.Abs(rigidbody2D.velocity.x) > fHorizontalSpeedThreshold) {
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
				if(Mathf.Abs(rigidbody2D.velocity.x) < fHorizontalSpeedThreshold) {
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
				if(Mathf.Abs(rigidbody2D.velocity.y) < fHorizontalSpeedThreshold)
					FSMEnterNewState(eFSMState.IDLE);
				break;

			case eFSMState.DOG_ON_LAP:
				if(playerType == MainGame.ePlayerType.DOG) {
					Vector3 vDudePosition = gameScript.trDude.transform.position;
					transform.position = vDudePosition;
				}

				if(playerType == MainGame.ePlayerType.DUDE) {

					fThrowBarValue -= Time.deltaTime;
					fThrowBarValue = Mathf.Clamp01(fThrowBarValue);
					hudScript.ThrowBarUpdate(fThrowBarValue);
				}
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

			case eFSMState.DOG_ON_LAP:
				if(playerType == MainGame.ePlayerType.DOG) {
					// Dog: restore the ability to move
					movementScript.bnCanMoveHorizontally = true;
				}
				if(playerType == MainGame.ePlayerType.DUDE) {
					// Dude: enable the throw
					trThrowCursor.gameObject.SetActive(false);
					// Updates the HUD
					hudScript.ButtonAAnimate(false);
				}
				break;
			default:
				//Debug.LogError("I shouldn't be here.");
				break;
		}
	}

}
