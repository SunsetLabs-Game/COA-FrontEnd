public interface IDamagabele
{
    public CharacterManager TakingDamage_Character();
    public void TakeDamage(int damageValue, AttackType attackType);
}

public interface IInteractable
{
    public void Interact();
    public string GetInteractText();
}