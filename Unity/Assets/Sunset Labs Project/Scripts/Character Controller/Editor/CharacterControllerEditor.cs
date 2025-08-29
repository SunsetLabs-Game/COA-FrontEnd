using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterCombat))]
public class CharacterCombatEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CharacterCombat combat = (CharacterCombat)target;

        if(GUILayout.Button("Set Damage Colliders"))
        {
            combat.SetDamageColliders();
        }
    }
}

[CustomEditor(typeof(CharacterStatistic))]
public class CharacterStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CharacterStatistic stat = (CharacterStatistic)target;

        if(GUILayout.Button("Store Material"))
        {
            stat.PrepareInitialMaterials();
        }
    }
}

