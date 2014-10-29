using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the rain
/// </summary>
public class ThunderControl : MonoBehaviour {

	public Animator[]		lightningAnimators;
	public AudioClip		sfxThunder;
	Transform						tr;
	
	void Awake() {

		tr = this.transform;
	}


	// Use this for initialization
	void Start () {
	
		lightningAnimators = new Animator[tr.childCount];
		int nIdx = 0;

		// Get each of the animators of each lightning background
		foreach(Transform trChild in tr) {

			lightningAnimators[nIdx] = trChild.gameObject.GetComponent<Animator>();
			nIdx++;
		}

	}
	
	// Update is called once per frame
	void Update () {
	
		// DEBUG
		if(Input.GetMouseButtonUp(0))
			DoTheRoar();
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
}
