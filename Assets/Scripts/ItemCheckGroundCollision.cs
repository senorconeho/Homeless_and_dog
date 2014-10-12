using UnityEngine;
using System.Collections;

/// <summary>
/// Check if the item hit the ground with speed, making noise
/// </summary>
public class ItemCheckGroundCollision : MonoBehaviour {

	float fCollisionVelocityThreshold = 2.5f;	//< Minimum collision velocity to be accounted as noisy
	float fItemNoise = .25f;
	public MainGame	gameScript;

	void Awake() {

		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
	}

	/// <summary>
	///
	/// </summary>
	void CrashedWithTheGround() {

		if(gameScript != null) {

			gameScript.AddNoise(fItemNoise);
		}
	}


	/// <summary>
	///
	/// </summary>
	void OnCollisionEnter2D(Collision2D collision) {

		if(collision.transform.gameObject.layer == MainGame.nGroundLayer) {

			if(collision.relativeVelocity.magnitude > fCollisionVelocityThreshold) {

				CrashedWithTheGround();
				// DEBUG
				Debug.Log("collision with ground! " + collision.relativeVelocity.magnitude);
			}
		}
	}
}
