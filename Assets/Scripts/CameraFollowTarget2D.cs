using UnityEngine;
using System.Collections;

/// <summary>
/// Makes a camera follow an object. When the camera hits the game screen limits it's stops following
/// </summary>
public class CameraFollowTarget2D : MonoBehaviour {

	public Transform	trTarget;				//< target to follow (any of the players characters)
	float	fHalfScreenWidth = 0.8f;		//< 80px, the width of a Game Boy screen is 160px
	float fHalfScreenHeight = 0.72f;	//< 72px, as the GameBoy screen is 144px tall
	Transform tr;

	// Limits of the current room
	public Transform	trLeftScenarioLimit;	//< the object/collider that will prevent the player to move further to the left
	public Transform	trRightScenarioLimit;	//< the object/collider that will prevent the player to move further to the right

	// Deprecate?
	public Vector3		vViewPortWorldPosition;
	public Vector3		vScreenWorldPosition;
	Camera cam;
	public bool bnCamLockedLeft;

	public Transform trCurrentBasicRoom;
	public BasicRoom currentBasicRoomScript;

	// CLEAN THIS FILE!
	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// Use this for initialization
	void Start () {

		cam = gameObject.GetComponent<Camera>();	
		tr = this.transform;

		//if(trTarget != null) {

		//	// Tell the player that we are they camera
		//	Player playerScript = trTarget.gameObject.GetComponent<Player>();
		//	playerScript.RegisterCamera(this.transform, this);
		//}
	}
	
	// Update is called once per frame
	void Update () {

		// update the camera position. The 'x' value is the left ('start') of the screen	
		//vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
	//	vScreenWorldPosition = cam.ScreenToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
	}

	/// <summary>
	///
	/// </summary>
	void LateUpdate() {

		if(trTarget == null)
			return;

		Vector3 vTargetPosition = trTarget.position;	// The position of the target in the world
		Vector3 vCurrentCameraPosition = tr.position;	// Current camera position

		// HORIZONTAL CHECK
		// Check where the target is in the screen
		//if(vTargetPosition.x < tr.position.x && !bnCamLockedLeft) {
		//	// Target to the left of the camera, camera not locked:
		//	// Follow target
		//	Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
		//	tr.position = vNewPosition;
		//}
		//else if(vTargetPosition.x > tr.position.x) {
		//	// Target to the right of the camera:
		//	// make the camera follow
		//	Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
		//	tr.position = vNewPosition;
		//}
		

		// Check the left limit of the screen
		if(trTarget.position.x <= (trLeftScenarioLimit.position.x + fHalfScreenWidth)) {

			if(!bnCamLockedLeft) {
				bnCamLockedLeft = true;	// Lock the camera
				Vector3 vNewPosition = new Vector3((trLeftScenarioLimit.position.x + fHalfScreenWidth), tr.position.y, tr.position.z);
				tr.position = vNewPosition;
			}
		}
		else if(trTarget.position.x >= (trRightScenarioLimit.position.x - fHalfScreenWidth)) {

			if(!bnCamLockedLeft) {
				bnCamLockedLeft = true;	// Lock the camera
				Vector3 vNewPosition = new Vector3((trRightScenarioLimit.position.x - fHalfScreenWidth), tr.position.y, tr.position.z);
				tr.position = vNewPosition;
			}
		}
		else {

			if(bnCamLockedLeft) 
				bnCamLockedLeft = false;

			// Follow target
			Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}
		// VERTICAL CHECK
		// 1 - If the dog is in the upper half of the camera, follow it
		//if(vTargetPosition.y < vCurrentCameraPosition.y) {

		//	// FIXME: fix the 0.72f value with a 'half room height' value or something like that
		//	if(vCurrentCameraPosition.y > 0.72f) {


		//		Vector3 vNewPosition = tr.position;
		//		vNewPosition.y = vTargetPosition.y;
		//		tr.position = vNewPosition;
		//	}


		//}
		//else if(vTargetPosition.y > vCurrentCameraPosition.y) {
		//	
		//	Vector3 vNewPosition = tr.position;
		//	vNewPosition.y = vTargetPosition.y;
		//	tr.position = vNewPosition;
		//}
		//else {

		//}

		//	vScreenWorldPosition = cam.ScreenToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
		//	Update the viewport position
		//vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));

		//if(vViewPortWorldPosition.x < 0f) {
		//	// Camera with the left off screen?
		//	bnCamLockedLeft = true;		// Lock it's movement
		//	tr.position = vCurrentCameraPosition;	// Bring it back
		//}
		//else if(isEqual(vViewPortWorldPosition.x, 0)) {
		//	// At zero
		//	if(!bnCamLockedLeft) {
		//		bnCamLockedLeft = true; // Camera not locked? Lock it up!
		//		tr.position = vCurrentCameraPosition;	// Update the position
		//	}
		//}
		//else {
		//	bnCamLockedLeft = false;
		//}

		// VERTICAL
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * 
	 * -----------------------------------------------------------------------------------------------------------
	 */

	public void SetCameraTarget(Transform trTarget) {

		if(trTarget != null) {

			// Tell the player that we are they camera
			Player playerScript = trTarget.gameObject.GetComponent<Player>();
			playerScript.RegisterCamera(this.transform, this);
		}
	}

	/// <summary>
	///
	/// </summary>
	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.06f);
	}
	
	/// <summary>
	///
	/// </summary>
	public void FocusCameraOnTarget() {

		Vector3 vTargetPosition = trTarget.position;		// The position of the target in the world
		vTargetPosition.z = this.transform.position.z;
		// Updates the height position
		if(trCurrentBasicRoom != null)
			vTargetPosition.y = trCurrentBasicRoom.position.y + fHalfScreenHeight;

		this.transform.position = vTargetPosition;
		CheckHorizontalLimits();
	}

	/// <summary>
	/// Set the current basic room
	/// </summary>
	/// <param name="trRoom"></param>
	public void SetCurrentBasicRoom(Transform trRoom, BasicRoom basicRoomScript) {

		trCurrentBasicRoom = trRoom;
		currentBasicRoomScript = basicRoomScript;
	}

	/// <summary>
	/// Check if the camera is showing something beyound the 0 world position
	/// </summary>
	void CheckHorizontalLimits() {

		// Check the viewport position: what of the world the camera is showing?
		//vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
		//if(vViewPortWorldPosition.x < 0.0f) {

		//	Vector3 vTargetPosition = transform.position;	// The position of the target in the world
		//	vTargetPosition.x = fHalfScreenWidth; 
		//	this.transform.position = vTargetPosition;
		//}
		if(tr.position.x < (trLeftScenarioLimit.position.x + fHalfScreenWidth)) {

			Vector3 vNewPosition = new Vector3((trLeftScenarioLimit.position.x + fHalfScreenWidth), tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}
		else if(tr.position.x > (trLeftScenarioLimit.position.x - fHalfScreenWidth)) {

			Vector3 vNewPosition = new Vector3((trRightScenarioLimit.position.x - fHalfScreenWidth), tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}
	}

	/// <summary>
	/// Update the limits objects. Called when the player enter a new room
	/// </summary>
	/// <param name="trLeft">Transform of the left limit object</param>
	/// <param name="trRight">Transform of the right limit object</param>
	public void UpdateLimits(Transform trLeft, Transform trRight) {

		trLeftScenarioLimit = trLeft;
		trRightScenarioLimit = trRight;
	}
 }
