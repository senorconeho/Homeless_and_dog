using UnityEngine;
using System.Collections;

/// <class>
/// Attempt to autoadjust the camera setting to not distort the gameboy' screen aspect ratio.
/// Gameboy screen: 160px x 144px, scaled or not
/// First, we have to set the orthographic camera size, and them update the x,y,w and h values on the 
/// 'ViewportRect' property
/// New method: 
/// 1 - keep the cameras size at 0.72f
/// 2 - change the viewport rectangle
/// 2.1 - x = (((Screen.width/2) - (160 * gameScale)) / 2) / Screen.width -> (Screen.width - 320 * gameScale) / (4 * Screen.width)
/// 2.2 - w = (gameScale * 160) / ( Screen.width / 2);
/// 2.3 - y = ((Screen.height - 144 * gameScale) / 2) / Screen.height -> 0.5f - ((72 * gameScale) / Screen.height)
/// 2.4 - h = (gameScale * 144) / Screen.height;
/// 3 - Tag all cameras as 'CameraDog' or 'CameraDude'
/// </class>
public class AutoAdjustCamera : MonoBehaviour {

	public float 		fOriginalScreenWidth = 160;
	public float 		fOriginalScreenHeight = 144;
	public int			nGameScale = 2;

	//public int nScreenHeight;
	//public int nScreenWidth;
	//public float fAspectRatio;

	//public float fCameraSize;

	public Rect		rectLeftCamera;
	public Rect		rectRightCamera;

	float	fRectX;
	float	fRectW;
	float	fRectY;
	float	fRectH;

	Camera[]	cams;

	// Use this for initialization
	void Start () {

		// Get all cameras
		cams = Camera.allCameras;	

		// Calculate the game scale accordingly to the screen's resolution
		nGameScale = Mathf.FloorToInt(Mathf.Min(Screen.width / (fOriginalScreenWidth * 2), Screen.height / fOriginalScreenHeight));

		// Get the current screen info
		//nScreenHeight = Screen.height;
		//nScreenWidth = Screen.width;

		//fAspectRatio = (float) nScreenWidth/nScreenHeight;

		//fCameraSize = 1.6f / fAspectRatio;

		// Calculate the viewport rectangle
		//fRectX = .25f - (fOriginalScreenWidth/2 * nGameScale/Screen.width);
		//fRectW = (fOriginalScreenWidth * nGameScale) / Screen.width;
		fRectX = ((Screen.width / 2) - (nGameScale * fOriginalScreenWidth)) /2;
		fRectW = fOriginalScreenWidth * nGameScale;

		fRectY = (Screen.height - (fOriginalScreenHeight * nGameScale)) / 2;
		fRectH = fOriginalScreenHeight * nGameScale; 
	
		//
		rectLeftCamera.x = fRectX;
		rectRightCamera.x = fRectX + (Screen.width / 2);
		rectLeftCamera.width = rectRightCamera.width = fRectW;
		rectLeftCamera.y = rectRightCamera.y = fRectY;
		rectLeftCamera.height = rectRightCamera.height = fRectH;



		// Adjust all cameras size accordingly to the screen resolution
		foreach(Camera cam in cams) {
		
			if(cam.gameObject.tag == "CameraDog") {

				cam.pixelRect = rectLeftCamera;
			}
			if(cam.gameObject.tag == "CameraDude") {

				cam.pixelRect = rectRightCamera;
			}
			//cam.orthographicSize = fCameraSize;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
