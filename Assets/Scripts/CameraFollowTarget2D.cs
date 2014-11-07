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

	public bool bnZoomIn = false;
	public int nZoomDirection = 1;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// Use this for initialization
	void Start () {

		cam = gameObject.GetComponent<Camera>();	
		tr = this.transform;

	}
	
	// Update is called once per frame
	void Update () {

		// update the camera position. The 'x' value is the left ('start') of the screen	
		//vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
	//	vScreenWorldPosition = cam.ScreenToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
		if(Input.GetMouseButtonUp(1)) {

			nZoomDirection *= -1;
			bnZoomIn = true;
		}
	}

	/// <summary>
	///
	/// </summary>
	void LateUpdate() {

		if(trTarget == null)
			return;

		if(bnZoomIn) {

			camera.orthographicSize += Time.deltaTime * nZoomDirection;
			if(camera.orthographicSize < 0.01f) { camera.orthographicSize = 0.01f; bnZoomIn = false; }
			if(camera.orthographicSize > 0.72f) { camera.orthographicSize = 0.72f; bnZoomIn = false; }
		}
		Vector3 vTargetPosition = trTarget.position;	// The position of the target in the world
		Vector3 vCurrentCameraPosition = tr.position;	// Current camera position

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
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * 
	 * -----------------------------------------------------------------------------------------------------------
	 */

	public void SetCameraTarget(Transform trNewTarget) {

		if(trNewTarget != null) {

			trTarget = trNewTarget;

			// Tell the player that we are they camera
			Player playerScript = trNewTarget.gameObject.GetComponent<Player>();

			if(playerScript != null)
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
		vTargetPosition.y = this.transform.position.y;

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

	public void ZoomInCharacters() {

		camera.orthographicSize = .3f;
		Vector3 vTargetPosition = new Vector3(transform.position.x, .41f, transform.position.z);
		transform.position = vTargetPosition;
	}
 }
