using UnityEngine;

public abstract class AIState : ScriptableObject
{
    public abstract AIState StateUpdater(CharacterManager characterManager);

    public AIState SwitchState(CharacterManager character, AIState nextState)
    {
        ResetStateParameters(character);
        return nextState;
    }

    protected virtual void ResetStateParameters(CharacterManager character)
    {

    }

    protected bool RollOutComeChance(int outcomeChance)
    {
        int random = Random.Range(0, 100);
        if (random <= outcomeChance)
        {
            return true;
        }
        return false;
    }
}
