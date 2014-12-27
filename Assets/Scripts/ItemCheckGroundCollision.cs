﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Check if the item hit the ground with speed, making noise
/// </summary>
public class ItemCheckGroundCollision : MonoBehaviour {

	float fCollisionVelocityThreshold = 2.5f;	//< Minimum collision velocity to be accounted as noisy
	float fItemNoise = .25f;

	public MainGame	gameScript;
	public Item			itemScript;

	void Awake() {

		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
		itemScript = transform.parent.transform.gameObject.GetComponent<Item>();
	}

	/// <summary>
	///
	/// </summary>
	void CrashedWithTheGround() {

		if(gameScript != null && itemScript != null) {

			gameScript.AddNoise(fItemNoise);
			itemScript.Crashed();
		}
	}


	/// <summary>
	///
	/// </summary>
	void OnCollisionEnter2D(Collision2D collision) {
		
		if(itemScript.bnPickedUp == true)
			return;

		if(collision.transform.gameObject.layer == MainGame.nGroundLayer) {

			// DEBUG
			//Debug.Log("collision without noise: " + collision.relativeVelocity.y);
			
			if(collision.relativeVelocity.magnitude > fCollisionVelocityThreshold) {

				CrashedWithTheGround();
				// DEBUG
				//Debug.Log("collision with ground! " + collision.relativeVelocity.magnitude + " " + collision.transform);
			}
			else {

				// DEBUG
				//Debug.Log("collision without noise: " + collision.relativeVelocity.magnitude);
			}
		}
	}
}
