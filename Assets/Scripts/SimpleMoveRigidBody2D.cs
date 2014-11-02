using UnityEngine;
using System.Collections;

/// <summary>
/// Class name and description
/// </summary>
public class SimpleMoveRigidBody2D : MonoBehaviour
{

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	private bool bnFacingLeft = false;
	private Animator animator;
	[HideInInspector] public Transform trSprite;

	Vector2 vRigidbodyVelocity;
	// PRIVATE
	[HideInInspector] public bool bnAllowedToGetInput = true;
	[HideInInspector] public bool	bnCanMoveHorizontally = true;	//< cannot move while on the air

	// PROTECTED
	public float fMaxSpeed = 1f;	//< this value could (will) be changed by the Player script
	public float fMoveForce = 40f;		

	[HideInInspector] public 	MainGame.ePlayerType playerType;	//< from MainGame
	float 	fH;	//< Horizontal movement

	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */
	//
	void Awake ()
	{

	}

	// Use this for initialization
	void Start ()
	{

		animator = this.GetComponent<Animator> ();
		trSprite = this.transform;
	}

	// Update is called once per frame
	void Update ()
	{

	}

	// Physics
	void FixedUpdate ()
	{

		if(playerType == MainGame.ePlayerType.NPC) {

			DoNPCMovement();
			return;
		}

		if (bnAllowedToGetInput) {

			if(playerType == MainGame.ePlayerType.DOG && bnCanMoveHorizontally) {
				fH = Input.GetAxis ("Horizontal");
			}
			else if(playerType == MainGame.ePlayerType.DUDE && bnCanMoveHorizontally) {

				fH = Input.GetAxis ("Horizontal_2");
			}

			if (animator != null) {

				animator.SetFloat("fSpeed", Mathf.Abs(fH));
			}

			// Rigidbody stuff
			if(fH * rigidbody2D.velocity.x < fMaxSpeed) {

				// Add a force to the player
				rigidbody2D.AddForce(Vector2.right * fH * fMoveForce);
			}

			// If the player's horizontal velocity is greater than the maxSpeed
			if(Mathf.Abs(rigidbody2D.velocity.x) > fMaxSpeed) {

				rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * fMaxSpeed,
						rigidbody2D.velocity.y);
			}

			// Flip the sprite?
			if (fH < 0 && !bnFacingLeft) {

				FlipSprite ();
			} else if (fH > 0 && bnFacingLeft) {

				FlipSprite ();
			}
		}

		if (animator != null &&	playerType == MainGame.ePlayerType.DOG) {

			animator.SetFloat("vSpeed", Mathf.Abs(rigidbody2D.velocity.y));
		}


		//
		vRigidbodyVelocity = rigidbody2D.velocity;
	}
	/* ==========================================================================================================
	 * NPC STUFF
	 * ==========================================================================================================
	 */
	/// <summary>
	/// Do the NPC movement
	/// </summary>
	public void DoNPCMovement() {

		// fH value come from another script
		if (animator != null) {

			animator.SetFloat("fSpeed", Mathf.Abs(fH));
		}

		// Rigidbody stuff
		if(fH * rigidbody2D.velocity.x < fMaxSpeed) {

			// Add a force to the player
			rigidbody2D.AddForce(Vector2.right * fH * fMoveForce);
		}

		// If the player's horizontal velocity is greater than the maxSpeed
		if(Mathf.Abs(rigidbody2D.velocity.x) > fMaxSpeed) {

			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * fMaxSpeed,
					rigidbody2D.velocity.y);
		}

		// Flip the sprite?
		if (fH < 0 && !bnFacingLeft) {

			FlipSprite ();
		} else if (fH > 0 && bnFacingLeft) {

			FlipSprite ();
		}

		// Update
		vRigidbodyVelocity = rigidbody2D.velocity;
	}

	/// <summary>
	/// Moves the NPC to the left or the right. Actually set the 'fH' value, like the player
	/// is pressing left or right to move it. Accessed by the ResidentBehaviour script
	/// </summary>
	/// <param name="nDirection"> 1 to move to right, -1 to move to the left
	public void SetNPCMovementDirection(int nDirection) {

		fH = nDirection;
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	///
	/// </summary>
	public void FlipSprite ()
	{
		Vector3 v3SpriteScale = trSprite.localScale;
		v3SpriteScale.x *= -1;
		trSprite.localScale = v3SpriteScale;

		bnFacingLeft = !bnFacingLeft;
	}

	/// <summary>
	///
	/// </summary>
	public void LockMovement ()
	{

		bnAllowedToGetInput = false;
	}

	/// <summary>
	///
	/// </summary>
	public void UnlockMovement ()
	{

		bnAllowedToGetInput = true;
	}
}

