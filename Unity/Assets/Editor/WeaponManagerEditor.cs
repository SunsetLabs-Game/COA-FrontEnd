using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(WeaponManager), true)]
public class WeaponManagerEditor : Editor
{
    private SerializedProperty script;
    private SerializedProperty saveable;

    private SerializedProperty m_Items;
    private ReorderableList m_ItemList;
    private SerializedProperty m_Amounts;

    private SerializedProperty m_Modifiers;
    private ReorderableList m_ModifierList;

    // Add SerializedProperty for the additional fields you want to display
    private SerializedProperty rigidBody;
    private SerializedProperty weaponMeshCollider;
    private SerializedProperty weaponTriggerCollider;

    private void OnEnable()
    {
        this.script = serializedObject.FindProperty("m_Script");
        this.saveable = serializedObject.FindProperty("saveable");

        // Initialize the additional properties
        this.rigidBody = serializedObject.FindProperty("rigidBody");
        this.weaponMeshCollider = serializedObject.FindProperty("weaponMeshCollider");
        this.weaponTriggerCollider = serializedObject.FindProperty("weaponTriggerCollider");

        // Initialize weaponItems and weaponModifiers
        this.m_Amounts = serializedObject.FindProperty("amount");
        this.m_Items = serializedObject.FindProperty("weaponItems");
        this.m_Modifiers = serializedObject.FindProperty("weaponModifiers");
        CreateItemList(serializedObject, this.m_Items);
    }

    private void CreateItemList(SerializedObject serializedObject, SerializedProperty elements)
    {
        this.m_ItemList = new ReorderableList(serializedObject, elements, true, true, true, true);
        this.m_ItemList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Weapon Item (Single Item)");
        };

        this.m_ItemList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            float verticalOffset = (rect.height - EditorGUIUtility.singleLineHeight) * 0.5f;
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y = rect.y + verticalOffset;
            rect.width = rect.width - 52f;
            SerializedProperty element = elements.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none, true);

            SerializedProperty amount = m_Amounts.GetArrayElementAtIndex(index);
            rect.x += rect.width + 2f;
            rect.width = 50f;
            EditorGUI.PropertyField(rect, amount, GUIContent.none);

            // If there are multiple elements in weaponItems, only show the first one.
        };

        this.m_ItemList.onReorderCallbackWithDetails = (ReorderableList list, int oldIndex, int newIndex) => {
            this.m_Amounts.MoveArrayElement(oldIndex, newIndex);
        };

        this.m_ItemList.onAddCallback = (ReorderableList list) => {
            ReorderableList.defaultBehaviours.DoAddButton(list);
            this.m_Amounts.InsertArrayElementAtIndex(list.index);
        };

        this.m_ItemList.onRemoveCallback = (ReorderableList list) =>
        {
            this.m_Amounts.DeleteArrayElementAtIndex(list.index);
            SerializedProperty amounts = serializedObject.FindProperty("amount");
            amounts.DeleteArrayElementAtIndex(list.index);

            list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        };
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(script);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.PropertyField(rigidBody);
        EditorGUILayout.PropertyField(weaponMeshCollider);
        EditorGUILayout.PropertyField(weaponTriggerCollider);

        serializedObject.Update();
        EditorGUILayout.PropertyField(saveable);
        GUILayout.Space(3f);

        m_ItemList.DoLayoutList();
        EditorGUILayout.Space();

        if (this.m_ModifierList != null)
        {
            this.m_ModifierList.DoLayoutList();
        }
        serializedObject.ApplyModifiedProperties();
    }
}