using UnityEngine;
using System.Collections;

/// <summary>
/// Simulates a ballistic launch
/// Used when the homeless dude throws the dog
public class BallisticLaunch : MonoBehaviour {

	public Transform trAim;
	public Transform dotPrefab;

	public Vector2 vVelocity0; // This value will bet set by the cursor script

	public int samples = 10;
	float fSampling = 0.1f; // Time between samples

	Transform[] trDots; 

	// Use this for initialization
	void Start () {

		// Get the starting point of throws
		trAim = MainGame.instance.dudeScript.trThrowPosition;
	
		trDots = new Transform[samples];
		
		for(int i=0; i<samples; i++) {
			trDots[i] = Instantiate(dotPrefab) as Transform;
			trDots[i].SetParent(this.transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
		int i = 0;
		while(i < samples) {

			Vector2 vPos = GetTrajectoryPoint(new Vector2(trAim.position.x, trAim.position.y), 
					vVelocity0, i * fSampling);
			Vector3 pos = new Vector3(vPos.x, vPos.y, -1.0f);
			trDots[i].position = pos;
			i++;
		}

	}

	/// <summary>
	/// Calculate the projectile position for a given time
	/// </summary>
	/// <returns> A vector2 with the projectile position</returns>
	Vector2 GetTrajectoryPoint(Vector2 vStartingPosition, Vector2 vStartingVelocity, float t) {

		return vStartingPosition + vStartingVelocity*t + Physics2D.gravity*t*t*0.5f;
	}

	/// <summary>
	/// </summary>
	void PlotTrajectory(Vector2 vStartingPosition, Vector2 vStartingVelocity, float fMaxTime) {

		Vector2 prev = vStartingPosition;

	}
}
