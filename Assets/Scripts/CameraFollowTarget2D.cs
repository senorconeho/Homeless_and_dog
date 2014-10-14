using UnityEngine;
using System.Collections;

/// <summary>
/// Makes a camera follow an object. When the camera hits the game screen limits it's stops following
/// </summary>
public class CameraFollowTarget2D : MonoBehaviour {

	public Transform	trTarget;	//< target to follow
	public Vector3		vViewPortWorldPosition;
	public Vector3		vScreenWorldPosition;
	Transform tr;
	Camera cam;
	public bool bnCamLockedLeft;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// Use this for initialization
	void Start () {

		cam = gameObject.GetComponent<Camera>();	
		tr = this.transform;

		if(trTarget != null) {

			// Tell the player that we are they camera
			Player playerScript = trTarget.gameObject.GetComponent<Player>();
			playerScript.RegisterCamera(this.transform, this);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
	//	vScreenWorldPosition = cam.ScreenToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
	}

	/// <summary>
	///
	/// </summary>
	void LateUpdate() {

		if(trTarget == null)
			return;

		Vector3 vTargetPosition = trTarget.position;	// The position of the target in the world
		Vector3 vCurrentCameraPosition = tr.position;						// Current camera position
		// HORIZONTAL CHECK
		// Check where the target is in the screen
		if(vTargetPosition.x < tr.position.x && !bnCamLockedLeft) {

			// Check left limit
			Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}
		else if(vTargetPosition.x > tr.position.x) {

			// make the camera follow
			Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}

		// VERTICAL CHECK
		// 1 - If the dog is in the upper half of the camera, follow it
		if(vTargetPosition.y < vCurrentCameraPosition.y) {

			// FIXME: fix the 0.72f value with a 'half room height' value or something like that
			if(vCurrentCameraPosition.y > 0.72f) {


				Vector3 vNewPosition = tr.position;
				vNewPosition.y = vTargetPosition.y;
				tr.position = vNewPosition;
			}


		}
		else if(vTargetPosition.y > vCurrentCameraPosition.y) {
			
			Vector3 vNewPosition = tr.position;
			vNewPosition.y = vTargetPosition.y;
			tr.position = vNewPosition;
		}
		else {

		}

		//	vScreenWorldPosition = cam.ScreenToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
		vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));

		if(vViewPortWorldPosition.x < 0f) {

				bnCamLockedLeft = true;
				tr.position = vCurrentCameraPosition;
		}
		else if(isEqual(vViewPortWorldPosition.x, 0)) {

			if(!bnCamLockedLeft) {
				bnCamLockedLeft = true;
				tr.position = vCurrentCameraPosition;
			}
		}
		else
			bnCamLockedLeft = false;

		// VERTICAL
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * 
	 * -----------------------------------------------------------------------------------------------------------
	 */
	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.001f);
	}
	
	/// <summary>
	///
	/// </summary>
	public void FocusCameraOnTarget() {

		Vector3 vTargetPosition = trTarget.position;	// The position of the target in the world
		vTargetPosition.z = this.transform.position.z;
		this.transform.position = vTargetPosition;
		CheckHorizontalLimits();
	}

	/// <summary>
	/// Check if the camera is showing something beyound the 0 world position
	/// </summary>
	void CheckHorizontalLimits() {

		// Check the viewport position: what of the world the camera is showing?
		vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
		if(vViewPortWorldPosition.x < 0.0f) {

			Vector3 vTargetPosition = transform.position;	// The position of the target in the world
			vTargetPosition.x = .80f; // FIXME: half width of the screen
			this.transform.position = vTargetPosition;
		}
	}
}
