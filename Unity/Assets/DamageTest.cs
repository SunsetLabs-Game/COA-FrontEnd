using UnityEngine;

public class DamageTest : MonoBehaviour, IDamagabele
{
    private Material selectedMaterial;

    [Header("Parameters")]
    [SerializeField] private MeshRenderer m_Renderer;
    [SerializeField] private Material[] materials;

    public void TakeDamage(int damage, AttackType attack)
    {
        print($"Taking damage: {damage} with attack type: {attack}");
        selectedMaterial = GameObjectTool.GetRandomExcluding(selectedMaterial, materials);
        m_Renderer.material = selectedMaterial;
    }

    public CharacterManager TakingDamage_Character()
    {
        return null;
    }
}
