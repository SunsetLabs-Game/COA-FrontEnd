using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Character stats and damage handling. Fixed: initialMaterials is now serializable and safe to use at runtime.
/// </summary>
public class CharacterStatistic : MonoBehaviour, IDamagabele
{
    private int currentHealth;
    private bool showHealthBar;
    private WaitForSeconds waitForSeconds;

    private Camera mainCamera;
    public int currentEndurance { get; private set; }

    private int deathAnimation;
    private int[] lightDamageAnimationArray;
    private int[] heavyDamageAnimationArray;

    public CharacterManager Character { get; private set; }

    [Header("Components")]
    [SerializeField] private UIBar healthBar;
    [SerializeField] private UIBar enduranceBar;
    [SerializeField] private UIBar hoveringHealthBarAI;

    [Header("Dissolve Material Parameters")]
    [SerializeField] private Material dissolveMaterial;

    // --- Replace dictionary with serializable list so Unity can persist it ---
    [Serializable]
    public class RendererMaterialsEntry
    {
        public SkinnedMeshRenderer renderer;
        public Material[] materials;
    }

    private SkinnedMeshRenderer[] renderers;
    private List<RendererMaterialsEntry> initialMaterialsList = new();

    [Header("Parameters")]
    [SerializeField] private float duration;
    [SerializeField] private int healthLevel;
    [SerializeField] private int enduranceLevel;
    [SerializeField] private Vector3 hoverOffset;

    private void Awake()
    {
        mainCamera = Camera.main;
        Character = GetComponent<CharacterManager>();
    }

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(duration);
        deathAnimation = Animator.StringToHash("Death");

        ResetStats();
        PrepareDamageAnimations();
    }

    private void SetMaterials()
    {
        if (renderers == null || renderers.Length == 0)
        {
            return;
        }

        if (initialMaterialsList == null || initialMaterialsList.Count == 0)
        {
            return;
        }

        foreach (var renderer in renderers)
        {
            if (!renderer.gameObject.activeSelf)
            {
                continue;
            }
            var entry = initialMaterialsList.Find(e => e.renderer == renderer);
            if (entry == null || entry.materials == null || entry.materials.Length == 0)
            {
                continue;
            }

            var mats = new Material[entry.materials.Length];
            for (int i = 0; i < entry.materials.Length; i++)
            {
                mats[i] = entry.materials[i];
            }
            renderer.materials = mats;
        }
    }

    public CharacterManager TakingDamage_Character()
    {
        return Character;
    }

    public void SetBarUIs(UIBar hB, UIBar eB)
    {
        healthBar = hB;
        enduranceBar = eB;
    }

    private void PrepareDamageAnimations()
    {
        int lightDamage1 = Animator.StringToHash("Light Damage 1");
        int lightDamage2 = Animator.StringToHash("Light Damage 2"); // fixed duplicate name

        int heavyDamage1 = Animator.StringToHash("Heavy Damage 1");
        int heavyDamage2 = Animator.StringToHash("Heavy Damage 2");

        lightDamageAnimationArray = new int[] { lightDamage1, lightDamage2 };
        heavyDamageAnimationArray = new int[] { heavyDamage1, heavyDamage2 };
    }

    public void ResetStats()
    {
        if (Character.isDead)
        {
            SetMaterials();
        }
        Character.isDead = false;

        int multiplier = 10;
        if(Character.characterType == CharacterType.AI)
        {
            multiplier = (Character.mentalState == CombatMentalState.High_Alert) ? 7 : 3;
        }
        currentHealth = healthLevel * multiplier;
        currentEndurance = enduranceLevel * multiplier;

        if (healthBar != null)
        {
            healthBar.SetMaxValue(currentHealth);
            healthBar.SetCurrentValue(currentHealth);
        }
        if (hoveringHealthBarAI != null)
        {
            hoveringHealthBarAI.SetBillboard(mainCamera, transform, currentHealth);
            hoveringHealthBarAI.gameObject.SetActive(showHealthBar);
        }
    }

    public void ReduceEndurance(float value)
    {
        // implement endurance reduction if needed
    }

    public void TakeDamage(int damageValue, AttackType attackType)
    {
        currentHealth -= damageValue;
        UpdateHealthBar();
        CharacterAnim anim = Character.AnimatorManagaer;

        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeathRoutine(anim));
            return;
        }
        PlayDamageAnimation(anim, attackType);
    }

    public void PrepareInitialMaterials()
    {
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        initialMaterialsList.Clear();
        foreach (var renderer in renderers)
        {
            Material[] renderMat = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderMat[i] = renderer.sharedMaterials[i];
            }

            var entry = new RendererMaterialsEntry
            {
                renderer = renderer,
                materials = renderMat
            };
            initialMaterialsList.Add(entry);
        }

        #if UNITY_EDITOR
        // mark scene dirty so that the serialized list is saved in editor
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            var scene = gameObject.scene;
            if (scene.IsValid())
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }
        #endif
    }

    public void PlayDamageAnimation(CharacterAnim anim, AttackType attackType)
    {
        if (attackType == AttackType.Light)
        {
            int lightRandom = UnityEngine.Random.Range(0, lightDamageAnimationArray.Length);
            anim.PlayTargetAnimation(lightDamageAnimationArray[lightRandom], true);
            return;
        }
        int heavyRandom = UnityEngine.Random.Range(0, heavyDamageAnimationArray.Length);
        anim.PlayTargetAnimation(heavyDamageAnimationArray[heavyRandom], true);
    }

    private void UpdateHealthBar()
    {
        showHealthBar = true;
        if (Character.characterType != CharacterType.AI || Character.combatMode)
        {
            if (healthBar != null)
                healthBar.SetCurrentValue(currentHealth);
            return;
        }
        if (hoveringHealthBarAI != null)
        {
            hoveringHealthBarAI.SetCurrentValue(currentHealth);
            hoveringHealthBarAI.gameObject.SetActive(showHealthBar);
            StartCoroutine(DisableHealthBar(hoveringHealthBarAI.gameObject));
        }
    }

    private IEnumerator DisableHealthBar(GameObject healthBar)
    {
        yield return waitForSeconds;
        showHealthBar = false;
        if (healthBar != null)
        {
            healthBar.SetActive(false);
        }
    }

    private IEnumerator HandleDeathRoutine(CharacterAnim anim)
    {
        WaitForSeconds wait = new(2.5f);
        CharacterCombat combat = Character.CombatManager;

        currentHealth = 0;
        Character.isDead = true;
        anim.PlayTargetAnimation(deathAnimation, true);

        combat.enemyPossibleWeapons.ForEach(x => Destroy(x.gameObject));
        combat.enemyPossibleWeapons.Clear();

        yield return wait;
        StartCoroutine(DissolveEffect());
    }

    private IEnumerator DissolveEffect()
    {
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var renderer in renderers)
        {
            Material[] dissolveMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < dissolveMaterials.Length; i++)
            {
                dissolveMaterials[i] = dissolveMaterial;
            }
            renderer.materials = dissolveMaterials;
        }

        float elapsedTime = 0f;
        float dissolveTime = 3.0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float cutoff = Mathf.Lerp(4f, -5f, elapsedTime / dissolveTime);

            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    // use property name you have in the shader, consider caching the id for perf
                    material.SetFloat("Vector1_CFBBCBA", cutoff);
                }
            }
            yield return null;
        }
        Character.mySpawnPool?.Release(Character);
    }
}
