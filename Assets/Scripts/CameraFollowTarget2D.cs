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

		Vector3 vTargetPosition = trTarget.position;
		Vector3 vOldPosition = tr.position;
		// Check where the target is in the screen
		if(vTargetPosition.x < tr.position.x && !bnCamLockedLeft) {

			Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}
		else if(vTargetPosition.x > tr.position.x) {

			Vector3 vNewPosition = new Vector3(vTargetPosition.x, tr.position.y, tr.position.z);
			tr.position = vNewPosition;
		}

		//	vScreenWorldPosition = cam.ScreenToWorldPoint(new Vector3(0,0, camera.nearClipPlane));
		vViewPortWorldPosition = cam.ViewportToWorldPoint(new Vector3(0,0, camera.nearClipPlane));

		if(vViewPortWorldPosition.x < 0f) {

				bnCamLockedLeft = true;
				tr.position = vOldPosition;
		}
		else if(isEqual(vViewPortWorldPosition.x, 0)) {

			if(!bnCamLockedLeft) {
				bnCamLockedLeft = true;
				tr.position = vOldPosition;
			}
		}
		else
			bnCamLockedLeft = false;

	}

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY
	 * -----------------------------------------------------------------------------------------------------------
	 */

	bool isEqual(float a, float b) {

		return(Mathf.Abs(a - b) < 0.001f);
	}
}
