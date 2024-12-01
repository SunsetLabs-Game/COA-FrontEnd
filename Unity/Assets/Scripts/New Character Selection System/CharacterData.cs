using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "CharacterSelection/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Parameters")]
    public string characterName;

    [field: Header("Display Parameters")]
    [field: SerializeField] public Sprite displayImage { get; private set; }
    [field: SerializeField] public CharacterAnimationController displayedCharacter { get; private set; }
}
