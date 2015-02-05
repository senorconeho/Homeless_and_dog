using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Class description
/// </summary>
[CustomEditor(typeof(RoomBuilder))]
public class RoomBuildEditor : Editor {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	private SerializedProperty	floorWidthInPixels;
	private SerializedProperty	offsetHorizontalFromObject;
	private SerializedProperty	offsetVerticalFromObject;

	private GameObject go;

	SpriteRenderer sprite;

	float gridX;
	Vector2 spriteSize_wu;
	Vector3 scale;
	float	tilesX;
	float	totalWidth;
	BoxCollider2D col;
	GameObject tilesParent;
	
	// GUI Text Messages
	private static GUIContent widthContent = new GUIContent("Floor Width", "Floor width in pixels");
	private static GUIContent offsetHorizontalContent = new GUIContent("Offset X", "Offset from the object");
	private static GUIContent offsetVerticalContent = new GUIContent("Offset Y", "Offset from the object");
	private static GUIContent buildContent = new GUIContent("Create floor",	"Build the floor for this room");

	// GUI Formatting


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	public void OnEnable() {

		floorWidthInPixels = serializedObject.FindProperty("widthInPixels");
		offsetHorizontalFromObject = serializedObject.FindProperty("offsetHorizontal");
		offsetVerticalFromObject = serializedObject.FindProperty("offsetVertical");
	}

	/// <summary>
	/// Here where the field is actually show on the screen
	/// <\summary>
	public override void OnInspectorGUI() {

		// Update the serializedProperty - always do this in the beginning of OnInspectorGUI
		serializedObject.Update();

		// Draws standart complete inspector layout and properties
		DrawDefaultInspector();

		// Draw the sprite field
		EditorGUILayout.PropertyField(floorWidthInPixels, widthContent);
		EditorGUILayout.PropertyField(offsetHorizontalFromObject, offsetHorizontalContent);
		EditorGUILayout.PropertyField(offsetVerticalFromObject, offsetVerticalContent);

		// Draw the build button
		EditorGUILayout.Space();
		if(GUILayout.Button(buildContent)) {

			CreateTiles();
			AddCollider();
			AddLimits();
		}

		// Update SerializedObject
		serializedObject.ApplyModifiedProperties();
	}
	
	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>
	void CreateTiles() {
		// Get the current gameObject
		go = Selection.activeGameObject;
		// Get the sprite renderer
		sprite = go.GetComponent<SpriteRenderer>();
		gridX = floorWidthInPixels.floatValue;

		// Check if the object already exist. If so, destroy them
		// Check the 'RoomFloor' object
		Transform trOldRoomFloor = go.transform.Find("RoomFloor");
		if(trOldRoomFloor != null) {

			DestroyImmediate(trOldRoomFloor.gameObject);
		}
		Transform trOldLimits = go.transform.Find("Limits");
		if(trOldLimits != null) {

			DestroyImmediate(trOldLimits.gameObject);
		}

		// Create the floor
		spriteSize_wu = new Vector2(sprite.bounds.size.x / go.transform.localScale.x,
				sprite.bounds.size.y / go.transform.localScale.y);
		scale = Vector3.one;
		
		if(gridX != 0.0f) {

			tilesX = gridX / sprite.sprite.rect.width; // How many tiles fit in gridX pixels
			spriteSize_wu.x = tilesX;
		}

		tilesParent = new GameObject();
		tilesParent.transform.parent = go.transform;
		tilesParent.transform.name = "RoomFloor";
		tilesParent.transform.localPosition = new Vector3(offsetHorizontalFromObject.floatValue, offsetVerticalFromObject.floatValue, 0.0f);
		tilesParent.layer = 8; //FIXME

		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		childPrefab.transform.position = go.transform.position;
		childSprite.sprite = sprite.sprite;
		childSprite.sortingLayerID = sprite.sortingLayerID;
		childSprite.sortingOrder = sprite.sortingOrder;

		GameObject child;
		for(int i=0; i < (int)Mathf.Round(tilesX); i++) {

				child = Instantiate(childPrefab) as GameObject;
				child.transform.localScale = scale;
				child.transform.name = "tile_" + i;
				child.transform.parent = tilesParent.transform;
				child.transform.localPosition = new Vector3(sprite.bounds.size.x * i, 0, 0);
				//child.transform.position = go.transform.position + (new Vector3(sprite.bounds.size.x * i, 0, 0));
		}

		totalWidth = (int)Mathf.Round(tilesX) * sprite.bounds.size.x;

		DestroyImmediate(childPrefab);
		sprite.enabled = false;
	}

	/// <summary>
	/// <\summary>
	void AddCollider() {

		// Add a collider to the tiles
		col = tilesParent.AddComponent<BoxCollider2D>();
		col.size = new Vector2(totalWidth, sprite.bounds.size.y);
		col.center = new Vector2(totalWidth/2, -sprite.bounds.size.y/2);
	}

	/// <summary>
	/// <\summary>
	void AddLimits() {

		GameObject limitsParent = new GameObject();
		limitsParent.transform.parent = go.transform;
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
		goLeftLimit.layer = 8; // FIXME
		

		// Add the right limit
		GameObject goRightLimit = new GameObject();
		goRightLimit.transform.parent = limitsParent.transform;

		// Create the collider
		BoxCollider2D rightCol =	goRightLimit.AddComponent<BoxCollider2D>();
		rightCol.size = new Vector2(0.1f, 1.0f);
		rightCol.center = new Vector2(leftCol.size.x / 2, 0);

		goRightLimit.transform.localPosition = new Vector3(totalWidth,0,0);
		goRightLimit.transform.name = "RightRoomLimit";
		goRightLimit.layer = 8; //FIXME


	}

}
