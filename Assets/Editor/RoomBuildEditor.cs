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
	private SerializedProperty	floorSprite;
	private SerializedProperty	floorWidthInPixels;

	private SpriteRenderer	m_SpriteRenderer;
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
	private static GUIContent spriteContent = new GUIContent("Sprite", "Sprite to be tiled");
	private static GUIContent widthContent = new GUIContent("Floor Width", "Floor width in pixels");
	private static GUIContent buildContent = new GUIContent("Build room",	"Build the room stuff");
	private static GUIContent deleteContent = new GUIContent("Clean room", "Delete room stuff");

	// GUI Formatting


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// <\summary>
	public void OnEnable() {

		floorSprite = serializedObject.FindProperty("floorSprite");
		floorWidthInPixels = serializedObject.FindProperty("widthInPixels");
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
		//EditorGUILayout.PropertyField(floorSprite, spriteContent);
		EditorGUILayout.PropertyField(floorWidthInPixels, widthContent);

		// Draw the build button
		EditorGUILayout.Space();
		if(GUILayout.Button(buildContent)) {

			CreateTiles();
			AddCollider();
			AddLimits();
		}
		EditorGUILayout.Space();
		if(GUILayout.Button(deleteContent)) {
			Debug.Log("Deleting Content");
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

		spriteSize_wu = new Vector2(sprite.bounds.size.x / go.transform.localScale.x,
				sprite.bounds.size.y / go.transform.localScale.y);
		scale = Vector3.one;
		
		if(gridX != 0.0f) {

			tilesX = gridX / sprite.sprite.rect.width; // How many tiles fit in gridX pixels
			spriteSize_wu.x = tilesX;
		}

		tilesParent = new GameObject();
		tilesParent.transform.parent = go.transform;
		tilesParent.transform.name = "StaticScenario";
		tilesParent.transform.localPosition = Vector3.zero;

		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		childPrefab.transform.position = go.transform.position;
		childSprite.sprite = sprite.sprite;
		childSprite.sortingLayerID = sprite.sortingLayerID;
		childSprite.sortingOrder = sprite.sortingOrder;

		GameObject child;
		for(int i=0; i < (int)Mathf.Round(tilesX); i++) {

				child = Instantiate(childPrefab) as GameObject;
				child.transform.position = go.transform.position + (new Vector3(sprite.bounds.size.x * i, 0, 0));
				child.transform.localScale = scale;
				child.transform.parent = tilesParent.transform;
				child.transform.name = "tile_" + i;
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
