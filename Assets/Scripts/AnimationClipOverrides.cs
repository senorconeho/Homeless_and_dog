using UnityEngine;
using System.Collections;

/// <summary>
/// Class description
/// </summary>
public class AnimationClipOverrides : MonoBehaviour {

	[System.Serializable]
	private class AnimationClipOverride {

		public string clipNamed;
		public AnimationClip overrideWith;
	}

	[System.Serializable]
	private class AnimationOverrideItem {

		public string itemName;
		public AnimatorOverrideController overrideController;
		public AnimationClipOverride[] clipOverrides;
	}

	//[SerializeField] AnimationClipOverride[] clipOverrides;
	[SerializeField] AnimationOverrideItem[] itemClipOverrides;

	public RuntimeAnimatorController originalRuntimeController;

	//public AnimatorOverrideController overrideController;

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC

	// PROTECTED


	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */

	//public void Init(Animator animator, string stItemName) {

	//	//AnimatorOverrideController overrideController = new AnimatorOverrideController();
	//	overrideController = new AnimatorOverrideController();
	//	overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

	//	foreach(AnimationClipOverride clipOverride in clipOverrides) {

	//			overrideController[clipOverride.clipNamed] = clipOverride.overrideWith;
	//	}

	//	animator.runtimeAnimatorController = overrideController;

	//}

	public void ChangeAnimation(Animator animator, string stItemName) {
		
		// Find the item
		foreach(AnimationOverrideItem itemOverride in itemClipOverrides) {

			if(itemOverride.itemName == stItemName) {

				if(itemOverride != null && itemOverride.overrideController == null) {

					// Create the controller
					itemOverride.overrideController = new AnimatorOverrideController();
					itemOverride.overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
					if(originalRuntimeController == null)
						originalRuntimeController = animator.runtimeAnimatorController;

					// Populate with the animations
					foreach(AnimationClipOverride clipOverride in itemOverride.clipOverrides) {

						itemOverride.overrideController[clipOverride.clipNamed] = clipOverride.overrideWith;
					}
				}

				animator.runtimeAnimatorController = itemOverride.overrideController;
			}
		}
	}

	public void RestoreAnimation(Animator animator) {

		animator.runtimeAnimatorController = originalRuntimeController;
	}

	/// <summary>
	/// <\summary>
	void Awake() {

	}
	
	/// <summary>
	/// Use this for initialization
	/// <\summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// <\summary>
	void Update () {
	
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// <\summary>

}
