using UnityEngine;

public class InputManager : MonoBehaviour
{
    private CharacterManager characterManager;

    public bool jumpInput { get; private set; }
    public bool dashInput { get; private set; }
    public bool lightAttackInput { get; private set; }
    public bool heavyAttackInput { get; private set; }

    public float moveAmount { get; private set; }
    public float verticalMoveInput { get; private set; }
    public float horizontalMoveInput { get; private set; }

    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    public void InputManager_Update()
    {
        HandleMovement();

        dashInput = Input.GetKey(KeyCode.LeftShift);
        jumpInput = Input.GetKeyDown(KeyCode.Space);
        lightAttackInput = Input.GetKeyDown(KeyCode.L);
        heavyAttackInput = Input.GetKeyDown(KeyCode.K);
    }

    private void HandleMovement()
    {
        verticalMoveInput = Input.GetAxis("Vertical");
        horizontalMoveInput = Input.GetAxis("Horizontal");

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalMoveInput) + Mathf.Abs(horizontalMoveInput));
        characterManager.isMoving = (moveAmount > 0.0f);
    }
}
