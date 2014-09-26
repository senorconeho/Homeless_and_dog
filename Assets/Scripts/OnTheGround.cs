using UnityEngine;
using System.Collections;

/// <summary>
/// Simulates the collision with the 'ground' layer but with a trigger
/// collider
/// </summary>
public class OnTheGround : MonoBehaviour {
	
	Rigidbody2D rb;

	void Awake () {
	
		rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
	
	}

	public void OnTriggerEnter2D(Collider2D col) {

		Debug.Log("Collided with " + col.transform);
		Vector2 vNewVelocity = new Vector2(rb.velocity.x, -rb.velocity.y);
		rb.velocity = vNewVelocity;
	}
}
