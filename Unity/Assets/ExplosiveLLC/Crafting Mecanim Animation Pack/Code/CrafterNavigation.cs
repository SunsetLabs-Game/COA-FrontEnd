using UnityEngine;
using UnityEngine.AI;

namespace CraftingAnims
{
	public class CrafterNavigation:MonoBehaviour
	{
		// Components.
		[HideInInspector] public CrafterController crafterController;
		[HideInInspector] public NavMeshAgent navMeshAgent;
		[HideInInspector] public GameObject nav;

		// Variables.
		public bool isNavigating;

		private void Awake()
		{
			crafterController = GetComponent<CrafterController>();

			// Setup NavMeshAgent
			navMeshAgent = GetComponent<NavMeshAgent>();
			navMeshAgent.speed = crafterController.walkSpeed;
			navMeshAgent.enabled = false;

			// Find Nav object for visualizing navpoint.
			nav = GameObject.Find("Nav");
			HideNavPointer();
		}

		private void OnDisable()
		{
			StopNavigating();
			HideNavPointer();
		}

		private void Update()
		{
			if (crafterController.navMeshNavigation) {
				ShowNavPointer();
				RaycastHit hit;
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
					nav.transform.position = hit.point;
					if (Input.GetMouseButtonDown(0)) { MeshNavToPoint(hit.point); }
				}
				// If in active NavMeshNavigation.
				if (isNavigating) { Navigating(); }
			}
			else {
				HideNavPointer();
				StopNavigating();
			}
		}

		/// <summary>
		/// Shows the NavPointer object meshes.
		/// </summary>
		private void ShowNavPointer()
		{
			if (nav != null) {
				nav.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
				nav.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = true;
			}
		}

		/// <summary>
		/// Hides the NavPointer object meshes.
		/// </summary>
		private void HideNavPointer()
		{
			if (nav != null) {
				nav.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
				nav.transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().enabled = false;
			}
		}

		/// <summary>
		/// Navigate towards the destiniation using NavMeshAgent.
		/// </summary>
		private void Navigating()
		{
			RotateTowardsMovementDir();

			// Animator settings.
			if (navMeshAgent.velocity.sqrMagnitude > 0) {
				crafterController.animator.SetBool("Moving", true);
				if (crafterController.navMeshRun) {
					navMeshAgent.speed = crafterController.runSpeed;
					crafterController.animator.SetFloat("Velocity Z", 1f);
				}
				else {
					navMeshAgent.speed = crafterController.walkSpeed;
					crafterController.animator.SetFloat("Velocity Z", 0.5f);
				}
			}
			else { crafterController.animator.SetFloat("Velocity Z", 0); }

			// Check if we've reached the destination
			if (!navMeshAgent.pathPending) {
				if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) { StopNavigating(); }
			}
		}

		/// <summary>
		/// Navigate to the destination using Unity's NavMeshAgent.
		/// </summary>
		/// <param name="destination">Point in world space to navigate to.</param>
		public void MeshNavToPoint(Vector3 destination)
		{
			Debug.Log("MeshNavToPoint: " + destination);
			navMeshAgent.enabled = true;
			isNavigating = true;
			navMeshAgent.SetDestination(destination);
		}

		/// <summary>
		/// Stop navigating to the current destination.
		/// </summary>
		public void StopNavigating()
		{
			Debug.Log("StopNavigating");
			isNavigating = false;
			navMeshAgent.enabled = false;
			crafterController.animator.SetFloat("Velocity Z", 0);
		}

		/// <summary>
		/// Face the character towards the movement direction.
		/// </summary>
		private void RotateTowardsMovementDir()
		{
			if (navMeshAgent.velocity.magnitude > 0.01f) {
				transform.rotation = Quaternion.Slerp(transform.rotation,
					Quaternion.LookRotation(navMeshAgent.velocity),
					Time.deltaTime * navMeshAgent.angularSpeed);
			}
		}
	}
}