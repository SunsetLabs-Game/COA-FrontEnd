using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Combat", menuName = "AIState/CombatState")]
public class CombatState : AIState
{
    private bool hasAttack;

    private AttackActions currentAction;
    private List<AttackActions> possibleAction = new();

    private float verticalMovement;
    private float horizontalMovement;
    private bool setStrafingDirection;

    [Header("Combo Actions")]
    [SerializeField] private bool canPerformCombo;
    [SerializeField] private float comboLikelyHood;
    [SerializeField] private bool hasRolledForCombo;

    [Header("Parameters")]
    [SerializeField] private float maxEngagementDistance;
    [SerializeField] private AttackActions[] actionsArray;

    public void Initialize()
    {
        possibleAction.Clear();

        for (int i = 0; i < actionsArray.Length; i++)
        {
            actionsArray[i] = Instantiate(actionsArray[i]);
            actionsArray[i].Initialize();
        }
    }

    public override AIState StateUpdater(CharacterManager characterManager)
    {
        NavMeshAgent agent = characterManager.Agent;
        CharacterAnim animManager = characterManager.AnimatorManagaer;
        CharacterMovement moveManager = characterManager.MovementManager;

        if (characterManager.performingAction || characterManager.Target == null)
        {
            animManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
            return this;
        }

        if (agent.enabled != true)
        {
            agent.enabled = true;
        }
        moveManager.RotateTowardsTarget();

        if(hasAttack != true)
        {
            GetOffensiveActions(characterManager);
        }
        else
        {
            characterManager.Attack.currentAttack = currentAction;
            return SwitchState(characterManager, characterManager.Attack);
        }

        if (characterManager.DistanceToTarget >= maxEngagementDistance)
        {
            return SwitchState(characterManager, characterManager.Pursue);
        }

        characterManager.isMoving = true;
        animManager.SetBlendTreeParameter(verticalMovement, horizontalMovement, false, Time.deltaTime);
        return this;
    }

    public void HandleStrafing(CharacterManager character)
    {
        if (setStrafingDirection == true)
        {
            return;
        }

        setStrafingDirection = true;
        horizontalMovement = RandomValue();
        verticalMovement = character.DistanceToTarget / 3.5f;
    }

    private float RandomValue()
    {
        float randomValue = Random.Range(-1, 1);

        if (randomValue >= -1.0f && randomValue <= 0.0f)
        {
            return -0.5f;
        }
        return 0.5f;
    }


    protected override void ResetStateParameters(CharacterManager character)
    {
        base.ResetStateParameters(character);

        hasAttack = false;
        currentAction = null;
        possibleAction.Clear();
        setStrafingDirection = false;
    }

    private void GetOffensiveActions(CharacterManager characterManager)
    {
        for(int i = 0; i < actionsArray.Length; i++)
        {
            if (possibleAction.Contains(actionsArray[i]))
            {
                continue;
            }

            if (actionsArray[i].angleBoundary.minValue > characterManager.AngleTarget)
            {
                continue;
            }

            if (actionsArray[i].angleBoundary.maxValue < characterManager.AngleTarget)
            {
                continue;
            }

            if (actionsArray[i].distanceBoundary.minValue > characterManager.DistanceToTarget)
            {
                continue;
            }

            if(actionsArray[i].distanceBoundary.maxValue < characterManager.DistanceToTarget)
            {
                continue;
            }
            possibleAction.Add(actionsArray[i]);
        }

        int totalWeight = 0;
        for(int i = 0; i < possibleAction.Count; i++)
        {
            totalWeight += possibleAction[i].actionWeight;
        }

        int processedWeight = 0;
        int randomWeight = Random.Range(0, totalWeight + 1);
        for(int i = 0; i < possibleAction.Count; i++)
        {
            processedWeight += possibleAction[i].actionWeight;
            if(randomWeight <= processedWeight)
            {
                hasAttack = true;
                currentAction = possibleAction[i];
                return;
            }
        }
    }
}
