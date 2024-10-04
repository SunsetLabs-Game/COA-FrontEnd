using CraftingAnims;
using UnityEngine;

public class GUIControls:MonoBehaviour
{
	[HideInInspector] public CrafterController crafterController;
	[HideInInspector] public CrafterActions actions;
	private bool useNavigation = false;
	private bool run;
	private bool carryItem;
	private bool carryToggle;

	private void Awake()
	{
		crafterController = GetComponent<CrafterController>();
		actions = GetComponent<CrafterActions>();
	}

	public void ResetCarry()
	{
		carryItem = false;
		carryToggle = false;
	}

	/// <summary>
	/// Exits Navigation mode and returns to Crafter to start position.
	/// </summary>
	private void ResetCrafter()
	{
		gameObject.transform.position = new Vector3(0, 0, 0);
		useNavigation = false;
	}

	private void Update()
	{
		// Reset Crafter.
		if (Input.GetKey(KeyCode.R)) { ResetCrafter(); }
	}

	private void OnGUI()
	{
		if (!crafterController.isMoving && crafterController.isGrounded) {

			if (crafterController.useNavMeshNavigation) {
				useNavigation = GUI.Toggle(new Rect(550, 105, 100, 30), useNavigation, "Navigation");
				crafterController.navMeshNavigation = useNavigation;
				if (useNavigation) {
					crafterController.crafterNavigation.enabled = true;
					run = GUI.Toggle(new Rect(550, 135, 100, 30), run, "Run");
					crafterController.navMeshRun = run;
				}
				else { crafterController.crafterNavigation.enabled = false; }
			}

			if (!crafterController.crafterNavigation.isNavigating) {
				if (crafterController.charState == CrafterState.Idle) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Get Hammer")) { actions.TakeAction("Get Hammer"); }
					if (GUI.Button(new Rect(195, 25, 150, 30), "Get Paintbrush")) { actions.TakeAction("Get Paintbrush"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Get Hatchet")) { actions.TakeAction("Get Hatchet"); }
					if (GUI.Button(new Rect(195, 65, 150, 30), "Get Spear")) { actions.TakeAction("Get Spear"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Get PickAxe")) { actions.TakeAction("Get PickAxe"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Pickup Shovel")) { actions.TakeAction("Pickup Shovel"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "PullUp Fishing Pole")) { actions.TakeAction("PullUp Fishing Pole"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Take Food")) { actions.TakeAction("Take Food"); }
					if (GUI.Button(new Rect(25, 265, 150, 30), "Recieve Drink")) { actions.TakeAction("Recieve Drink"); }
					if (GUI.Button(new Rect(25, 305, 150, 30), "Pickup Box")) { actions.TakeAction("Pickup Box"); }
					if (GUI.Button(new Rect(195, 305, 150, 30), "Pickup Lumber")) { actions.TakeAction("Pickup Lumber"); }
					if (GUI.Button(new Rect(370, 305, 150, 30), "Pickup Overhead")) { actions.TakeAction("Pickup Overhead"); }
					if (GUI.Button(new Rect(25, 345, 150, 30), "Recieve Box")) { actions.TakeAction("Recieve Box"); }
					if (GUI.Button(new Rect(25, 385, 150, 30), "Get Saw")) { actions.TakeAction("Get Saw"); }
					if (GUI.Button(new Rect(25, 425, 150, 30), "Get Sickle")) { actions.TakeAction("Get Sickle"); }
					if (GUI.Button(new Rect(25, 465, 150, 30), "Get Rake")) { actions.TakeAction("Get Rake"); }
					if (GUI.Button(new Rect(200, 465, 150, 30), "Use")) { actions.TakeAction("Use"); }
					if (GUI.Button(new Rect(375, 465, 150, 30), "Crawl")) { actions.TakeAction("Crawl"); }
					if (GUI.Button(new Rect(25, 505, 150, 30), "Sit")) { actions.TakeAction("Sit"); }
					if (GUI.Button(new Rect(200, 505, 150, 30), "Push Cart")) { actions.TakeAction("Push Cart"); }
					if (GUI.Button(new Rect(375, 505, 150, 30), "Laydown")) { actions.TakeAction("Laydown"); }
					if (GUI.Button(new Rect(25, 545, 150, 30), "Gather")) { actions.TakeAction("Gather"); }
					if (GUI.Button(new Rect(200, 545, 150, 30), "Gather Kneeling")) { actions.TakeAction("Gather Kneeling"); }
					if (GUI.Button(new Rect(200, 585, 150, 30), "Wave1")) { actions.TakeAction("Wave1"); }
					if (GUI.Button(new Rect(375, 545, 150, 30), "Cheer1")) { actions.TakeAction("Cheer1"); }
					if (GUI.Button(new Rect(25, 585, 150, 30), "Scratch Head")) { actions.TakeAction("Scratch Head"); }
					if (GUI.Button(new Rect(375, 585, 150, 30), "Cheer2")) { actions.TakeAction("Cheer2"); }
					if (GUI.Button(new Rect(375, 630, 150, 30), "Cheer3")) { actions.TakeAction("Cheer3"); }
					if (GUI.Button(new Rect(375, 670, 150, 30), "Fear")) { actions.TakeAction("Fear"); }
					if (GUI.Button(new Rect(25, 625, 150, 30), "Climb")) { actions.TakeAction("Climb"); }
					if (GUI.Button(new Rect(200, 625, 150, 30), "Climb Top")) { actions.TakeAction("Climb Top"); }
					if (GUI.Button(new Rect(200, 665, 150, 30), "Pray")) { actions.TakeAction("Pray"); }
					if (GUI.Button(new Rect(25, 665, 150, 30), "Push Pull")) { actions.TakeAction("Push Pull"); }
				}
				if (crafterController.charState == CrafterState.Cart) {
					if (GUI.Button(new Rect(200, 505, 150, 30), "Release Cart")) { actions.TakeAction("Release Cart"); }
				}
				if (crafterController.charState == CrafterState.Pray) {
					if (GUI.Button(new Rect(200, 665, 150, 30), "Stand")) { actions.TakeAction("Stand"); }
				}
				if (crafterController.charState == CrafterState.Hammer) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Hammer Wall")) { actions.TakeAction("Hammer Wall"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Hammer Table")) { actions.TakeAction("Hammer Table"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Hammer")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Away Hammer")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Put Down Hammer")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Drop Hammer")) { actions.TakeAction("Drop Item"); }
					if (GUI.Button(new Rect(25, 265, 150, 30), "Kneel")) { actions.TakeAction("Kneel"); }
					if (GUI.Button(new Rect(25, 305, 150, 30), "Chisel")) { actions.TakeAction("Chisel"); }
					if (GUI.Button(new Rect(25, 345, 150, 30), "Swing Horizontal")) { actions.TakeAction("Swing Horizontal"); }
					if (GUI.Button(new Rect(25, 385, 150, 30), "Swing Vertical")) { actions.TakeAction("Swing Vertical"); }
				}
				if (crafterController.charState == CrafterState.Painting) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Paint Wall")) { actions.TakeAction("Paint Wall"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Fill Brush")) { actions.TakeAction("Fill Brush"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Paintbrush")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Away Paintbrush")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Put Down Paintbrush")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Drop Paintbrush")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Kneel) {
					if (GUI.Button(new Rect(25, 30, 150, 30), "Hammer")) { actions.TakeAction("Hammer"); }
					if (GUI.Button(new Rect(25, 265, 150, 30), "Stand")) { actions.TakeAction("Stand"); }
				}
				if (crafterController.charState == CrafterState.Drink) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Drink")) { actions.TakeAction("Drink"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Water")) { actions.TakeAction("Water"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Drink")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Drink Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Put Drink Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Drop Drink")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Food) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Eat Food")) { actions.TakeAction("Eat Food"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Give Food")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Put Food Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Food Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop Food")) { actions.TakeAction("Drop Item"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Plant Food")) { actions.TakeAction("Plant Item"); }
				}
				if (crafterController.charState == CrafterState.Sickle) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Use Sickle")) { actions.TakeAction("Use Sickle"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Give Sickle")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Put Sickle Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Sickle Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop Sickle")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Hatchet) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Start Chopping")) { actions.TakeAction("Start Chopping"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Put Hatchet Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Hatchet")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Hatchet Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop Hatchet")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.PickAxe) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Start PickAxing")) { actions.TakeAction("Start PickAxing"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Put PickAxe Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give PickAxe")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put PickAxe Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop PickAxe")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Saw) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Start Sawing")) { actions.TakeAction("Start Sawing"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Put Saw Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Saw")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Drop Saw")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Sawing) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Finish Sawing")) { actions.TakeAction("Finish Sawing"); }
				}
				if (crafterController.charState == CrafterState.Chopping) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Chop Vertical")) { actions.TakeAction("Chop Vertical"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Chop Horizontal")) { actions.TakeAction("Chop Horizontal"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Chop Diagonal")) { actions.TakeAction("Chop Diagonal"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Chop Ground")) { actions.TakeAction("Chop Ground"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Finish Chopping")) { actions.TakeAction("Finish Chopping"); }
				}
				if (crafterController.charState == CrafterState.PickAxing) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Swing Vertical")) { actions.TakeAction("Swing Vertical"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Swing Horizontal")) { actions.TakeAction("Swing Horizontal"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Swing Ground")) { actions.TakeAction("Swing Ground"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Swing Ceiling")) { actions.TakeAction("Swing Ceiling"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Swing Diagonal")) { actions.TakeAction("Swing Diagonal"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Finish PickAxing")) { actions.TakeAction("Finish PickAxing"); }
				}
				if (crafterController.charState == CrafterState.Shovel) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Start Digging")) { actions.TakeAction("Start Digging"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Put Shovel Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Shovel")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Shovel Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop Shovel")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Rake) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Start Raking")) { actions.TakeAction("Start Raking"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Put Rake Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Rake")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Rake Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop Rake")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Raking) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Rake")) { actions.TakeAction("Rake"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Finish Raking")) { actions.TakeAction("Finish Raking"); }
				}
				if (crafterController.charState == CrafterState.Digging) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Dig")) { actions.TakeAction("Dig"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Finish Digging")) { actions.TakeAction("Finish Digging"); }
				}
				if (crafterController.charState == CrafterState.FishingPole) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Cast Reel")) { actions.TakeAction("Cast Reel"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Put Fishing Pole Away")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Fishing Pole")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Fishing Pole Down")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Drop FishingPole")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Sit) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Talk1")) { actions.TakeAction("Talk1"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Eat")) { actions.TakeAction("Eat"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Drink")) { actions.TakeAction("Drink"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Stand")) { actions.TakeAction("Stand"); }
				}
				if (crafterController.charState == CrafterState.Fishing) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Reel In")) { actions.TakeAction("Reel In"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Finish Fishing")) { actions.TakeAction("Finish Fishing"); }
				}
				if (crafterController.charState == CrafterState.Box) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Put Down Box")) { actions.TakeAction("Put Down Box"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Throw Box")) { actions.TakeAction("Throw Box"); }
					if (GUI.Button(new Rect(25, 104, 150, 30), "Give Box")) { actions.TakeAction("Give Box"); }
				}
				if (crafterController.charState == CrafterState.Lumber) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Put Down Lumber")) { actions.TakeAction("Put Down Lumber"); }
				}
				if (crafterController.charState == CrafterState.Overhead) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Throw Sphere")) { actions.TakeAction("Throw Sphere"); }
				}
				if (crafterController.charState == CrafterState.Climb) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Climb Off Top")) { actions.TakeAction("Climb Off Top"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Climb Up")) { actions.TakeAction("Climb Up"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Climb Down")) { actions.TakeAction("Climb Down"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Climb Off Bottom")) { actions.TakeAction("Climb Off Bottom"); }
				}
				if (crafterController.charState == CrafterState.PushPull) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Release")) { actions.TakeAction("Release"); }
				}
				if (crafterController.charState == CrafterState.Laydown) {
					if (GUI.Button(new Rect(375, 505, 150, 30), "Getup")) { actions.TakeAction("Getup"); }
				}
				if (crafterController.charState == CrafterState.Use) {
					if (GUI.Button(new Rect(200, 465, 150, 30), "Stop Use")) { actions.TakeAction("Stop Use"); }
				}
				if (crafterController.charState == CrafterState.Crawl) {
					if (GUI.Button(new Rect(375, 465, 150, 30), "Getup")) { actions.TakeAction("Getup"); }
				}
				if (crafterController.charState == CrafterState.Spear) {
					if (GUI.Button(new Rect(25, 65, 150, 30), "Start Spearfishing")) { actions.TakeAction("Start Spearfishing"); }
					if (GUI.Button(new Rect(25, 105, 150, 30), "Give Spear")) { actions.TakeAction("Give Item"); }
					if (GUI.Button(new Rect(25, 145, 150, 30), "Put Away Spear")) { actions.TakeAction("Put Away Item"); }
					if (GUI.Button(new Rect(25, 185, 150, 30), "Put Down Spear")) { actions.TakeAction("Put Down Item"); }
					if (GUI.Button(new Rect(25, 225, 150, 30), "Drop Spear")) { actions.TakeAction("Drop Item"); }
				}
				if (crafterController.charState == CrafterState.Spearfishing) {
					if (GUI.Button(new Rect(25, 25, 150, 30), "Spear")) { actions.TakeAction("Spear"); }
					if (GUI.Button(new Rect(25, 65, 150, 30), "Finish Spearfishing")) { actions.TakeAction("Finish Spearfishing"); }
				}

				//Carry Item animation override.
				if (crafterController.charState == CrafterState.Hammer
					|| crafterController.charState == CrafterState.Painting
					|| crafterController.charState == CrafterState.Drink
					|| crafterController.charState == CrafterState.Food
					|| crafterController.charState == CrafterState.Sickle
					|| crafterController.charState == CrafterState.Hatchet
					|| crafterController.charState == CrafterState.PickAxe
					|| crafterController.charState == CrafterState.Shovel
					|| crafterController.charState == CrafterState.Rake) {
					carryItem = GUI.Toggle(new Rect(500, 15, 100, 30), carryItem, "Carry Item");
					if (carryItem) {
						if (!carryToggle) {
							carryToggle = true;
							crafterController.CarryItem(true);
						}
					}
					else if (!carryItem) {
						if (carryToggle) {
							carryToggle = false;
							crafterController.CarryItem(false);
						}
					}
				}
			}
		}
	}
}