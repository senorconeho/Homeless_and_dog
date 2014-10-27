using UnityEngine;
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
	public UILabel						uiBottomScreenLabel;
	public Transform 					trNoiseBar;
	public UISlider						uiNoiseBar;

	Player	playerScript;			//< Pointer to the player script

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY MAIN LOOP
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	///
	/// </summary>
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

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
		// Get all the UI components
		if(trUI == null)
			return;

		// Get the center screen label
		uiCenterScreenLabel = trUI.Find("Label_CenterScreen").gameObject.GetComponent<UILabel>();
		uiCenterScreenLabel.text = "";
		uiBottomScreenLabel = trUI.Find("Label_BottomScreen").gameObject.GetComponent<UILabel>();
		uiBottomScreenLabel.text = "";

		// Get the buttons labels
		Transform trButtonALabel = trUI.Find("Label_A");
		if(trButtonALabel != null) {

			uiButtonALabel = trButtonALabel.gameObject.GetComponent<UILabel>();
			uiButtonALabel.text = "";
		}

		Transform trButtonBLabel = trUI.Find("Label_B");
		if(trButtonBLabel != null) {
			uiButtonBLabel = trButtonBLabel.gameObject.GetComponent<UILabel>();
			uiButtonBLabel.text = "";
		}

		Transform trButtonASprite = trUI.Find("ButtonA/Sprite_ButtonA");
		if(trButtonASprite != null) {
			uiButtonAAnimation = trButtonASprite.gameObject.GetComponent<UISpriteAnimation>();
			uiButtonAAnimation.enabled = false;
		}

		Transform trButtonBSprite = trUI.Find("ButtonB/Sprite_ButtonB");
		if(trButtonBSprite != null) {
			uiButtonBAnimation = trButtonBSprite.gameObject.GetComponent<UISpriteAnimation>();
			uiButtonBAnimation.enabled = false;
		}


		// For the dude only: find the throw bar object
		trThrowBar = trUI.Find("ThrowBar");
		if(trThrowBar != null) {
			uiThrowBar = trThrowBar.gameObject.GetComponent<UISlider>();
			uiThrowBar.sliderValue = 0.0f;
			trThrowBar.gameObject.SetActive(false);
		}

		// Global HUD: get the 
		trNoiseBar = trUI.Find("NoiseBar");
		if(trNoiseBar != null) {

			uiNoiseBar = trNoiseBar.gameObject.GetComponent<UISlider>();
			uiNoiseBar.sliderValue = 0.0f;
		}

		// Find the "Game Over" sprite
		trSpriteGameOver = trUI.Find("SpriteGameOver");
		if(trSpriteGameOver != null)
			trSpriteGameOver.gameObject.SetActive(false);
	}
	
	/* -----------------------------------------------------------------------------------------------------------
	 * 
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Turn on the A button animation
	/// </summary>
	public void ButtonAAnimate(bool state) {

		if(uiButtonAAnimation != null)
			uiButtonAAnimation.enabled = state;
	}

	/// <summary>
	/// Activate the ThrowBar object for the dude player
	/// </summary>
	public void ThrowBarActivate() {

		if(uiThrowBar != null) {

			trThrowBar.gameObject.SetActive(true);
		}
	}

	/// <summary>
	/// Deactivate the throwbar object
	/// </summary>
	public void ThrowBarDeactivate() {

		if(uiThrowBar != null) {

			trThrowBar.gameObject.SetActive(false);
		}
	}

	/// <summary>
	/// Update the value on the throwbar
	/// </summary>
	/// <param name="fValue">the value to be shown on the slider (0..1)</param>
	public void ThrowBarUpdate(float fValue) {

		if(uiThrowBar != null) {

			if(!trThrowBar.gameObject.activeInHierarchy) {
				trThrowBar.gameObject.SetActive(true);
			}
			uiThrowBar.sliderValue = fValue;
		}
	}

	/// <summary>
	/// Update the value on the Noise Bar. This method is usually called from the game manager
	/// when the noise is up
	/// </summary>
	/// <param name="fValue">the value to be shown on the slider (0..1)</param>
	public void NoiseBarUpdate(float fValue) {

		if(uiNoiseBar != null) {

			uiNoiseBar.sliderValue = fValue;
		}
	}

	/// <summary>
	/// Activate the 'GameOver' sprite, showing it on the screen
	/// </summary>
	public void ShowGameOver() {

		trSpriteGameOver.gameObject.SetActive(true);
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetButtonsText(string stTextA, string stTextB) {

		if(stTextA != null)
			uiButtonALabel.text = stTextA;

		if(stTextB != null)
			uiButtonBLabel.text = stTextB;
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetBottomScreenText(string stText) {

		uiBottomScreenLabel.text = stText;
	}
}
