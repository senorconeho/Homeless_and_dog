using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Level Control: adjust all the initial parameters of the level
/// </summary>
public class LevelControl : MonoBehaviour {

	public float	fLevelBurnSpeed;	//< How many seconds the barrel will take to burn from 100% to 0% in this level? Lower values means harder levels
	public float	fStartFireLevel;	//< The starting level of the fire (ranging from 0 to 1)

	public bool		bnIsRaining;			//< turns the rain on or off
	public float	fRainIntensityOverFire;	//< How faster the fire will extinguish when raining? 10% faster? So this value must be 0.1f and so on

	[SerializeField]
	public Transform	barrelPrefab;
	Transform	trBarrelSpawner;

	Transform					trBarrel;
	Barrel 						barrelScript;
	RainControl				rainScript;
	MainGame					gameScript;


	void Awake() {

		// Get the main game script
		gameScript = GameObject.Find("GameManager").gameObject.GetComponent<MainGame>();
		gameScript.RegisterLevelController(this.transform, this);

		// Get the barrel placement object
		trBarrelSpawner = transform.Find("BarrelSpawner");
		CreateAndConfigureTheBarrel();

		// Rain
		rainScript = gameObject.GetComponent<RainControl>();
		ConfigureRain();
	}

	/// <summary>
	/// Create the barrel on the place indicated and configure it with the level values
	/// </summary>
	public void CreateAndConfigureTheBarrel() {

		if(barrelPrefab == null) {

			Debug.LogError("No barrel prefab defined in the inspector!");
			return;
		}

		trBarrel = Instantiate(barrelPrefab, trBarrelSpawner.position, barrelPrefab.rotation) as Transform;
		barrelScript = trBarrel.gameObject.GetComponent<Barrel>();
		
		// Configure the barrel
		barrelScript.SetFireRate(fLevelBurnSpeed);
		barrelScript.SetFireLevel(fStartFireLevel);

	}


	/// <summary>
	//
	/// </summary>
	public void ConfigureRain() {

		rainScript.Raining(bnIsRaining);

		if(bnIsRaining) {

			// Is raining? So the fires extinguishes faster...
			barrelScript.SetFireRate( barrelScript.GetFireRate() * (1-fRainIntensityOverFire) );
		}
	}


	/// <summary>
	///
	/// </summary>
	public Transform GetBarrel() {

		return trBarrel;
	}
}
