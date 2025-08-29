using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "CharacterSelection/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Parameters")]
    public string characterName;

    [field: Header("Display Parameters")]
    [field: SerializeField] public Sprite DisplayImage { get; private set; }
    [field: SerializeField] public CharacterManager PlayableCharacter { get; private set; }
    [field: SerializeField] public CharacterAnimationController DisplayedCharacter { get; private set; }
}
