using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the rain
/// </summary>
public class RainControl : MonoBehaviour {

	Animator[]	lightningAnimators;
	Transform		trThunder;

	public bool	bnPlayThunder = true;	//< Play or not the thunder sfx and animation
	[SerializeField]
	public AudioClip		sfxThunder;
	public float				fThunderMinTime;
	public float				fThunderMaxTime;

	Transform		trRainDrops;
	float				fThunderTimer;
	
	// Use this for initialization
	void Start () {
	
		// Find the thunder/lightning object
		trThunder = transform.Find("Rain/Thunder");
		lightningAnimators = new Animator[trThunder.childCount];
		int nIdx = 0;

		// Get each of the animators of each lightning background
		foreach(Transform trChild in trThunder) {

			lightningAnimators[nIdx] = trChild.gameObject.GetComponent<Animator>();
			nIdx++;
		}

		//trRainDrops = transform.Find("Rain/RainDropsSprites");
		//trRainDrops.gameObject.SetActive(false);
		if(bnPlayThunder)
			StartCoroutine(TimedThunder());
	}
	
	/// <summary>
	/// Play the lightning animation and play the thunder sound effect
	/// </summary>
	public void DoTheRoar() {

		// Set the trigger to play the animation
		foreach(Animator ani in lightningAnimators) {

			ani.SetTrigger("triggerDoTheRoar");
		}

		// Play the sfx
		if(sfxThunder != null) {

			audio.PlayOneShot(sfxThunder);
		}
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="bnStatus"> a boolean: true to activate the rain sprites, false to deactivate them </param>
	public void Raining(bool bnStatus) {

		if(trRainDrops == null)
			trRainDrops = transform.Find("Rain/RainDropsSprites");

		// Activate or deactivate the rain sprites
		trRainDrops.gameObject.SetActive(bnStatus);
	}

	/// <summary>
	///
	/// </summary>
	IEnumerator TimedThunder() {

		float fTime = Random.Range(fThunderMinTime, fThunderMaxTime);
		
		while (true) {
			DoTheRoar();
			yield return new WaitForSeconds(fTime);
		 	fTime = Random.Range(fThunderMinTime, fThunderMaxTime);
			
			if(bnPlayThunder == false) {

				break;
			}
		}
	}

	public void StopTimedThunder() {

		bnPlayThunder = false;
		StopCoroutine(TimedThunder());
	}
}
