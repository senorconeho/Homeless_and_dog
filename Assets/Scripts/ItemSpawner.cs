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

	/// <summary>
	/// Instantiate a new item and return it
	/// </summary>
	/// <returns> Transform of the item instantiated, null if something went wrong</returns>
	public Transform GenerateNewItem() {

		Transform trItem = null;

		if(prefabItem != null) {

			trItem = Instantiate(prefabItem, trSpawnPoint.position, prefabItem.transform.rotation) as Transform;
		}

		return trItem;
	}

}
