using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour {

	[SerializeField] public Transform	prefabItem;
	public Transform  trSpawnPoint;
	public Transform	trItemGenerated;


	// Use this for initialization
	void Start () {
	
		trSpawnPoint = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// </summary>
	public void GenerateItem() {

		if(prefabItem != null) {

			trItemGenerated = Instantiate(prefabItem, trSpawnPoint.position, prefabItem.transform.rotation) as Transform;
		}
	}

	/// <summary>
	/// </summary>
	public Transform GetItemGenerated() {

		return trItemGenerated;
	}
}
