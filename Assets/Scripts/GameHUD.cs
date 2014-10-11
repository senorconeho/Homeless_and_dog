﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Provide the in-game HUD for the player, using NGUI
/// <summary>
public class GameHUD : MonoBehaviour {

	public Transform 	trUI;	//< Drop the panel transform here
	public UILabel						uiButtonALabel;
	public UILabel						uiButtonBLabel;
	public UISpriteAnimation	uiButtonAAnimation;
	public UISpriteAnimation	uiButtonBAnimation;
	public Transform					trThrowBar;
	public UISlider						uiThrowBar;
	public Transform					trSpriteGameOver;
	public UILabel						uiCenterScreenLabel;

	//
	Player	playerScript;

	void Awake() {

		playerScript = GetComponent<Player>();

		// find the panel object
		if(trUI == null && playerScript.playerType == MainGame.ePlayerType.DOG) {

			trUI = GameObject.Find("/UI Root (2D)/CameraLeft/PanelDog").gameObject.transform;
		}
		if(trUI == null && playerScript.playerType == MainGame.ePlayerType.DUDE) {

			trUI = GameObject.Find("/UI Root (2D)/CameraRight/PanelDude").gameObject.transform;
		}
	}


	// Use this for initialization
	void Start () {
	
		// Get all the UI components
		if(trUI == null)
			return;

		// Get the center screen label
		uiCenterScreenLabel = trUI.Find("Label_CenterScreen").gameObject.GetComponent<UILabel>();

		// Get the buttons labels
		uiButtonALabel = trUI.Find("Label_A").gameObject.GetComponent<UILabel>();
		uiButtonBLabel = trUI.Find("Label_B").gameObject.GetComponent<UILabel>();
		uiButtonAAnimation = trUI.Find("ButtonA/Sprite_ButtonA").gameObject.GetComponent<UISpriteAnimation>();
		uiButtonBAnimation = trUI.Find("ButtonB/Sprite_ButtonB").gameObject.GetComponent<UISpriteAnimation>();

		// Clean the buttons
		uiButtonALabel.text = "";
		uiButtonBLabel.text = "";

		// Stops any animation
		if(uiButtonAAnimation != null)
			uiButtonAAnimation.enabled = false;
		if(uiButtonBAnimation != null)
			uiButtonBAnimation.enabled = false;

		// For the dude only: find the throw bar object
		trThrowBar = trUI.Find("ThrowBar");
		if(trThrowBar != null) {
			uiThrowBar = trThrowBar.gameObject.GetComponent<UISlider>();
			uiThrowBar.sliderValue = 0.0f;
			trThrowBar.gameObject.SetActive(false);
		}

		// Find the "Game Over" sprite
		trSpriteGameOver = trUI.Find("SpriteGameOver");
		trSpriteGameOver.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonAAnimate(bool state) {

		if(uiButtonAAnimation != null)
			uiButtonAAnimation.enabled = state;
	}

	/// <summary>
	///
	/// </summary>
	public void ThrowBarActivate() {

		if(uiThrowBar != null) {

			trThrowBar.gameObject.SetActive(true);
		}
	}

	/// <summary>
	///
	/// </summary>
	public void ThrowBarDeactivate() {

		if(uiThrowBar != null) {

			trThrowBar.gameObject.SetActive(false);
		}
	}

	/// <summary>
	///
	/// </summary>
	public void ThrowBarUpdate(float fValue) {

		if(uiThrowBar != null) {

			if(!trThrowBar.gameObject.activeInHierarchy) {
				trThrowBar.gameObject.SetActive(true);
			}
			uiThrowBar.sliderValue = fValue;
		}
	}

	/// <summary>
	///
	/// </summary>
	public void ShowGameOver() {

		trSpriteGameOver.gameObject.SetActive(true);
	}
}
