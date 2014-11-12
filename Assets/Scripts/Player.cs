using UnityEngine;
using System.Collections;

/// <summary>
///
/// </summary>
public class Player : MonoBehaviour {

	public MainGame.ePlayerType		playerType;	//< (from MainGame)
	public Transform							trCarrier;					//< Put the object that indicates where the items must be placed (usually CarrierPosition)
	//public bool										bnPickedUp = false;
	//public bool										bnCarryingItem = false;
	[HideInInspector] public bool										bnCarryingDog = false;
	[HideInInspector] public bool										bnCollisionDogAndDude = false;
	[HideInInspector] public GameHUD								hudScript;					//< The in-game HUD
	[HideInInspector] public Transform							trItemPicked;				//< Have we picked some item?
	public Transform							trItemOver;					//< Transform of the item we are over
	[HideInInspector] public Transform							trWindowOver;				//< 
	[HideInInspector] public Transform							trThrowCursor; 			//< Transform of the 'ThrowCursor' object. Only need for the homeless dude
	[HideInInspector] public ThrowCursor						throwCursorScript;	//< The ThrowCursor from the cursor object
	[HideInInspector] public SimpleMoveRigidBody2D	movementScript;
	[HideInInspector] public MainGame								gameScript;
	[HideInInspector] public CheckHitBox						hitBoxScript;
	public Vector2								vThrowForceDirection;
	[HideInInspector] public Transform trThrowPosition;
	[HideInInspector] public Transform	trSpawnPoint;

	Transform							trCamera;
	CameraFollowTarget2D	cameraScript;

	private Animator animator;
	private Animator animatorOriginal;

	// Movement stuff
	public float		fCarryingItemSpeed = 0.5f;
	public float		fRunningSpeed = 1.0f;
	public float		fAirSpeed = 10.0f;
	float		fHorizontalSpeedThreshold = 0.1f;
	float		fVerticalSpeedThreshold = 0.3f;
	
	// Throw bar stuff
	float		fThrowBarValue = 0.0f;
	float		fLastKeyUp = 0.0f;
	public float fMaxThrowForce = 10.0f; // FIXME: make public and tweakable

	/// Enumeration with all the possible states
	public enum eFSMState { 
		IDLE,										// 0
		RUNNING,								// 1 - Walking, actually
		IDLE_CARRYING_ITEM,			// 2 - 
		RUNNING_CARRYING_ITEM,	// 3 -
		THROW,									// 4 - Dude: throwing the dog
		ON_AIR,									// 5 - Dog: on air (falling or being throwed)
		DOG_ON_LAP,							// 6 - Dude: ready to throw the dog; Dog: cannot move
		SIT,										// 7 - Sitted by the fire
		STATE_NULL 					/// null
	};
	[HideInInspector] public eFSMState currentState;
	AnimationClipOverrides animationOverridesScript;
	SpriteRenderer sr;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	///
	/// </summary>
	void Awake() {

		// Get the HUD script
		hudScript = GetComponent<GameHUD>();	// HUD script
		movementScript = GetComponent<SimpleMoveRigidBody2D>();	// movement script
		animator = this.GetComponent<Animator> ();	// animator
		animatorOriginal = animator;	// keep the original animator (used by the dude's animation stuff)
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();	// main game script
		hitBoxScript = transform.Find("HitBox").gameObject.GetComponent<CheckHitBox>(); // hit box script

		sr = GetComponent<SpriteRenderer>();	// pointer to the sprite renderer

		// HUD STUFF
		// Get the objects for each type of player
		if(playerType == MainGame.ePlayerType.DUDE) {

			// Throw cursor object
			trThrowCursor = transform.Find("ThrowCursor");
			if(trThrowCursor != null) {

				throwCursorScript = trThrowCursor.gameObject.GetComponent<ThrowCursor>();
				trThrowCursor.gameObject.SetActive(false);
			}

			// Get the script to change the animations for each item carried
			animationOverridesScript = GetComponent<AnimationClipOverrides>();

			trThrowPosition = transform.Find("ThrowPosition");
		}
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
		// Initializes the FSM
		currentState = eFSMState.IDLE;
		FSMEnterNewState(currentState);

		//
		if(playerType == MainGame.ePlayerType.DUDE) {

			gameScript.dudeScript = this;
			gameScript.hudDudeScript = hudScript;
		}
		if(playerType == MainGame.ePlayerType.DOG) {

			gameScript.dogScript = this;
			gameScript.hudDogScript = hudScript;
		}
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
		
		// FIXME
		CheckInput();
		FSMExecuteCurrentState();
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * 
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// The camera will call this method to register itself with this player
	/// </summary>
	public void RegisterCamera(Transform trCam, CameraFollowTarget2D camScript) {

		trCamera = trCam;
		cameraScript = camScript;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * COLLISION WITH ANOTHER PLAYER
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Dog over the dude
	/// </summary>
	public void OverDudeEnter() {

		// ignore all collisions when not playing
		if(gameScript.GetCurrentGameStatus() != MainGame.eGameStatus.GAME_PLAY)
			return;

		// Dog with dude? 
		if(playerType == MainGame.ePlayerType.DOG) {

			if(hudScript != null){
				// Updates the HUD
				hudScript.SetButtonsText("JUMP ON", null);
				bnCollisionDogAndDude = true;
			}
		}
	}

	/// <summary>
	/// Dog over the dude
	/// </summary>
	public void OverDudeExit() {

		// ignore all collisions when not playing
		if(gameScript.GetCurrentGameStatus() != MainGame.eGameStatus.GAME_PLAY)
			return;

		// Dog with dude? 
		// FIXME
		//if(playerType == MainGame.ePlayerType.DOG && FSMGetCurrentState() != eFSMState.DOG_ON_LAP) {
		if(playerType == MainGame.ePlayerType.DOG && !bnCarryingDog) {

			if(hudScript != null) {
				// Updates the HUD
				hudScript.SetButtonsText("", null);
				bnCollisionDogAndDude = false;
			}
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * COLLISION WITH ITEMS PROCESSING
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// We are over some item
	/// <summary>
	public void OverItemEnter(Transform trItem) {


		// Do we have an item already?
		if(trItemPicked != null)
			return;

		trItemOver = trItem;

		if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_PLAY) {
			// Updates the HUD
			hudScript.SetButtonsText(null, "PICK");
		}
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

			if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_PLAY) {
				hudScript.SetButtonsText(null,"");
			}
			trItemOver = null;
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * WINDOWS METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// We are over some window
	/// <summary>
	/// <param name="trWindow">The Transform of the window</param>
	public void OverWindowEnter(Transform trWindow, Transform trWindowOtherSide) {

		// ignore all collisions when not playing
		if(gameScript.GetCurrentGameStatus() != MainGame.eGameStatus.GAME_PLAY)
			return;

		trWindowOver = trWindow;
		// Updates the HUD
		// Are we holding an item?
		if(trItemPicked != null) {

			hudScript.SetButtonsText("THROW OUT",null);
		}
		else {
			hudScript.SetButtonsText("JUMP IN", null);
		}
	}

	/// <summary>
	/// Not 'touching' the window anymore
	/// <summary>
	/// <param name="trWindow">The Transform of the window</param>
	public void OverWindowExit(Transform trWindow) {

		// ignore all collisions when not playing
		if(gameScript.GetCurrentGameStatus() != MainGame.eGameStatus.GAME_PLAY)
			return;

		if(trWindowOver == trWindow)
			trWindowOver = null;

		// Updates the HUD
		hudScript.SetButtonsText("",null);
	}

	/// <summary>
	/// Throw an item through the window. Only works if the window is the 'inside one'
	/// </summary>
	void ThrowItemThroughWindow(Transform trWindowOutside) {

		if(trItemPicked == null)  
			return;

		// 1 - Move the item to the out window location
		trItemPicked.transform.position = trWindowOutside.transform.position;
		// Updates the HUD
		hudScript.SetButtonsText("",null);

		// 2 - Drop the item
		DropItem();
	}

	/// <summary>
	/// Makes the character move through a window, reappearing on the other side (actually, another room)
	/// </summary>
	/// <param name="trWindow"> Transform of the window </param>
	public void MoveThroughWindow(Transform trWindow) {

		// Get the 'other side' object
		Window windowScript = trWindow.gameObject.GetComponent<Window>();
		Transform trOtherSide = windowScript.trWindowOtherSide;
		BasicRoom otherSideBasicRoomScript = windowScript.windowOtherSideScript.basicRoomScript; //< The BasicRoom script for the window on the other side

		// Move the character
		this.transform.position = trOtherSide.transform.position;	// FIXME
		// Trigger the 'entered the room'
		otherSideBasicRoomScript.EnteredRoom();
		//
		//cameraScript.AdjustHeightForNewRoom(windowScript.windowOtherSideScript.trBasicRoom);
		cameraScript.SetCurrentBasicRoom(windowScript.windowOtherSideScript.trBasicRoom, otherSideBasicRoomScript);
		// Updates the camera limits
		cameraScript.UpdateLimits(otherSideBasicRoomScript.GetLeftLimit(), otherSideBasicRoomScript.GetRightLimit());
		// Update the focus of the camera
		cameraScript.FocusCameraOnTarget();
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * ITEMS METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void PickItem() {

		Debug.Log(this.transform.name + " PickItem");

		// FIXME: and if the item cannot be picked? How about just trigger the item animation and bail out?
		if(trItemOver != null && trItemPicked == null) {

			trItemPicked = trItemOver;
			trItemOver = null;

			if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_PLAY) {
				// Updates the HUD
				hudScript.SetButtonsText(null,"DROP");
			}

			// Tell the item that we picked it up
			Item itemScript = trItemPicked.gameObject.GetComponent<Item>();
			//itemScript.PickedUp(this.transform);
			itemScript.PickedUp(trCarrier, this);

			// When carrying an item, the player will move slowly
			//movementScript.fMaxSpeed = fCarryingItemSpeed;
			
			// Override the animation if needed
			if(animationOverridesScript != null) {

				//animationOverridesScript.Init(animator, trItemPicked.name);
				animationOverridesScript.ChangeAnimation(animator, itemScript.itemType);
			}
		}
	}

	/// <summary>
	///
	/// </summary>
	void DropItem() {

		if(trItemPicked == null) 
			return;

		// Override the animation if needed
		if(animationOverridesScript != null) {

			//animationOverridesScript.Init(animator, trItemPicked.name);
			animationOverridesScript.RestoreAnimation(animator);
		}
		
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
		hudScript.SetButtonsText(null,"");

		// When carrying an item, the player will move slowly
		movementScript.fMaxSpeed = fRunningSpeed;
	}

	/// <summary>
	/// Item was thrown in the fire (this is almost like the 'drop')
	/// </summary>
	public void BurnItem() {
		trItemPicked = null;

		// Change the animation
		if (animator != null) {

			animator.SetBool("bnPickedItem", false);
		}

		// Updates the HUD
		if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_PLAY) {
			hudScript.SetButtonsText(null,"");
		}

		// When carrying an item, the player will move slowly
		movementScript.fMaxSpeed = fRunningSpeed;

	}

	/* -----------------------------------------------------------------------------------------------------------
	 * DOG/DUDE INTERACTION
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	public void DogJumpedOnMyLap() {

		//FSMEnterNewState(eFSMState.DOG_ON_LAP);
		bnCarryingDog = true;

		if(playerType == MainGame.ePlayerType.DOG) {
			//// Dog: while on the lap, the dog cannot move
			//movementScript.bnPlayerCanControl = false;
			//// Updates the HUD
			//hudScript.SetButtonsText("JUMP OFF", null);
			//// Tell the Dude object that the dog is in his lap
			//gameScript.dudeScript.DogJumpedOnMyLap();
			//sr.enabled = false;
			FSMEnterNewState(eFSMState.DOG_ON_LAP);
		}

		if(playerType == MainGame.ePlayerType.DUDE) {
			// Override the animation if needed
			if(animationOverridesScript != null) {

				animationOverridesScript.ChangeAnimation(animator, MainGame.eItemTypes.ITEM_DOG);
			}
			
			FSMEnterNewState(eFSMState.IDLE_CARRYING_ITEM);
			// Dude: enable the throw
			trThrowCursor.gameObject.SetActive(true);
			// Updates the HUD
			hudScript.SetButtonsText("","THROW");
			// Tell the player that he must press the button
			hudScript.ButtonAAnimate(true);
			hudScript.ThrowBarActivate();
		}
	}

	/// <summary>
	/// The dog jumped from the dude's lap
	/// </summary>
	public void DogJumpedOffMyLap() {

		bnCarryingDog = false;

		// From the dog side...
		if(playerType == MainGame.ePlayerType.DOG) {
			// Refresh the collider from the dog, to re-detect the collisions
			if(hitBoxScript != null) {

				hitBoxScript.RefreshCollider();
			}

			// Dog: restore the ability to move
			movementScript.bnPlayerCanControl = true;
			// Updates the HUD
			hudScript.SetButtonsText("", null);
			// Tell the Dude object that the dog is NOT in his lap anymore
			gameScript.dudeScript.DogJumpedOffMyLap();

			// Move the dog to the dude position
			Vector3 vDudePosition = gameScript.trDude.transform.position;
			transform.position = vDudePosition;
			// Restore the sprite renderer
			sr.enabled = true;
		}

		if(playerType == MainGame.ePlayerType.DUDE) {
			// Dude: enable the throw
			trThrowCursor.gameObject.SetActive(false);
			// Updates the HUD
			hudScript.SetButtonsText(null, "");
			hudScript.ThrowBarDeactivate();
			hudScript.ButtonAAnimate(false);

			FSMEnterNewState(eFSMState.IDLE);
			// Override the animation if needed
			if(animationOverridesScript != null) {

				animationOverridesScript.RestoreAnimation(animator);
			}
		}

		// Change to idle or running
		FSMEnterNewState(eFSMState.IDLE);
	}

	/// <summary>
	/// What happens when the dog is catched inside an apartment
	/// Called from Room
	/// </summary>
	public void DogCatched() {
		// disable control
		MovementAllowToGetInput(false);
		// Drop the item, if any
		DropItem();
	}

	/// <summary>
	/// Called when the dog is caught inside an apartment
	/// </summary>
	public void ThrowTheDogOutOfTheWindow(Transform trWindow) {

		if(trWindow != null) {

			// Throw ourselves out the window, back into the street
			// TRANSPORT
			MoveThroughWindow(trWindow);
			//this.transform.position = trWindow.gameObject.GetComponent<Window>().trWindowOtherSide.transform.position;
			//cameraScript.FocusCameraOnTarget();

			// Trying to 'toss' the dog out
			rigidbody2D.AddForce(new Vector2(-1,1), ForceMode2D.Impulse);
		}
	}

	/// <summary>
	/// Throw method for the homeless dude
	/// <summary>
	public void ThrowDog() {

		//float fAngle = throwCursorScript.GetCurrentAngle() * Mathf.Deg2Rad;

		//vThrowForceDirection = new Vector3(Mathf.Sign(this.transform.localScale.x) * Mathf.Cos(fAngle), Mathf.Sin(fAngle), 0.0f);
	 	//vThrowForceDirection = vThrowForceDirection.normalized * fMaxThrowForce;
		//vThrowForceDirection = throwCursorScript.GetCursorDirection() * fMaxThrowForce;

		Vector3 vDir = trThrowCursor.position - trThrowPosition.position;

		vThrowForceDirection = new Vector2(vDir.x, vDir.y).normalized;

		gameScript.dogScript.ThrowDog(vThrowForceDirection);
		
		// Change the FSM
		FSMEnterNewState(eFSMState.IDLE);
	}

	/// <summary>
	/// Throw method for the dog (called from the throw method from the dude)
	/// <summary>
	public void ThrowDog(Vector2 vThrowForce) {

		// Restore the sprite renderer
		sr.enabled = true;
		FSMEnterNewState(eFSMState.ON_AIR);
		vThrowForceDirection = vThrowForce * fMaxThrowForce;
		rigidbody2D.AddForce(vThrowForceDirection); // fMaxThrowForce now is set on the dog prefab

		// DEBUG
		Debug.DrawRay(transform.position, vThrowForceDirection, Color.red, 1.0f, false);

	}
	
	/* -----------------------------------------------------------------------------------------------------------
	 * GAME STATES
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Activate the Game Over status
	/// </summary>
	public void ActivateGameOver() {

		hudScript.ShowGameOver();
		MovementAllowToGetInput(false);
	}

	/// <summary>
	/// The player won this level
	/// </summary>
	public void ActivateGameWonLevel() {

		// TODO: make the players sit around the fire
		// or play some cutscene
		MovementAllowToGetInput(false);
		// Move the character to the spawn point
		Transform trSpawner = gameScript.GetPlayerSpawner(playerType);
		
		FSMEnterNewState(eFSMState.SIT);
		movementScript.HaltCharacter();

		if(trSpawner != null)
			transform.position = trSpawner.position;



		// And if we set the camera target as the barrel?
		cameraScript.SetCameraTarget(gameScript.GetBarrel());
		movementScript.FaceObject(gameScript.GetBarrel());

		cameraScript.FocusCameraOnTarget(); // FIXME: 
		cameraScript.ZoomInCharacters();

		// Change the hud
		hudScript.ShowLevelWon();

		// TODO
		// Change the music
		// wait for a keypress to jump to the next level
	}

	/// <summary>
	/// Change the movement script to apply the player's input on the character movement or not
	/// </summary>
	/// <param name="bnStatus">True to allow to get input and move the character; false otherwise
	public void MovementAllowToGetInput(bool bnStatus) {

		movementScript.bnAllowedToGetInput = bnStatus;
	}

	/* ====================================================================================================
	 * INPUT STUFF
	 * ====================================================================================================
	 */
		/// <summary>
	/// Check the player input
	/// <summary>
	void CheckInput() {

		// --------------------------------------------------------
		// DOG STUFF
		// --------------------------------------------------------
		if(playerType == MainGame.ePlayerType.DOG) {

			// On the start screen
			if(Input.GetKeyUp(KeyCode.Return)) {

				if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_START_SCREEN) {
					// Allow the player to move around
					//MovementAllowToGetInput(true);
					// Hud
					hudScript.uiCenterScreenLabel.text = "Waiting for the other player...";
					// Dog pressed the start button on the start screen
					gameScript.DogEnteredTheGame();
				}
				if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_WON_LEVEL_SCREEN) {
					// Load the next level
					Application.LoadLevel(gameScript.GetNextLevel());
				}
			}


			//
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
				if(bnCollisionDogAndDude && FSMGetCurrentState() != eFSMState.DOG_ON_LAP) {
					// Jump on the lap of the dude
					FSMEnterNewState(eFSMState.DOG_ON_LAP);
					// TODO: if holding an item, drop it immediately
				}
				else if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) {
					// Jump off the lap of the dude
					FSMEnterNewState(eFSMState.ON_AIR);
					// FIXME: tell the dude to change from DOG_ON_LAP
				}
				else if(trWindowOver != null) { // Touching a window

					Window windowScript = trWindowOver.gameObject.GetComponent<Window>();
					if(windowScript != null && windowScript.trWindowOtherSide != null)  {

						if(trItemPicked != null) {

							// Are we carrying an item? Throw it out the window!
							ThrowItemThroughWindow(windowScript.trWindowOtherSide);
						}
						else if(windowScript.IsTheWindowOpen()){

							// Throw ourselves out the window
							// TRANSPORT
							MoveThroughWindow(trWindowOver);
							//this.transform.position = windowScript.trWindowOtherSide.transform.position;
							//cameraScript.FocusCameraOnTarget();
						}
					}
				}
			}
		}

		// --------------------------------------------------------
		// DUDE STUFF
		// --------------------------------------------------------
		if(playerType == MainGame.ePlayerType.DUDE) {

			// On the start screen
			if(Input.GetKeyUp(KeyCode.Space)) { // Start button

				if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_START_SCREEN) {
					// Allow the player to move around
					//MovementAllowToGetInput(true);
					// Hud
					hudScript.uiCenterScreenLabel.text = "Waiting for the other player...";
					// Dog pressed the start button on the start screen
					gameScript.DudeEnteredTheGame();
				}
				if(gameScript.GetCurrentGameStatus() == MainGame.eGameStatus.GAME_WON_LEVEL_SCREEN) {
					// Load the next level
					Application.LoadLevel(gameScript.GetNextLevel());
				}
			}

			if(Input.GetKeyUp(KeyCode.Y)) { // Dude Button B

				// FIXME
				//if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) { // Throw the dog?
				if(bnCarryingDog) { // Throw the dog?

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

				// FIXME
				//if(FSMGetCurrentState() == eFSMState.DOG_ON_LAP) {
				if(bnCarryingDog) {
					// Pump the throw bar
					if((Time.time - fLastKeyUp) > 0.05f) {

						fThrowBarValue += 16 * Time.deltaTime;
						fThrowBarValue = Mathf.Clamp01(fThrowBarValue);
					}
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
				movementScript.fMaxSpeed = fAirSpeed;
				movementScript.bnPlayerCanControl = false;
				movementScript.bnOnAir = true;
				break;

			case eFSMState.DOG_ON_LAP:
				if(playerType == MainGame.ePlayerType.DOG) {
					bnCarryingDog = true;
					// Dog: while on the lap, the dog cannot move
					movementScript.bnPlayerCanControl = false;
					// Updates the HUD
					hudScript.SetButtonsText("JUMP OFF", null);
					// Tell the Dude object that the dog is in his lap
					gameScript.dudeScript.DogJumpedOnMyLap();
					sr.enabled = false; // Disable the sprite, because it's already in the homeless animation
					movementScript.SetRigidbodyKinematic(true); // Make it kinematic
				}

				//if(playerType == MainGame.ePlayerType.DUDE) {
				//	// Dude: enable the throw
				//	trThrowCursor.gameObject.SetActive(true);
				//	// Updates the HUD
				//	hudScript.SetButtonsText("","THROW");
				//	// Tell the player that he must press the button
				//	hudScript.ButtonAAnimate(true);
				//	hudScript.ThrowBarActivate();
				//}
				break;

			case eFSMState.SIT:
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
				if(trItemPicked != null || bnCarryingDog) 
					FSMEnterNewState(eFSMState.IDLE_CARRYING_ITEM);
				break;

			case eFSMState.RUNNING:
				// Check if the character stopped
				if(Mathf.Abs(rigidbody2D.velocity.x) < fHorizontalSpeedThreshold)
					FSMEnterNewState(eFSMState.IDLE);

				if(Mathf.Abs(rigidbody2D.velocity.y) > fVerticalSpeedThreshold)
					FSMEnterNewState(eFSMState.ON_AIR);

				// Check if we didn't picked an item
				if(trItemPicked != null || bnCarryingDog) 
					FSMEnterNewState(eFSMState.RUNNING_CARRYING_ITEM);
				break;

			case eFSMState.IDLE_CARRYING_ITEM:
				// Check if the character started to move
				if(Mathf.Abs(rigidbody2D.velocity.x) > fHorizontalSpeedThreshold) {
					if(trItemPicked != null || bnCarryingDog)
						FSMEnterNewState(eFSMState.RUNNING_CARRYING_ITEM);
					else
						FSMEnterNewState(eFSMState.RUNNING);
				}
				else if(trItemPicked == null && !bnCarryingDog) {

					FSMEnterNewState(eFSMState.IDLE);
				}

				// If the player is carrying the dog, updates the throw bar
				if(playerType == MainGame.ePlayerType.DUDE && bnCarryingDog) {

					fThrowBarValue -= Time.deltaTime;
					fThrowBarValue = Mathf.Clamp01(fThrowBarValue);
					hudScript.ThrowBarUpdate(fThrowBarValue);
				}
				break;

			case eFSMState.RUNNING_CARRYING_ITEM:
				// Check if the character stopped to move
				if(Mathf.Abs(rigidbody2D.velocity.x) < fHorizontalSpeedThreshold) {
					if(trItemPicked != null || bnCarryingDog)
						FSMEnterNewState(eFSMState.IDLE_CARRYING_ITEM);
					else
						FSMEnterNewState(eFSMState.IDLE);
				}
				else if(trItemPicked == null && !bnCarryingDog) {
						FSMEnterNewState(eFSMState.RUNNING);
				}
				// If the player is carrying the dog, updates the throw bar
				if(playerType == MainGame.ePlayerType.DUDE && bnCarryingDog) {

					fThrowBarValue -= Time.deltaTime;
					fThrowBarValue = Mathf.Clamp01(fThrowBarValue);
					hudScript.ThrowBarUpdate(fThrowBarValue);
				}
				break;

			case eFSMState.ON_AIR:
				// Check if the character stopped falling
				if(Mathf.Abs(rigidbody2D.velocity.y) < fHorizontalSpeedThreshold)
					FSMEnterNewState(eFSMState.IDLE);
				break;

			case eFSMState.DOG_ON_LAP:
				if(playerType == MainGame.ePlayerType.DOG) {
					Vector3 vDudePosition = gameScript.dudeScript.trThrowPosition.transform.position;
					transform.position = vDudePosition;
					// Added so the dog can fall when leaving the dude's lap
					bnCollisionDogAndDude = false;
				}

				//if(playerType == MainGame.ePlayerType.DUDE) {

				//	fThrowBarValue -= Time.deltaTime;
				//	fThrowBarValue = Mathf.Clamp01(fThrowBarValue);
				//	hudScript.ThrowBarUpdate(fThrowBarValue);
				//}
				break;

			case eFSMState.SIT:
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

			case eFSMState.IDLE:
				break;

			case eFSMState.RUNNING:

				break;
			case eFSMState.IDLE_CARRYING_ITEM:

				break;
			case eFSMState.RUNNING_CARRYING_ITEM:
				break;

			case eFSMState.THROW:
				break;

			case eFSMState.ON_AIR:
				movementScript.bnPlayerCanControl = true;
				movementScript.bnOnAir = false;
				break;

			case eFSMState.DOG_ON_LAP:
				if(playerType == MainGame.ePlayerType.DOG) {
					// TESTING: disable the collider from the dog
					if(hitBoxScript != null) {

						//hitBoxScript.RefreshCollider();
					}

					// Dog: restore the ability to move
					movementScript.bnPlayerCanControl = true;
					// Updates the HUD
					hudScript.SetButtonsText("", null);
					// Tell the Dude object that the dog is NOT in his lap anymore
					gameScript.dudeScript.DogJumpedOffMyLap();
					bnCarryingDog = false;
					// Restore the sprite renderer
					sr.enabled = true;
					movementScript.SetRigidbodyKinematic(false);
				}
				//if(playerType == MainGame.ePlayerType.DUDE) {
				//	// Dude: enable the throw
				//	trThrowCursor.gameObject.SetActive(false);
				//	// Updates the HUD
				//	hudScript.SetButtonsText(null, "");
				//	hudScript.ThrowBarDeactivate();
				//	hudScript.ButtonAAnimate(false);
				//}
				break;

			case eFSMState.SIT:
				break;

			default:
				Debug.LogError("FSM Exit state: I shouldn't be here.");
				break;
		}
	}
}
