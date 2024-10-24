using UnityEngine;
using System.Collections;

namespace CraftingAnims
{
	public class CrafterIKHands:MonoBehaviour
	{
		public Transform leftHandObj;
		public Transform attachLeft;

		[Range(0, 1)] public float leftHandPositionWeight;
		[Range(0, 1)] public float leftHandRotationWeight;

		private Transform blendToTransform;
		private Coroutine co;

		private Animator animator;
		private CrafterController crafterController;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			crafterController = GetComponentInParent<CrafterController>();
		}

		/// <summary>
		/// If there is movement and/or rotation data in the animation for the Left Hand, use IK to
		/// set the position of the Left Hand of the character.
		/// </summary>
		private void OnAnimatorIK(int layerIndex)
		{
			if (!leftHandObj) { return; }
			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftHandPositionWeight);
			animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
			if (!attachLeft) { return; }
			animator.SetIKPosition(AvatarIKGoal.LeftHand, attachLeft.position);
			animator.SetIKRotation(AvatarIKGoal.LeftHand, attachLeft.rotation);
		}

		/// <summary>
		/// Smoothly blend IK on and off so there's no snapping into position.
		/// </summary>
		public void BlendIK(bool blendOn, float delay, float timeToBlend)
		{
			StopAllCoroutines();
			co = StartCoroutine(_BlendIK(blendOn, delay, timeToBlend));
		}

		private IEnumerator _BlendIK(bool blendOn, float delay, float timeToBlend)
		{
			GetCurrentAttachPoint();
			yield return new WaitForSeconds(delay);
			var t = 0f;
			var blendTo = 0;
			var blendFrom = 0;

			if (blendOn) { blendTo = 1; }
			else { blendFrom = 1; }
			while (t < 1) {
				t += Time.deltaTime / timeToBlend;
				attachLeft = blendToTransform;
				leftHandPositionWeight = Mathf.Lerp(blendFrom, blendTo, t);
				leftHandRotationWeight = Mathf.Lerp(blendFrom, blendTo, t);
				yield return null;
			}
		}

		/// <summary>
		/// Pauses IK while character uses Left Hand during an animation.
		/// </summary>
		public void SetIKPause(float pauseTime)
		{
			StopAllCoroutines();
			co = StartCoroutine(_SetIKPause(pauseTime));
		}

		private IEnumerator _SetIKPause(float pauseTime)
		{
			var t = 0f;
			while (t < 1) {
				t += Time.deltaTime / 0.1f;
				leftHandPositionWeight = Mathf.Lerp(1, 0, t);
				leftHandRotationWeight = Mathf.Lerp(1, 0, t);
				yield return null;
			}
			yield return new WaitForSeconds(pauseTime - 0.2f);
			t = 0f;
			while (t < 1) {
				t += Time.deltaTime / 0.1f;
				leftHandPositionWeight = Mathf.Lerp(0, 1, t);
				leftHandRotationWeight = Mathf.Lerp(0, 1, t);
				yield return null;
			}
		}

		private void GetCurrentAttachPoint()
		{
			// Hatchet for Chopping.
			if (crafterController.charState == CrafterState.Chopping) {
				try { blendToTransform = crafterController.hatchet.transform.GetChild(0).transform; }
				catch { Debug.LogWarning("No attachpoint for " + crafterController.hatchet); }
				if (crafterController.hatchet.transform.GetChild(0).transform != null)
				{ blendToTransform = crafterController.hatchet.transform.GetChild(0).transform; }
			}
			// Fishing Pole for Fishing.
			else if (crafterController.charState == CrafterState.Fishing) {
				try { blendToTransform = crafterController.fishingpole.transform.GetChild(0).transform; }
				catch { Debug.LogWarning("No attachpoint for " + crafterController.fishingpole); }
				if (crafterController.fishingpole.transform.GetChild(0).transform != null)
				{ blendToTransform = crafterController.fishingpole.transform.GetChild(0).transform; }
			}
			// PickAxe for PickAxing.
			else if (crafterController.charState == CrafterState.PickAxing) {
				try { blendToTransform = crafterController.pickaxe.transform.GetChild(0).transform; }
				catch { Debug.LogWarning("No attachpoint for " + crafterController.pickaxe); }
				if (crafterController.pickaxe.transform.GetChild(0).transform != null)
				{ blendToTransform = crafterController.pickaxe.transform.GetChild(0).transform; }
			}
			// Rake for Raking.
			else if (crafterController.charState == CrafterState.Raking) {
				try { blendToTransform = crafterController.rake.transform.GetChild(0).transform; }
				catch { Debug.LogWarning("No attachpoint for " + crafterController.rake); }
				if (crafterController.rake.transform.GetChild(0).transform != null)
				{ blendToTransform = crafterController.rake.transform.GetChild(0).transform; }
			}
			// Shovel for Digging.
			else if (crafterController.charState == CrafterState.Digging) {
				try { blendToTransform = crafterController.shovel.transform.GetChild(0).transform; }
				catch { Debug.LogWarning("No attachpoint for " + crafterController.shovel); }
				if (crafterController.shovel.transform.GetChild(0).transform != null)
				{ blendToTransform = crafterController.shovel.transform.GetChild(0).transform; }
			}
			// Spear for Spearfishing.
			else if (crafterController.charState == CrafterState.Spearfishing) {
				try { blendToTransform = crafterController.spear.transform.GetChild(0).transform; }
				catch { Debug.LogWarning("No attachpoint for " + crafterController.spear); }
				if (crafterController.spear.transform.GetChild(0).transform != null)
				{ blendToTransform = crafterController.spear.transform.GetChild(0).transform; }
			}
		}
	}
}