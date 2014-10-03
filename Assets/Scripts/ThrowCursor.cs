using UnityEngine;
using System.Collections;

/// <summary>
///	Makes the cursor rotating over the homeless dude
/// </summary>
public class ThrowCursor : MonoBehaviour {

	float fMinAngle = 0;
	float fMaxAngle = 90;
	public float fCurrentAngle;
	float fIncrementAngle = 120.0f;
	float	fRadius = 0.2f;
	public Vector3	vNewPosition;
	public Vector2 	vOffset;


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
	
		// Check if the sprite is 'looking' to the left
		//if(this.transform.parent.transform.localScale.x < 0) {
		//	// Dude looking to the left
		//	fMinAngle = 90.0f;
		//	fMaxAngle = 180.0f;
		//}
		//else {
		//	fMinAngle = 0.0f;
		//	fMaxAngle = 90.0f;
		//}

		// Check if we need to reverse the cursors movement
		if(fCurrentAngle >= fMaxAngle || fCurrentAngle <= fMinAngle) {

			fCurrentAngle = Mathf.Clamp(fCurrentAngle, fMinAngle, fMaxAngle);
			fIncrementAngle *= -1;
		}

		// Calculate the new position
		vNewPosition = new Vector3(vOffset.x + (fRadius * Mathf.Cos(fCurrentAngle * Mathf.Deg2Rad)), 
				vOffset.y + (fRadius * Mathf.Sin(fCurrentAngle * Mathf.Deg2Rad)),
				0.0f);

		// Move the cursor to the new position
		transform.localPosition = vNewPosition;

		// Update the rotation angle
		fCurrentAngle += (fIncrementAngle * Time.deltaTime);
	}

	/// <summary>
	/// Return the current lauching angle
	/// </summary>
	/// <returns> The current angle, between 0 and 90 degrees </returns>
	public float GetCurrentAngle() {

		return fCurrentAngle;
	}
}
