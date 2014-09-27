using UnityEngine;
using System.Collections;

/// <summary>
/// Provide the in-game HUD for the player, using NGUI
/// <summary>
public class GameHUD : MonoBehaviour {

	public Transform 	trUI;	//< Drop the panel transform here
	public UILabel		uiButtonALabel;
	public UILabel		uiButtonBLabel;

	// Use this for initialization
	void Start () {
	
		// Get all the UI components
		if(trUI == null)
			return;

		// Get the A button label
		uiButtonALabel = trUI.Find("Label_A").gameObject.GetComponent<UILabel>();
		uiButtonBLabel = trUI.Find("Label_B").gameObject.GetComponent<UILabel>();

		// Clean the buttons
		uiButtonALabel.text = "";
		uiButtonBLabel.text = "";

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
