using UnityEngine;
using System.Collections;

/// <class>
/// Attempt to autoadjust the camera setting to not distort the gameboy' screen aspect ratio.
/// Gameboy screen: 160px x 144px, scaled or not
/// First, we have to set the orthographic camera size, and them update the x,y,w and h values on the 
/// 'ViewportRect' property
/// </class>
public class AutoAdjustCamera : MonoBehaviour {

	public int nScreenHeight;
	public int nScreenWidth;
	public float fAspectRatio;

	public float fCameraSize;

	Camera[]	cams;

	// Use this for initialization
	void Start () {

		// Get all cameras
		cams = Camera.allCameras;	

		// Get the current screen info
		nScreenHeight = Screen.height;
		nScreenWidth = Screen.width;

		fAspectRatio = (float) nScreenWidth/nScreenHeight;

		fCameraSize = 1.6f / fAspectRatio;

		// Adjust all cameras size accordingly to the screen resolution
		foreach(Camera cam in cams) {
		
			cam.orthographicSize = fCameraSize;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
