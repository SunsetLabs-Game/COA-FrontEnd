using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "AIState/AttackState")]
public class AttackState : AIState
{
    private bool willPerformCombo;
    public AttackActions currentAttack;

    private bool pivotAfterAttack;
    private bool hasPerformedCombo;
    private bool hasPerformedAttack;

    public override AIState StateUpdater(CharacterManager characterManager)
    {
        if (characterManager.performingAction || characterManager.Target == null)
        {
            return this;
        }

        characterManager.AnimatorManagaer.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

        if (willPerformCombo && hasPerformedCombo != true)
        {
            if(currentAttack.comboAction != null)
            {
                //hasPerformedCombo = true;
                //combat.currentAction = currentAttack;
                //currentAttack.comboAction.PerformAction(characterManager);
            }
        }

        if(!hasPerformedAttack)
        {
            if(characterManager.CombatManager.currentRecovery > 0)
            {
                return this;
            }

            if(characterManager.performingAction)
            {
                return this;
            }
            PerformAttack(characterManager);
            return this;
        }
        return SwitchState(characterManager, characterManager.Combat);
    }

    private void PerformAttack(CharacterManager character)
    {
        CharacterCombat combat = character.CombatManager;

        hasPerformedAttack = true;
        combat.currentAction = currentAttack;
        currentAttack.PerformAction(character);
        combat.currentRecovery = currentAttack.recoveryTime;
    }

    protected override void ResetStateParameters(CharacterManager character)
    {
        currentAttack = null;
        willPerformCombo = false;
        hasPerformedCombo = false;
        hasPerformedAttack = false;
    }
}