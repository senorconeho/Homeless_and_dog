using UnityEngine;
using System.Collections;

/// <summary>
///
/// </summary>
public class Player : MonoBehaviour {

	public MainGame.ePlayerType playerType;	//< (from MainGame)
	public bool 								bnPickedUp = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// 
	/// </summary>
	void CollisionWithDude() {

		// Dog with dude? 
		if(playerType == MainGame.ePlayerType.DOG) {

		}
	}
}
