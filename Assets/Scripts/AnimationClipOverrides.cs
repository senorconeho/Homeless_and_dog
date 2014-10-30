using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// One solution to the homeless dude animation, which have an idle and walking animation for each 
/// item that can be carried
/// How does it work? For each item, we create an array of the idle and walking animation, and then we create
/// an array of these information, indexed by the item name.
/// When the Player.cs pick an item, we create (or use the previously created) an AnimationClipOverride, and 
/// change the controller to this new overrider. When another item is picked, we then change the controller
/// again to the new one, and so forth.
/// </summary>
public class AnimationClipOverrides : MonoBehaviour {

	[System.Serializable]
	private class AnimationClipOverride {

		public string clipNamed;
		public AnimationClip overrideWith;
	}

	[System.Serializable]
	private class AnimationOverrideItem {

		public MainGame.eItemTypes itemType;
		public AnimatorOverrideController overrideController;
		public AnimationClipOverride[] clipOverrides = new AnimationClipOverride[2];	// only 2 states: idle and run
	}

	//[SerializeField] AnimationClipOverride[] clipOverrides;
	[SerializeField] AnimationOverrideItem[] itemClipOverrides = new AnimationOverrideItem[6]; // number of items + dog

	// IMPORTANT! We MUST keep the original controller, and restore it when we want to play the original animations
	// (no item), otherwise the Unity will crash when we pick another item
	public RuntimeAnimatorController originalRuntimeController;

	//public AnimatorOverrideController overrideController;

	/* ==========================================================================================================
	 * UNITY
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

	void Awake() {

	}


	/// <summary>
	/// Change the animation controller to the new one, which contains the animation to the item 'stItemName'
	/// On the first call, will preserve the original animator for later use. If the animation controller for
	/// the item doesn't exists, it will be created
	/// </summary>
	/// <param name="animator"> The animator object of the player </param>
	/// <param name="stItemName"> The item name (in the hierarchy) </param>
	public void ChangeAnimation(Animator animator, MainGame.eItemTypes itemType) {
		
		// Find the item
		foreach(AnimationOverrideItem itemOverride in itemClipOverrides) {

			if(itemOverride.itemType == itemType) {

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

	/// <summary>
	/// Restore the original animator controller. Usually called when the item is dropped and we must return
	/// to the animations with no item. If we don't restore this controller, the animation controller will not
	/// play the right animations on the next item, and the Unity Editor will crash eventually
	/// </summary>
	/// <param name="animator"> The animator to have it controller restored </param>
	public void RestoreAnimation(Animator animator) {

		animator.runtimeAnimatorController = originalRuntimeController;
	}
}
