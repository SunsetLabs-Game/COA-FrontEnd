using UnityEngine;

[CreateAssetMenu(fileName = "Attack Action", menuName = "Attack Action")]
public class AttackActions : ScriptableObject
{
    private int attackHash;

    [Header("Attack Information")]
    public int damageValue;
    [SerializeField] private int enduranceCost;
    [SerializeField] public AttackType attackType;
    [SerializeField] private string attackAnimation;

    [field: Header("AI Parameters")]
    public int actionWeight;
    public float recoveryTime;
    [field: SerializeField] public AttackActions comboAction { get; private set; }
    [field: SerializeField] public Boundary angleBoundary { get; private set; }
    [field: SerializeField] public Boundary distanceBoundary { get; private set; }

    public void Initialize()
    {
        attackHash = Animator.StringToHash(attackAnimation);
    }

    public void PerformAction(CharacterManager character)
    {
        CharacterCombat combat = character.CombatManager;

        if(character.StatsManager.currentEndurance <= enduranceCost)
        {
            combat.canCombo = false;
            return;
        }

        if(combat.canCombo == true)
        {
            HandleCombo(character);
            return;
        }
        HandleAttack(character);
    }

    private void HandleAttack(CharacterManager character)
    {
        if (character.performingAction)
        {
            return;
        }
        character.isAttacking = true;
        character.CombatManager.attackType = attackType;

        character.StatsManager.ReduceEndurance(enduranceCost);
        character.AnimatorManagaer.PlayTargetAnimation(attackHash, true);
    }

    private void HandleCombo(CharacterManager character)
    {
        if (character.performingAction)
        {
            return;
        }
        CharacterCombat combat = character.CombatManager;

        combat.canCombo = false;
        combat.currentAction = comboAction;
        comboAction.HandleAttack(character);  
    }
}
