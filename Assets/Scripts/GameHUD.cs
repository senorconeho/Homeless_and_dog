using UnityEngine;
using System.Collections;

/// <summary>
/// Provide the in-game HUD for the player, using NGUI
/// <summary>
public class GameHUD : MonoBehaviour {

	[SerializeField]	public Transform 	trUI;	//< Drop the panel transform here
	Transform					trButtonALabel;
	Transform					trButtonBLabel;
	Transform					trButtonASprite;
	Transform					trButtonBSprite;
	UILabel						uiButtonALabel;
	UILabel						uiButtonBLabel;
	UISpriteAnimation	uiButtonAAnimation;
	UISpriteAnimation	uiButtonBAnimation;
	Transform					trThrowBar;
	UISlider					uiThrowBar;
	Transform					trSpriteGameOver;
	[HideInInspector] public UILabel		uiCenterScreenLabel;
	UILabel						uiBottomScreenLabel;
	UILabel						uiTopScreenLabel;
	Transform 				trNoiseBar;
	UISlider					uiNoiseBar;
	Transform					trFireLevel;
	Transform					trFireLabel;
	UILabel						uiFireLevelLabel;
	Transform					trGameButtonsPanel;

	Player						playerScript;			//< Pointer to the player script

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

			trUI = GameObject.Find("/UI Root (2D)/CameraRight/PanelDog").gameObject.transform;
		}
		if(trUI == null && playerScript.playerType == MainGame.ePlayerType.DUDE) {

			trUI = GameObject.Find("/UI Root (2D)/CameraLeft/PanelDude").gameObject.transform;
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
		uiTopScreenLabel = trUI.Find("Label_TopScreen").gameObject.GetComponent<UILabel>();
		uiTopScreenLabel.text = "";
		uiCenterScreenLabel = trUI.Find("Label_CenterScreen").gameObject.GetComponent<UILabel>();
		uiCenterScreenLabel.text = "";
		uiBottomScreenLabel = trUI.Find("Label_BottomScreen").gameObject.GetComponent<UILabel>();
		uiBottomScreenLabel.text = "";

		// Get the buttons labels
		trButtonALabel = trUI.Find("Label_A");
		if(trButtonALabel != null) {

			uiButtonALabel = trButtonALabel.gameObject.GetComponent<UILabel>();
			uiButtonALabel.text = "";
		}

		trButtonBLabel = trUI.Find("Label_B");
		if(trButtonBLabel != null) {
			uiButtonBLabel = trButtonBLabel.gameObject.GetComponent<UILabel>();
			uiButtonBLabel.text = "";
		}

		trButtonASprite = trUI.Find("ButtonA/Sprite_ButtonA");
		if(trButtonASprite != null) {
			uiButtonAAnimation = trButtonASprite.gameObject.GetComponent<UISpriteAnimation>();
			uiButtonAAnimation.enabled = false;
		}

		trButtonBSprite = trUI.Find("ButtonB/Sprite_ButtonB");
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

		// Global HUD: get the fire level to be shown on the HUD
		trFireLevel = trUI.Find("FireLevel");

		if(trFireLevel != null) {
			trFireLabel = trFireLevel.Find("FireLabel");
		}
		if(trFireLabel != null) {

			uiFireLevelLabel = trFireLabel.gameObject.GetComponent<UILabel>();
		}

		// Find the "Game Over" sprite
		trSpriteGameOver = trUI.Find("SpriteGameOver");
		if(trSpriteGameOver != null)
			trSpriteGameOver.gameObject.SetActive(false);

		// Find the panel which explain the game controls
		trGameButtonsPanel = trUI.Find("Panel-GameButtons");
		if(trGameButtonsPanel != null)
			trGameButtonsPanel.gameObject.SetActive(false);
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
	/// Updates the label which shown the current fire level
	/// </summary>
	public void FireLevelUpdate(string stText) {

		if(uiFireLevelLabel == null)
			return;

		uiFireLevelLabel.text = stText;
	}

	/// <summary>
	/// Activate the 'GameOver' sprite, showing it on the screen
	/// </summary>
	public void ShowGameOver() {

		trSpriteGameOver.gameObject.SetActive(true);
		SetBottomScreenText("Press start to play again");
		ActivateButtonIcons(false); // Disable the buttons icons
		ActivateButtonLabels(false); // Disable the buttons labels
		ActivateNoiseBar(false); // Disable the noise stuff
		ActivateFireLevel(false); // Disable the fire level meter
	}

	/// <summary>
	/// Activate the 'Level Won' state
	/// </summary>
	public void ShowLevelWon() {

		ActivateButtonIcons(false); // Disable the buttons icons
		ActivateButtonLabels(false); // Disable the buttons labels
		SetTopScreenText("Survived for another day..."); // Set the message
		ActivateNoiseBar(false); // Disable the noise stuff
		ActivateFireLevel(false); // Disable the fire level meter

		// TODO: disable the thunder sound
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

	/// <summary>
	/// 
	/// </summary>
	public void SetCenterScreenText(string stText) {

		uiCenterScreenLabel.text = stText;
	}

	/// <summary>
	/// 
	/// </summary>
	public void SetTopScreenText(string stText) {

		uiTopScreenLabel.text = stText;
	}

	/// <summary>
	/// Enable or disable the buttons icons on the screen
	/// </summary>
	/// <param name="bnNewStatus">True to activate the game object (showing the sprite on the screen), false to hide it</param>
	public void ActivateButtonIcons(bool bnNewStatus) {

		trButtonASprite.gameObject.SetActive(bnNewStatus);
		trButtonBSprite.gameObject.SetActive(bnNewStatus);
	}

	/// <summary>
	/// Enable or disable the buttons labels on the screen
	/// </summary>
	/// <param name="bnNewStatus">True to activate the game object (showing the text on the screen), false to hide it</param>
	public void ActivateButtonLabels(bool bnNewStatus) {

		trButtonALabel.gameObject.SetActive(bnNewStatus);
		trButtonBLabel.gameObject.SetActive(bnNewStatus);
	}

	/// <summary>
	/// Enable or disable the noise bar on the screen
	/// </summary>
	/// <param name="bnNewStatus"> A boolean, true to activate the object, false otherwise </param>
	public void ActivateNoiseBar(bool bnNewStatus) {

		trNoiseBar.gameObject.SetActive(bnNewStatus);
	}

	/// <summary>
	/// Enable or disable the fire level on the screen
	/// </summary>
	/// <param name="bnNewStatus"> A boolean, true to activate the object, false otherwise </param>
	public void ActivateFireLevel(bool bnNewStatus) {

		if(trFireLevel != null)
			trFireLevel.gameObject.SetActive(bnNewStatus);
	}

	/// <summary>
	/// Show or hide the panel showing the game buttons
	/// </summary>
	public void ActivateGameButtonsPanel(bool bnNewStatus) {

		if(trGameButtonsPanel != null)
			trGameButtonsPanel.gameObject.SetActive(bnNewStatus);
	}

	/// <summary>
	/// Call the coroutine to show the panel and then hide it after some time
	/// </summary>
	public void ActivateGameButtonsPanelForSomeTime() {

		StartCoroutine(ShowGameButtonsForSomeTime());
	}

	/// <summary>
	/// Show the game buttons panel for some time and the disappear
	/// </summary>
	IEnumerator ShowGameButtonsForSomeTime() {

		ActivateGameButtonsPanel(true);

		yield return new WaitForSeconds(2.5f);

		ActivateGameButtonsPanel(false);
	}


}
