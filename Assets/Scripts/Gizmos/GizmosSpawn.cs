using UnityEngine;
using System.Collections;

/// <summary>
/// Class just to show a pretty icon
/// </summary>
public class GizmosSpawn : MonoBehaviour {

	public Texture2D	_icon;

	void OnDrawGizmos() {

		if(_icon != null) {

			Gizmos.DrawIcon(transform.position, _icon.name, true);
		}
	}

}
