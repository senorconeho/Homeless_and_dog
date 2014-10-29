using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the rain
/// </summary>
public class RainControl : MonoBehaviour {

	Animator[]	lightningAnimators;
	public Transform	trThunder;
	

	// Use this for initialization
	void Start () {
	
		// Find the thunder/lightning object
		trThunder = transform.Find("Thunder");

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
