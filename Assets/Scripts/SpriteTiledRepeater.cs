using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]

/// <summary>
/// Class description
/// </summary>
public class SpriteTiledRepeater : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public float gridX = 1.0f;

	SpriteRenderer sprite;

	Vector2 spriteSize_wu;
	Vector3 scale;
	float	pixelToUnits;

	float	tilesX;
	float	totalWidth;

	BoxCollider2D col;

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	void Awake() {

		CreateTiles();
		AddCollider();
		AddLimits();
	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>
	void CreateTiles() {
		sprite = GetComponent<SpriteRenderer>();
		pixelToUnits = sprite.sprite.rect.width / sprite.sprite.bounds.size.x;

		spriteSize_wu = new Vector2(sprite.bounds.size.x / transform.localScale.x,
				sprite.bounds.size.y / transform.localScale.y);
		scale = Vector3.one;
		
		if(gridX != 0.0f) {

			tilesX = gridX / sprite.sprite.rect.width; // How many tiles fit in gridX pixels
			spriteSize_wu.x = tilesX;
		}

		GameObject tilesParent = new GameObject();
		tilesParent.transform.parent = transform;
		tilesParent.transform.name = "StaticScenario";
		tilesParent.transform.localPosition = Vector3.zero;

		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		childPrefab.transform.position = transform.position;
		childSprite.sprite = sprite.sprite;
		childSprite.sortingLayerID = sprite.sortingLayerID;
		childSprite.sortingOrder = sprite.sortingOrder;

		GameObject child;
		for(int i=0; i < (int)Mathf.Round(tilesX); i++) {

				child = Instantiate(childPrefab) as GameObject;
				child.transform.position = transform.position + (new Vector3(sprite.bounds.size.x * i, 0, 0));
				child.transform.localScale = scale;
				child.transform.parent = tilesParent.transform;
				child.transform.name = "tile_" + i;
		}

		totalWidth = (int)Mathf.Round(tilesX) * sprite.bounds.size.x;

		Destroy(childPrefab);
		sprite.enabled = false;

	}

	/// <summary>
	/// <\summary>
	void AddCollider() {

		// Add a collider to the tiles
		col = gameObject.AddComponent<BoxCollider2D>();
		col.size = new Vector2(totalWidth, sprite.bounds.size.y);
		col.center = new Vector2(totalWidth/2, -sprite.bounds.size.y/2);
	}

	/// <summary>
	/// <\summary>
	void AddLimits() {

		GameObject limitsParent = new GameObject();
		limitsParent.transform.parent = transform;
		limitsParent.transform.name = "Limits";
		limitsParent.transform.localPosition = Vector3.zero;

		// Create the limit prefab
		GameObject goLeftLimit = new GameObject();
		goLeftLimit.transform.parent = limitsParent.transform;

		// Create the collider
		BoxCollider2D leftCol =	goLeftLimit.AddComponent<BoxCollider2D>();
		leftCol.size = new Vector2(0.1f, 1.0f);
		leftCol.center = new Vector2(-leftCol.size.x / 2, 0);

		goLeftLimit.transform.localPosition = new Vector3(0 ,0,0);
		goLeftLimit.transform.name = "LeftRoomLimit";
		

		// Add the right limit
		GameObject goRightLimit = new GameObject();
		goRightLimit.transform.parent = limitsParent.transform;

		// Create the collider
		BoxCollider2D rightCol =	goRightLimit.AddComponent<BoxCollider2D>();
		rightCol.size = new Vector2(0.1f, 1.0f);
		rightCol.center = new Vector2(leftCol.size.x / 2, 0);

		goRightLimit.transform.localPosition = new Vector3(totalWidth,0,0);
		goRightLimit.transform.name = "RightRoomLimit";


	}

}
