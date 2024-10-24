using UnityEngine;

namespace CraftingAnims
{
	public class CrafterActions:MonoBehaviour
	{
		// Components.
		[HideInInspector] public CrafterController crafterController;

		private void Awake()
		{ crafterController = GetComponent<CrafterController>(); }

		public void TakeAction(string action)
		{
			#region LoseItems

			if (action == "Give Item") {
				crafterController.TriggerAnimation("ItemHandoffTrigger");
				crafterController.showItem.ItemShow("none", 0.4f);
				crafterController.ChangeCharacterState(0.4f, CrafterState.Idle);
				crafterController.LockMovement(1f);
				crafterController.BlendOff(0f);
			}
			else if (action == "Put Away Item") {
				crafterController.TriggerAnimation("ItemBeltAwayTrigger");
				crafterController.showItem.ItemShow("none", 0.5f);
				crafterController.ChangeCharacterState(0.4f, CrafterState.Idle);
				crafterController.LockMovement(1f);
				crafterController.BlendOff(0f);
			}
			else if (action == "Put Down Item") {
				crafterController.TriggerAnimation("ItemPutdownTrigger");
				crafterController.showItem.ItemShow("none", 0.7f);
				crafterController.charState = CrafterState.Idle;
				crafterController.LockMovement(1.2f);
				crafterController.BlendOff(0f);
			}
			else if (action == "Drop Item") {
				crafterController.TriggerAnimation("ItemDropTrigger");
				crafterController.showItem.ItemShow("none", 0.4f);
				crafterController.charState = CrafterState.Idle;
				crafterController.LockMovement(1.2f);
				crafterController.BlendOff(0f);
			}
			else if (action == "Plant Item") {
				crafterController.TriggerAnimation("ItemPlantTrigger");
				crafterController.showItem.ItemShow("none", 0.4f);
				crafterController.charState = CrafterState.Idle;
				crafterController.LockMovement(1.8f);
				crafterController.BlendOff(0f);
			}

			#endregion

			if (crafterController.charState == CrafterState.Idle) {

				#region GainItems

				if (action == "Get Hammer") {
					crafterController.TriggerAnimation("ItemBeltTrigger");
					crafterController.showItem.ItemShow("hammer", 0.5f);
					crafterController.charState = CrafterState.Hammer;
					crafterController.LockMovement(1f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Get Paintbrush") {
					crafterController.TriggerAnimation("ItemBeltTrigger");
					crafterController.showItem.ItemShow("paintbrush", 0.5f);
					crafterController.charState = CrafterState.Painting;
					crafterController.LockMovement(1f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Get Hatchet") {
					crafterController.TriggerAnimation("ItemBackTrigger");
					crafterController.showItem.ItemShow("axe", 0.5f);
					crafterController.charState = CrafterState.Hatchet;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Get Spear") {
					crafterController.TriggerAnimation("ItemBackTrigger");
					crafterController.showItem.ItemShow("spear", 0.5f);
					crafterController.charState = CrafterState.Spear;
					crafterController.isSpearfishing = true;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Get PickAxe") {
					crafterController.TriggerAnimation("ItemBackTrigger");
					crafterController.showItem.ItemShow("pickaxe", 0.5f);
					crafterController.charState = CrafterState.PickAxe;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Pickup Shovel") {
					crafterController.TriggerAnimation("ItemPickupTrigger");
					crafterController.showItem.ItemShow("shovel", 0.3f);
					crafterController.charState = CrafterState.Shovel;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "PullUp Fishing Pole") {
					crafterController.TriggerAnimation("ItemPullUpTrigger");
					crafterController.showItem.ItemShow("fishingpole", 0.5f);
					crafterController.charState = CrafterState.FishingPole;
					crafterController.LockMovement(1.7f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Take Food") {
					crafterController.TriggerAnimation("ItemTakeTrigger");
					crafterController.showItem.ItemShow("food", 0.3f);
					crafterController.charState = CrafterState.Food;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Recieve Drink") {
					crafterController.TriggerAnimation("ItemRecieveTrigger");
					crafterController.showItem.ItemShow("drink", 0.5f);
					crafterController.charState = CrafterState.Drink;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Pickup Box") {
					crafterController.TriggerAnimation("CarryPickupTrigger");
					crafterController.showItem.ItemShow("box", 0.1f);
					crafterController.charState = CrafterState.Box;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Pickup Lumber") {
					crafterController.TriggerAnimation("LumberPickupTrigger");
					crafterController.showItem.ItemShow("lumber", 0.5f);
					crafterController.charState = CrafterState.Lumber;
					crafterController.LockMovement(1.6f);
				}
				else if (action == "Pickup Overhead") {
					crafterController.TriggerAnimation("CarryOverheadPickupTrigger");
					crafterController.showItem.ItemShow("sphere", 0.5f);
					crafterController.charState = CrafterState.Overhead;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Recieve Box") {
					crafterController.TriggerAnimation("CarryRecieveTrigger");
					crafterController.showItem.ItemShow("box", 0.5f);
					crafterController.charState = CrafterState.Box;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Get Saw") {
					crafterController.TriggerAnimation("ItemBeltTrigger");
					crafterController.showItem.ItemShow("saw", 0.5f);
					crafterController.charState = CrafterState.Saw;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Get Sickle") {
					crafterController.TriggerAnimation("ItemBeltTrigger");
					crafterController.showItem.ItemShow("sickle", 0.5f);
					crafterController.charState = CrafterState.Sickle;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}
				else if (action == "Get Rake") {
					crafterController.TriggerAnimation("ItemBackTrigger");
					crafterController.showItem.ItemShow("rake", 0.5f);
					crafterController.charState = CrafterState.Rake;
					crafterController.LockMovement(1.2f);
					crafterController.RightHandBlend(true);
				}

				#endregion

				#region Actions

				else if (action == "Use") {
					crafterController.animator.SetBool("Use", true);
					crafterController.charState = CrafterState.Use;
					crafterController.LockMovement(-1);
				}
				else if (action == "Crawl") {
					crafterController.TriggerAnimation("CrawlStartTrigger");
					crafterController.charState = CrafterState.Crawl;
					crafterController.LockMovement(1f);
				}
				else if (action == "Sit") {
					crafterController.TriggerAnimation("ChairSitTrigger");
					crafterController.showItem.ItemShow("chair", 0.3f);
					crafterController.charState = CrafterState.Sit;
					crafterController.LockMovement(-1f);
				}
				else if (action == "Push Cart") {
					crafterController.TriggerAnimation("CartPullGrabTrigger");
					crafterController.showItem.ItemShow("cart", 0.25f);
					crafterController.charState = CrafterState.Cart;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Laydown") {
					crafterController.TriggerAnimation("LaydownLaydownTrigger");
					crafterController.charState = CrafterState.Laydown;
					crafterController.LockMovement(-1f);
				}
				else if (action == "Gather") {
					crafterController.TriggerAnimation("GatherTrigger");
					crafterController.LockMovement(2.2f);
				}
				else if (action == "Gather Kneeling") {
					crafterController.TriggerAnimation("GatherKneelingTrigger");
					crafterController.LockMovement(2.2f);
				}
				else if (action == "Wave1") {
					crafterController.TriggerAnimation("Wave1Trigger");
					crafterController.LockMovement(2.2f);
				}
				else if (action == "Scratch Head") {
					crafterController.TriggerAnimation("Bored1Trigger");
					crafterController.LockMovement(2.5f);
				}
				else if (action == "Cheer1") {
					crafterController.TriggerAnimation("Cheer1Trigger");
					crafterController.LockMovement(2.7f);
				}
				else if (action == "Cheer2") {
					crafterController.TriggerAnimation("Cheer2Trigger");
					crafterController.LockMovement(3f);
				}
				else if (action == "Cheer3") {
					crafterController.TriggerAnimation("Cheer3Trigger");
					crafterController.LockMovement(2.4f);
				}
				else if (action == "Fear") {
					crafterController.TriggerAnimation("FearTrigger");
					crafterController.LockMovement(4f);
				}
				else if (action == "Climb") {
					crafterController.TriggerAnimation("ClimbStartTrigger");
					crafterController.showItem.ItemShow("ladder", 0.3f);
					crafterController.charState = CrafterState.Climb;
					crafterController.LockMovement(-1f);
				}
				else if (action == "Climb Top") {
					crafterController.gameObject.transform.position += new Vector3(0, 3, 0);
					crafterController.TriggerAnimation("ClimbOnTopTrigger");
					crafterController.showItem.ItemShow("ladder", 0.3f);
					crafterController.charState = CrafterState.Climb;
					crafterController.LockMovement(-1f);
				}
				else if (action == "Pray") {
					crafterController.TriggerAnimation("PrayDownTrigger");
					crafterController.charState = CrafterState.Pray;
					crafterController.LockMovement(-1f);
				}
				else if (action == "Push Pull") {
					crafterController.TriggerAnimation("PushPullStartTrigger");
					crafterController.showItem.ItemShow("pushpull", 0.3f);
					crafterController.charState = CrafterState.PushPull;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Push Pull") {
					crafterController.TriggerAnimation("PushPullStartTrigger");
					crafterController.showItem.ItemShow("pushpull", 0.3f);
					crafterController.charState = CrafterState.PushPull;
					crafterController.LockMovement(1.2f);
				}
			}

			#endregion

			#region EnterStates

			// These states all trigger IKBlendOn to position the left hand.

			else if (crafterController.charState == CrafterState.FishingPole) {
				if (action == "Cast Reel") {
					crafterController.TriggerAnimation("FishingCastTrigger");
					crafterController.charState = CrafterState.Fishing;
					crafterController.LockMovement(-1f);
					crafterController.IKBlendOn();
				}
			}
			else if (crafterController.charState == CrafterState.Hatchet) {
				if (action == "Start Chopping") {
					crafterController.TriggerAnimation("ChoppingStartTrigger");
					crafterController.charState = CrafterState.Chopping;
					crafterController.LockMovement(-1f);
					crafterController.IKBlendOn();
				}
			}
			else if (crafterController.charState == CrafterState.PickAxe) {
				if (action == "Start PickAxing") {
					crafterController.TriggerAnimation("ChoppingStartTrigger");
					crafterController.charState = CrafterState.PickAxing;
					crafterController.LockMovement(-1f);
					crafterController.IKBlendOn();
				}
			}
			else if (crafterController.charState == CrafterState.Rake) {
				if (action == "Start Raking") {
					crafterController.TriggerAnimation("DiggingStartTrigger");
					crafterController.charState = CrafterState.Raking;
					crafterController.LockMovement(-1f);
					crafterController.IKBlendOn();
				}
			}
			else if (crafterController.charState == CrafterState.Shovel) {
				if (action == "Start Digging") {
					crafterController.TriggerAnimation("DiggingStartTrigger");
					crafterController.charState = CrafterState.Digging;
					crafterController.LockMovement(-1f);
					crafterController.IKBlendOn();
				}
			}
			else if (crafterController.charState == CrafterState.Spear) {
				if (action == "Start Spearfishing") {
					crafterController.TriggerAnimation("SpearfishStartTrigger");
					crafterController.charState = CrafterState.Spearfishing;
					crafterController.LockMovement(1.2f);
					crafterController.IKBlendOn();
				}
			}

			#endregion

			#region States

			else if (crafterController.charState == CrafterState.Cart) {
				if (action == "Release Cart") {
					crafterController.TriggerAnimation("CartPullReleaseTrigger");
					crafterController.showItem.ItemShow("none", 0.75f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
			}
			else if (crafterController.charState == CrafterState.Hammer) {
				if (action == "Hammer Wall") {
					crafterController.TriggerAnimation("HammerWallTrigger");
					crafterController.LockMovement(1.9f);
				}
				else if (action == "Hammer Table") {
					crafterController.TriggerAnimation("HammerTableTrigger");
					crafterController.LockMovement(1.9f);
				}
				else if (action == "Kneel") {
					crafterController.TriggerAnimation("ItemKneelDownTrigger");
					crafterController.charState = CrafterState.Kneel;
					crafterController.LockMovement(-1f);
				}
				else if (action == "Chisel") {
					crafterController.TriggerAnimation("ItemChiselTrigger");
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Swing Horizontal") {
					print("Here");
					crafterController.TriggerAnimation("ItemSwingHorizontal");
					crafterController.LockMovement(1.0f);
				}
				else if (action == "Swing Vertical") {
					crafterController.TriggerAnimation("ItemSwingVertical");
					crafterController.LockMovement(1.0f);
				}
			}
			else if (crafterController.charState == CrafterState.Painting) {
				if (action == "Paint Wall") {
					crafterController.TriggerAnimation("ItemPaintTrigger");
					crafterController.LockMovement(1.9f);
				}
				else if (action == "Fill Brush") {
					crafterController.TriggerAnimation("ItemPaintRefillTrigger");
					crafterController.LockMovement(1.9f);
				}
			}
			else if (crafterController.charState == CrafterState.Saw) {
				if (action == "Start Sawing") {
					crafterController.TriggerAnimation("SawStartTrigger");
					crafterController.charState = CrafterState.Sawing;
					crafterController.LockMovement(-1f);
				}
			}
			else if (crafterController.charState == CrafterState.Drink) {
				if (action == "Drink") {
					crafterController.TriggerAnimation("ItemDrinkTrigger");
					crafterController.LockMovement(1.4f);
				}
				else if (action == "Water") {
					crafterController.TriggerAnimation("ItemWaterTrigger");
					crafterController.LockMovement(2f);
				}
			}
			else if (crafterController.charState == CrafterState.Food) {
				if (action == "Eat Food") {
					crafterController.TriggerAnimation("ItemEatTrigger");
					crafterController.LockMovement(1.4f);
				}
				else if (action == "Plant Food") {
					crafterController.TriggerAnimation("ItemPlantTrigger");
					crafterController.showItem.ItemShow("none", 0.6f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
			}
			else if (crafterController.charState == CrafterState.Sickle) {
				if (action == "Use Sickle") {
					crafterController.TriggerAnimation("ItemSickleUseTrigger");
					crafterController.LockMovement(1.6f);
				}
			}
			else if (crafterController.charState == CrafterState.Box) {
				if (action == "Put Down Box") {
					crafterController.TriggerAnimation("CarryPutdownTrigger");
					crafterController.showItem.ItemShow("none", 0.9f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Throw Box") {
					crafterController.TriggerAnimation("CarryThrowTrigger");
					crafterController.showItem.ItemShow("none", 0.5f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
				else if (action == "Give Box") {
					crafterController.TriggerAnimation("CarryHandoffTrigger");
					crafterController.showItem.ItemShow("none", 0.6f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
			}
			else if (crafterController.charState == CrafterState.Lumber) {
				if (action == "Put Down Lumber") {
					crafterController.TriggerAnimation("CarryPutdownTrigger");
					crafterController.showItem.ItemShow("none", 1f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
			}
			else if (crafterController.charState == CrafterState.Overhead) {
				if (action == "Throw Sphere") {
					crafterController.TriggerAnimation("CarryOverheadThrowTrigger");
					crafterController.showItem.ItemShow("none", 0.5f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.2f);
				}
			}
			else if (crafterController.charState == CrafterState.PushPull) {
				if (action == "Release") {
					crafterController.TriggerAnimation("PushPullReleaseTrigger");
					crafterController.showItem.ItemShow("none", 0.5f);
					crafterController.ChangeCharacterState(0.5f, CrafterState.Idle);
					crafterController.LockMovement(1f);
				}
			}
			else if (crafterController.charState == CrafterState.Crawl) {
				if (action == "Getup") {
					crafterController.TriggerAnimation("CrawlGetupTrigger");
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1f);
				}
			}
			else if (crafterController.charState == CrafterState.Spear) {
				if (action == "Start Spearfishing") {
					crafterController.TriggerAnimation("SpearfishAttackTrigger");
					crafterController.LockMovement(0.6f);
				}
			}
			else if (crafterController.charState == CrafterState.Spearfishing) {
				if (action == "Spear") {
					crafterController.TriggerAnimation("SpearfishAttackTrigger");
					crafterController.LockMovement(1.6f);
					crafterController.crafterIKhands.SetIKPause(1.25f);
				}
				// Trigger IKBlendOff to release the left hand.
				else if (action == "Finish Spearfishing") {
					crafterController.TriggerAnimation("SpearfishEndTrigger");
					crafterController.charState = CrafterState.Spear;
					crafterController.LockMovement(0.6f);
					crafterController.IKBlendOff();
				}
			}

			#endregion

			#region LockedStates

			// The finish state actions all trigger IKBlendOff to release the left hand.

			else if (crafterController.charState == CrafterState.Pray) {
				if (action == "Stand") {
					crafterController.TriggerAnimation("PrayStandTrigger");
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.4f);
				}
			}
			else if (crafterController.charState == CrafterState.Kneel) {
				if (action == "Hammer") { crafterController.TriggerAnimation("ItemKneelHammerTrigger"); }
				else if (action == "Stand") {
					crafterController.TriggerAnimation("ItemKneelStandTrigger");
					crafterController.charState = CrafterState.Hammer;
					crafterController.LockMovement(1f);
				}
			}
			else if (crafterController.charState == CrafterState.Chopping) {
				if (action == "Chop Vertical") { crafterController.TriggerAnimation("ChopVerticalTrigger"); }
				else if (action == "Chop Horizontal") { crafterController.TriggerAnimation("ChopHorizontalTrigger"); }
				else if (action == "Chop Diagonal") { crafterController.TriggerAnimation("ChopDiagonalTrigger"); }
				else if (action == "Chop Ground") { crafterController.TriggerAnimation("ChopGroundTrigger"); }
				else if (action == "Finish Chopping") {
					crafterController.TriggerAnimation("ChopFinishTrigger");
					crafterController.charState = CrafterState.Hatchet;
					crafterController.LockMovement(1.4f);
					crafterController.IKBlendOff();
				}
			}
			else if (crafterController.charState == CrafterState.PickAxing) {
				if (action == "Swing Vertical") { crafterController.TriggerAnimation("ChopVerticalTrigger"); }
				else if (action == "Swing Horizontal") { crafterController.TriggerAnimation("ChopHorizontalTrigger"); }
				else if (action == "Swing Ground") { crafterController.TriggerAnimation("ChopGroundTrigger"); }
				else if (action == "Swing Ceiling") {
					crafterController.TriggerAnimation("ChopCeilingTrigger");
					crafterController.crafterIKhands.SetIKPause(1f);
				}
				else if (action == "Swing Diagonal") { crafterController.TriggerAnimation("ChopDiagonalTrigger"); }
				else if (action == "Finish PickAxing") {
					crafterController.TriggerAnimation("ChopFinishTrigger");
					crafterController.charState = CrafterState.PickAxe;
					crafterController.LockMovement(1.4f);
					crafterController.IKBlendOff();
				}
			}
			else if (crafterController.charState == CrafterState.Raking) {
				if (action == "Rake") { crafterController.TriggerAnimation("ItemRakeUseTrigger"); }
				else if (action == "Finish Raking") {
					crafterController.TriggerAnimation("DiggingFinishTrigger");
					crafterController.charState = CrafterState.Rake;
					crafterController.LockMovement(1f);
					crafterController.IKBlendOff();
				}
			}
			else if (crafterController.charState == CrafterState.Digging) {
				if (action == "Dig") { crafterController.TriggerAnimation("DiggingScoopTrigger"); }
				else if (action == "Finish Digging") {
					crafterController.TriggerAnimation("DiggingFinishTrigger");
					crafterController.charState = CrafterState.Shovel;
					crafterController.LockMovement(1f);
					crafterController.IKBlendOff();
				}
			}
			else if (crafterController.charState == CrafterState.Sawing) {
				if (action == "Finish Sawing") {
					crafterController.TriggerAnimation("SawFinishTrigger");
					crafterController.charState = CrafterState.Saw;
					crafterController.LockMovement(1f);
				}
			}
			else if (crafterController.charState == CrafterState.Sit) {
				if (action == "Talk1") { crafterController.TriggerAnimation("ChairTalk1Trigger"); }
				else if (action == "Eat") {
					crafterController.TriggerAnimation("ChairEatTrigger");
					crafterController.showItem.ItemShow("chaireat", 0.2f);
					crafterController.showItem.ItemShow("chair", 1.1f);
				}
				else if (action == "Drink") {
					crafterController.TriggerAnimation("ChairDrinkTrigger");
					crafterController.showItem.ItemShow("chairdrink", 0.2f);
					crafterController.showItem.ItemShow("chair", 1.1f);
				}
				else if (action == "Stand") {
					crafterController.TriggerAnimation("ChairStandTrigger");
					crafterController.showItem.ItemShow("none", 0.5f);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(1.3f);
				}
			}
			else if (crafterController.charState == CrafterState.Fishing) {
				if (action == "Reel In") {
					crafterController.TriggerAnimation("FishingReelTrigger");
					crafterController.crafterIKhands.SetIKPause(2f);
				}
				else if (action == "Finish Fishing") {
					crafterController.TriggerAnimation("FishingFinishTrigger");
					crafterController.charState = CrafterState.FishingPole;
					crafterController.LockMovement(0.7f);
					crafterController.IKBlendOff();
				}
			}
			else if (crafterController.charState == CrafterState.Climb) {
				if (action == "Climb Off Bottom") {
					crafterController.TriggerAnimation("ClimbOffBottomTrigger");
					crafterController.showItem.ItemShow("none", 0.9f);
					crafterController.LockMovement(0.9f);
					crafterController.ChangeCharacterState(0.9f, CrafterState.Idle);
				}
				else if (action == "Climb Up") { crafterController.TriggerAnimation("ClimbUpTrigger"); }
				else if (action == "Climb Down") { crafterController.TriggerAnimation("ClimbDownTrigger"); }
				else if (action == "Climb Off Top") {
					Vector3 posPivot = crafterController.animator.pivotPosition;
					crafterController.TriggerAnimation("ClimbOffTopTrigger");
					crafterController.showItem.ItemShow("none", 2f);
					crafterController.LockMovement(2.0f);
					crafterController.ChangeCharacterState(2f, CrafterState.Idle);
				}
			}
			else if (crafterController.charState == CrafterState.Laydown) {
				if (action == "Getup") {
					crafterController.TriggerAnimation("LaydownGetupTrigger");
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(2f);
				}
			}
			else if (crafterController.charState == CrafterState.Use) {
				if (action == "Stop Use") {
					crafterController.animator.SetBool("Use", false);
					crafterController.charState = CrafterState.Idle;
					crafterController.LockMovement(0.3f);
				}
			}

			#endregion
		}
	}
}