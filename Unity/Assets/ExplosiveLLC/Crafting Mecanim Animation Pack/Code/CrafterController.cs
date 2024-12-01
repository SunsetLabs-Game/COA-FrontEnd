using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace CraftingAnims
{
    public class CrafterController : MonoBehaviour
    {
        // Components.
        [HideInInspector] public Animator animator;
        [HideInInspector] public CrafterShowItem showItem;
        [HideInInspector] public CrafterIKHands crafterIKhands;
        [HideInInspector] public GUIControls guiControls;
        [HideInInspector] public Rigidbody rb;
        [HideInInspector] public CrafterActions crafterActions;


        // Variables.
        public CrafterState charState;
        public float animationSpeed = 1;

        // Objects.
        public GameObject hatchet;
        public GameObject hammer;
        public GameObject fishingpole;
        public GameObject shovel;
        public GameObject box;
        public GameObject food;
        public GameObject drink;
        public GameObject saw;
        public GameObject pickaxe;
        public GameObject sickle;
        public GameObject rake;
        public GameObject chair;
        public GameObject ladder;
        public GameObject lumber;
        public GameObject pushpull;
        public GameObject sphere;
        public GameObject cart;
        public GameObject paintbrush;
        public GameObject spear;

        // Actions
        [HideInInspector] public bool isMoving;
        [HideInInspector] public bool isLocked;
        [HideInInspector] public bool isGrounded;
        [HideInInspector] public bool isSpearfishing;
        private Coroutine coroutineLock = null;
        private Vector3 newVelocity;
        private bool isFacing = false;
        private bool isRunning = false;
        private float pushpullTime = 0f;
        private bool carryItem = false;

        // Input.
        private bool allowedInput = true;
        private Vector3 inputVec;
        private float inputHorizontal = 0f;
        private float inputVertical = 0f;
        private float inputHorizontal2 = 0f;
        private float inputVertical2 = 0f;
        private bool inputFacing;
        private bool inputRun;

        [Header("Movement")]
        public float rotationSpeed = 10f;
        public float runSpeed = 8f;
        public float walkSpeed = 4f;
        public float spearfishingSpeed = 1.25f;
        public float crawlSpeed = 1f;

        [Header("Navigation")]
        public bool useNavMeshNavigation = false;
        [HideInInspector] public CrafterNavigation crafterNavigation;
        [HideInInspector] public bool navMeshNavigation = false;
        [HideInInspector] public bool navMeshRun = false;

        private void Awake()
        {
            // Setup animator.
            animator = GetComponentInChildren<Animator>();

            if (animator)
            {
                animator.gameObject.AddComponent<CrafterAnimatorController>();
                animator.GetComponent<CrafterAnimatorController>().crafterController = this;
                animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
                animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                animator.SetLayerWeight(1, 1f);
                animator.SetLayerWeight(2, 0f);
            }

            if (crafterActions == null)
            {
                crafterActions = GetComponent<CrafterActions>();
            }

            if (crafterActions == null)
            {
                Debug.LogError("CrafterActions no está asignado en CrafterController.");
            }

            rb = GetComponent<Rigidbody>();
            guiControls = GetComponent<GUIControls>();
            showItem = GetComponent<CrafterShowItem>();
            crafterIKhands = GetComponentInChildren<CrafterIKHands>();

            // Configurar la cámara
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            cameraController.target = this.transform;

            // Configurar o eliminar componentes de navegación según la configuración
            if (useNavMeshNavigation)
            {
                crafterNavigation = GetComponent<CrafterNavigation>();
            }
            else
            {
                Destroy(GetComponent<CrafterNavigation>());
                Destroy(GetComponent<NavMeshAgent>());
                Destroy(GameObject.Find("Nav"));
            }
        }


        private void Start()
        {
            showItem.ItemShow("none", 0);
            charState = CrafterState.Idle;
        }

        /// <summary>
        /// Input abstraction for easier asset updates using outside control schemes.
        /// </summary>
        private void Inputs()
        {
            try
            {
                // Inputs de movimiento
                inputHorizontal = Input.GetAxisRaw("Horizontal");
                inputVertical = -(Input.GetAxisRaw("Vertical"));

                inputHorizontal2 = Input.GetAxisRaw("Horizontal2");
                inputVertical2 = -(Input.GetAxisRaw("Vertical2"));

                inputFacing = Input.GetButton("Aiming");
                inputRun = Input.GetButton("Fire3");

                // Rotación del personaje hacia el ratón cuando no estamos rotando la cámara
                if (!Input.GetMouseButton(1))  // Si no estamos usando el botón derecho del ratón
                {
                    RotateTowardsMouse();
                }
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Inputs not found! Please see Readme file in Documentation folder.");
            }
        }

        // Método para rotar el personaje hacia el puntero del ratón
        private void RotateTowardsMouse()
        {
            Plane groundPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (groundPlane.Raycast(ray, out float hitDist))
            {
                Vector3 point = ray.GetPoint(hitDist);
                Vector3 direction = (point - transform.position).normalized;

                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                }
            }
        }



        #region Updates

        private void Update()
        {
            // Check if input is allowed.  Disable it if using NavMesh.
            if (allowedInput && !navMeshNavigation) { Inputs(); }

            // Facing switch.
            if (inputFacing) { isFacing = true; }
            else { isFacing = false; }

            // Slow time.
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (Time.timeScale != 1) { Time.timeScale = 1; }
                else { Time.timeScale = 0.15f; }
            }

            // Pause.
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale != 1) { Time.timeScale = 1; }
                else { Time.timeScale = 0f; }
            }

            // Push-Pull
            if (charState != CrafterState.PushPull) { CameraRelativeInput(); }
            else { PushPull(); }

            // Map Alpha keys to actions using a dictionary for efficiency.
            Dictionary<KeyCode, string> actionMapping = new Dictionary<KeyCode, string>
            {
                { KeyCode.Alpha1, "Use" },
                { KeyCode.Alpha2, "Stop Use" },
                { KeyCode.Alpha3, "Gather" },
                { KeyCode.Alpha4, "Gather Kneeling" },
                { KeyCode.Alpha5, "Wave1" },
                { KeyCode.Alpha6, "Scratch Head" },
                { KeyCode.Alpha7, "Cheer1" },
                { KeyCode.Alpha8, "Cheer2" }
            };

            // Iterate over the dictionary to trigger actions when keys are pressed.
            foreach (var action in actionMapping)
            {
                if (Input.GetKeyDown(action.Key))
                {
                    crafterActions.TakeAction(action.Value);
                }
            }


        }



        private void FixedUpdate()
        {
            CheckForGrounded();

            // If locked, apply Root motion.
            if (!isLocked)
            {
                if (charState == CrafterState.Climb
                    || charState == CrafterState.PushPull
                    || charState == CrafterState.Laydown
                    || charState == CrafterState.Use)
                {
                    animator.applyRootMotion = true;
                    isMoving = false;
                    rb.useGravity = false;
                }
                else
                {
                    animator.applyRootMotion = false;
                    rb.useGravity = true;
                }
            }

            // Change animator Animation Speed.
            animator.SetFloat("AnimationSpeed", animationSpeed);
        }

        private void LateUpdate()
        {
            // Si se está corriendo, asegurarse de que el personaje no esté realizando ciertas acciones.
            if (inputRun)
            {
                if (charState != CrafterState.Box && charState != CrafterState.Cart && charState != CrafterState.Overhead
                    && charState != CrafterState.PushPull && charState != CrafterState.Lumber && charState != CrafterState.Use)
                {
                    isRunning = true;
                    isFacing = false;
                }
            }
            else
            {
                isRunning = false;
            }

            // Si no estamos usando Navmesh, actualizamos el movimiento manualmente.
            if (!navMeshNavigation)
            {
                if (UpdateMovement() > 0)
                {
                    isMoving = true;
                    animator.SetBool("Moving", true);
                }
                else
                {
                    isMoving = false;
                    animator.SetBool("Moving", false);
                }

                // Obtener la velocidad local del personaje y actualizar el animator.
                float velocityZ = transform.InverseTransformDirection(rb.velocity).z;
                float velocityX = transform.InverseTransformDirection(rb.velocity).x;

                // Actualizamos el Animator con los valores de velocidad si no se está realizando "PushPull".
                if (charState != CrafterState.PushPull)
                {
                    animator.SetFloat("Velocity Z", velocityZ / runSpeed);
                    animator.SetFloat("Velocity X", velocityX / runSpeed);
                }

                // Si no se está rotando la cámara, rotar al personaje hacia el ratón.
                if (!Input.GetMouseButton(1))  // Si no se está presionando el botón derecho del ratón
                {
                    RotateTowardsMouse();  // Rotar el personaje hacia el puntero del ratón
                }
            }
        }


        /// <summary>
        /// Moves the character.
        /// </summary>
        private float UpdateMovement()
        {
            Vector3 motion = inputVec;

            // Reduce input for diagonal movement.
            if (motion.magnitude > 1) { motion.Normalize(); }

            if (!isLocked
                && charState != CrafterState.PushPull
                && charState != CrafterState.Laydown
                && charState != CrafterState.Crawl)
            {

                // Set speed by walking / running.
                if (isRunning) { newVelocity = motion * runSpeed; }
                else if (charState == CrafterState.Spearfishing) { newVelocity = motion * spearfishingSpeed; }
                else { newVelocity = motion * walkSpeed; }
            }
            else if (charState == CrafterState.Crawl) { newVelocity = motion * crawlSpeed; }

            // Aiming or rotate towards movement direction.
            if (isFacing
                && charState != CrafterState.Box
                && charState != CrafterState.Lumber
                && charState != CrafterState.Overhead)
            {
                Facing();
            }
            else
            {
                if (!isLocked && charState != CrafterState.PushPull
                    && charState != CrafterState.Laydown
                    && charState != CrafterState.Use)
                {
                    RotateTowardsMovementDir();
                }
            }

            // If character is falling use momentum.
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;

            // Return a movement value for the animator.
            return inputVec.magnitude;
        }

        #endregion

        /// <summary>
        /// Checks if character is within a certain distance from the ground, and markes it IsGrounded.
        /// </summary>
        private void CheckForGrounded()
        {
            float distanceToGround;
            float threshold = .45f;
            Vector3 offset = new Vector3(0, 0.4f, 0);
            if (Physics.Raycast((transform.position + offset), -Vector3.up, out RaycastHit hit, 100f))
            {
                distanceToGround = hit.distance;
                if (distanceToGround < threshold) { isGrounded = true; }
                else { isGrounded = false; }
            }
        }

        /// <summary>
        /// All movement is based off camera facing.
        /// </summary>
        private void CameraRelativeInput()
        {
            // Movimiento relativo a la cámara
            Transform cameraTransform = Camera.main.transform;

            // Vector hacia adelante relativo a la cámara en el plano x-z
            Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
            forward.y = 0;
            forward = forward.normalized;

            // Vector hacia la derecha relativo a la cámara, ortogonal al vector forward
            Vector3 right = new Vector3(forward.z, 0, -forward.x);

            // Entradas direccionales
            inputVec = inputHorizontal * right + -inputVertical * forward;

            // Solo rotar hacia la dirección de movimiento si no estamos rotando la cámara
            if (!Input.GetMouseButton(1))  // Si no se está presionando el botón derecho
            {
                RotateTowardsMovementDir();  // Rotar el personaje hacia la dirección de movimiento
            }
        }
        /// <summary>
        /// Used when the Crafter is in Push/Pull mode.
        /// </summary>
        private void PushPull()
        {
            if (inputHorizontal == 0 && inputVertical == 0) { pushpullTime = 0; }
            if (inputHorizontal != 0) { inputVertical = 0; }
            if (inputVertical != 0) { inputHorizontal = 0; }

            pushpullTime += 0.5f * Time.deltaTime;
            float h = Mathf.Lerp(0, inputHorizontal, pushpullTime);
            float v = Mathf.Lerp(0, inputVertical, pushpullTime);

            animator.SetFloat("Velocity Z", v);
            animator.SetFloat("Velocity X", h);
        }


        /// <summary>
        /// Faces Crafter= along input direction.
        /// </summary>
        private void RotateTowardsMovementDir()
        {
            if (inputVec != Vector3.zero)
            {
                // Rota suavemente hacia la dirección del movimiento
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(inputVec),
                    Time.deltaTime * rotationSpeed);
            }
        }


        /// <summary>
        /// For facing the Crafter in a different direction than the Crafter.
        /// </summary>
        private void Facing()
        {
            // Verificar si hay un joystick conectado
            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                // Si el joystick está moviendo la palanca derecha, usarla para rotar el personaje
                if (Mathf.Abs(inputHorizontal2) > 0.1f || Mathf.Abs(inputVertical2) < -0.1f)
                {
                    Vector3 joyDirection = new Vector3(inputHorizontal2, 0, -inputVertical2).normalized;
                    Quaternion joyRotation = Quaternion.LookRotation(joyDirection);
                    transform.rotation = joyRotation;
                    return;  // Salir de la función, ya que estamos usando el joystick
                }
            }

            // Si no hay joysticks conectados o no se está usando el joystick, usar el ratón
            if (!Input.GetMouseButton(1))  // Si no se está usando el botón derecho del ratón
            {
                Plane characterPlane = new Plane(Vector3.up, transform.position);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (characterPlane.Raycast(ray, out float hitdist))
                {
                    Vector3 mousePosition = ray.GetPoint(hitdist);
                    mousePosition.y = transform.position.y;  // Asegurar que el personaje no gire hacia arriba/abajo
                    Vector3 relativePos = transform.position - mousePosition;

                    // Rotar el personaje hacia la posición del ratón
                    Quaternion rotation = Quaternion.LookRotation(-relativePos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }
            }
        }


        /// <summary>
        /// Prevents the Crafter from moving to apply Root Motion and let the animation drive the character position.
        /// </summary>
        /// <param name="locktime"></param>
        public void LockMovement(float locktime)
        {
            if (coroutineLock != null) { StopCoroutine(coroutineLock); }
            coroutineLock = StartCoroutine(_LockMovement(locktime));
        }

        private IEnumerator _LockMovement(float locktime)
        {
            allowedInput = false;
            isLocked = true;
            animator.applyRootMotion = true;

            if (locktime != -1f)  // Si locktime es -1, el movimiento permanece bloqueado indefinidamente
            {
                yield return new WaitForSeconds(locktime);
                isLocked = false;
                animator.applyRootMotion = false;
                allowedInput = true;
            }
        }

        /// <summary>
        /// Sets the CrafterState with a possible wait duration.
        /// </summary>
        /// <param name="waitTime"></param>
        /// <param name="state"></param>
        public void ChangeCharacterState(float waitTime, CrafterState state)
        {
            StartCoroutine(_ChangeCharacterState(waitTime, state));
        }

        private IEnumerator _ChangeCharacterState(float waitTime, CrafterState state)
        {
            yield return new WaitForSeconds(waitTime);
            charState = state;
        }



        /// <summary>
        /// Set Animator Trigger using legacy Animation Trigger names.
        /// </summary>
        public void TriggerAnimation(string trigger)
        {
            Debug.Log("TriggerAnimation: " + (CrafterAnimatorTriggers)System.Enum.Parse(typeof(CrafterAnimatorTriggers), trigger) +
                      " - " + (int)(CrafterAnimatorTriggers)System.Enum.Parse(typeof(CrafterAnimatorTriggers), trigger));
            animator.SetInteger("Action", (int)(CrafterAnimatorTriggers)System.Enum.Parse(typeof(CrafterAnimatorTriggers), trigger));
            animator.SetTrigger("Trigger");
        }


        #region AnimationLayerBlending - IK

        /// <summary>
        /// Uses Avatar mask to Blend a Right Hand fist for holding items.
        /// </summary>
        /// <param name="use"></param>
        public void RightHandBlend(bool use)
        { StartCoroutine(_RightHandBlend(use)); }

        private IEnumerator _RightHandBlend(bool use)
        {
            if (use)
            {
                float counter = 0f;
                while (counter < 1)
                {
                    counter += 0.05f;
                    yield return new WaitForEndOfFrame();
                    animator.SetLayerWeight(3, counter);
                }
                animator.SetLayerWeight(3, 1);
            }
            else
            {
                float counter = 1f;
                while (counter > 0)
                {
                    counter -= 0.05f;
                    yield return new WaitForEndOfFrame();
                    animator.SetLayerWeight(3, counter);
                }
                animator.SetLayerWeight(3, 0);
            }
        }

        private IEnumerator _RightHandBlendOff(float time)
        {
            yield return new WaitForSeconds(time);
            StartCoroutine(_RightHandBlend(false));
        }

        private IEnumerator _RightArmBlendOff(float time)
        {
            if (carryItem)
            {
                yield return new WaitForSeconds(time);
                StartCoroutine(_RightArmBlend(false));
            }
        }

        /// <summary>
        /// Uses Avatar mask to Blend a Right arm for carrying items like a torch.
        /// </summary>
        /// <param name="use"></param>
        public void RightArmBlend(bool use)
        { StartCoroutine(_RightArmBlend(use)); }

        private IEnumerator _RightArmBlend(bool use)
        {
            if (use)
            {
                float counter = 0f;
                while (counter < 1)
                {
                    counter += 0.05f;
                    yield return new WaitForEndOfFrame();
                    animator.SetLayerWeight(2, counter);
                }
                animator.SetLayerWeight(2, 1);
                carryItem = true;
            }
            else
            {
                float counter = 1f;
                while (counter > 0)
                {
                    counter -= 0.05f;
                    yield return new WaitForEndOfFrame();
                    animator.SetLayerWeight(2, counter);
                }
                animator.SetLayerWeight(2, 0);
                carryItem = false;
            }
        }

        /// <summary>
        /// Blend Right Arm Carry animation on/off.
        /// </summary>
        /// <param name="carry"></param>
        public void CarryItem(bool carry)
        {
            if (carry)
            {
                carryItem = true;
                RightArmBlend(true);
            }
            else
            {
                carryItem = false;
                RightArmBlend(false);
            }
        }

        /// <summary>
        /// Blends all Hand/Arm animations overrides off.
        /// </summary>
        /// <param name="time"></param>
        public void BlendOff(float time)
        {
            guiControls.ResetCarry();
            StartCoroutine(_RightArmBlendOff(time));
            StartCoroutine(_RightHandBlendOff(time));
        }

        /// <summary>
        /// Use CrafterIKHands.cs to blend in the use of IK to position the left hand.
        /// </summary>
        public void IKBlendOn()
        {
            if (crafterIKhands != null) { crafterIKhands.BlendIK(true, 0.75f, 0.5f); }
        }

        /// <summary>
        /// Use CrafterIKHands.cs to blend out the use of IK to release the left hand.
        /// </summary>
        public void IKBlendOff()
        {
            if (crafterIKhands != null) { crafterIKhands.BlendIK(false, 0, 0.25f); }
        }

        #endregion






    }

}