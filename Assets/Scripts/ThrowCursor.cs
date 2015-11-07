using UnityEngine;
using System.Collections;

/// <summary>
///	Makes the cursor rotating over the homeless dude
/// </summary>
public class ThrowCursor : MonoBehaviour {

	float fMinAngle = 0;
	float fMaxAngle = 90;
	public float fCurrentAngle;
	float fIncrementAngle = 90.0f;
	float	fRadius = 0.2f;
	public Vector3	vNewPosition;
	public Vector2 	vOffset;
	public Vector2 	vDirection;


	void OnEnable() {

		fCurrentAngle = fMinAngle;
	}

	void OnDisable() {

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// FIXME: shouldn't all inputs be together?
		// INPUT STUFF
		if(Input.GetKey(KeyCode.W)) { // Dude's key up
			// Update the rotation angle
			fCurrentAngle += (fIncrementAngle * Time.deltaTime);
		}
		if(Input.GetKey(KeyCode.S)) {

			// Update the rotation angle
			fCurrentAngle -= (fIncrementAngle * Time.deltaTime);
		}

		fCurrentAngle = Mathf.Clamp(fCurrentAngle, fMinAngle, fMaxAngle);
		
		// Calculate the new position
		vNewPosition = new Vector3(vOffset.x + (fRadius * Mathf.Cos(fCurrentAngle * Mathf.Deg2Rad)), 
				vOffset.y + (fRadius * Mathf.Sin(fCurrentAngle * Mathf.Deg2Rad)),
				0.0f);

		// Move the cursor to the new position
		transform.localPosition = vNewPosition;

	}

	/// <summary>
	/// Return the current lauching angle
	/// </summary>
	/// <returns> The current angle, between 0 and 90 degrees </returns>
	public float GetCurrentAngle() {

		return fCurrentAngle;
	}

	public Vector3 GetCursorDirection() {

		return vNewPosition.normalized;
	}
}
