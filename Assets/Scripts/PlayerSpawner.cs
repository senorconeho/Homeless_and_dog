﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Class description
/// </summary>
public class PlayerSpawner : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	[SerializeField]	public Transform	dogPrefab;
	[SerializeField]	public Transform	dudePrefab;
	Transform	trDogSpawnPoint;
	Transform	trDudeSpawnPoint;
	MainGame	gameScript;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	public void SpawnPlayers() {

		gameScript = GetComponent<MainGame>();

		// Find the spawn points
		trDogSpawnPoint = GameObject.Find("DogSpawner").transform;
		trDudeSpawnPoint = GameObject.Find("HomelessSpawner").transform;

		if(dogPrefab != null) {

			// Instantiate the dog object in the game
			Transform trDog = Instantiate(dogPrefab, trDogSpawnPoint.position, dogPrefab.rotation) as Transform;
			trDog.name = "Dog";	// Remove the 'clone' from the object's name
			gameScript.trDog = trDog;

			Player playerScript = trDog.gameObject.GetComponent<Player>();
			playerScript.trSpawnPoint = trDogSpawnPoint;

			// Correct the sprite facing rotation
			if(trDogSpawnPoint.rotation.y == 1) { // Facing Left, 180 degrees

				playerScript.FaceLeft();
			}
		}
		if(dudePrefab != null) {

			// Instantiate the dog object in the game
			Transform trDude = Instantiate(dudePrefab, trDudeSpawnPoint.position, dudePrefab.rotation) as Transform;
			trDude.name = "Homeless";
			gameScript.trDude = trDude;

			Player playerScript = trDude.gameObject.GetComponent<Player>();
			playerScript.trSpawnPoint = trDudeSpawnPoint;
		}
	}

	/// <summary>
	/// <\summary>
	public Transform GetDudeSpawnPoint() {

		return trDudeSpawnPoint;
	}

	/// <summary>
	/// <\summary>
	public Transform GetDogSpawnPoint() {

		return trDogSpawnPoint;
	}
}
