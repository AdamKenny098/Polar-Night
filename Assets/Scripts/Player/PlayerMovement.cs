// Author: Adam Kenny
// Student: Applied Computing (Game Development) 3rd Year (20102588)
// Date Created: 2025-08-15
// Description: Handles player movement, including walking, sprinting, and stamina management.
//              Gravity Added so player can explore the outside without the risk of being suspended mid air.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10f; // Base movement speed
    public float sprintMultiplier = 1.75f; // Multiplier for sprinting speed
    private CharacterController controller; // Reference to CharacterController
    [SerializeField] private Animator animator; // Reference to Animator
    public Camera fpsCam;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaRegenRate = 10f;
    public float staminaDrainRate = 20f;
    public float staminaCooldown = 2f;
    public Image staminaImage;

    private float regenCooldownTimer;
    public bool isSprinting = false;

    public static PlayerMovement Instance;

    public Vector3 velocity;
    public float gravity = -9.81f;
    

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Safely get Animator on this or child object
        animator = GetComponentInChildren<Animator>();

        currentStamina = maxStamina;
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        HandleMovement();
        HandleStamina();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small nudge to stay grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        PlayerNeedsUI.Instance.UpdateStaminaUI(isSprinting);
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        //Move = right/left + forward/back (if moveX is 1, move right. If -1, move left. Same for moveZ)
        Vector3 move = fpsCam.transform.right * moveX + fpsCam.transform.forward * moveZ;
        move.y = 0; // Keep movement horizontal

        bool hasMovement = move.magnitude > 0.1f;
        animator.SetBool("isWalking", hasMovement);

        // Sprint only if moving and has stamina
        isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && hasMovement;

        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        if (animator)
        {
            animator.SetFloat("Speed", move.magnitude * (isSprinting ? sprintMultiplier : 1));
        }

        if (isSprinting && currentStamina > 0)
        {
            GameManager.Instance.sprintTime += Time.deltaTime;
        }
    }

    private void HandleStamina()
    {
        if (isSprinting && GameManager.Instance.UpgradeUnlocked(GameManager.PlayerUpgrade.BetterBoots))
        {
            currentStamina -= staminaDrainRate * Time.deltaTime * 0.66f;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            regenCooldownTimer = staminaCooldown;

        }
        else if (isSprinting && !GameManager.Instance.UpgradeUnlocked(GameManager.PlayerUpgrade.BetterBoots))
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            regenCooldownTimer = staminaCooldown;

        }
        else
        {
            if (regenCooldownTimer > 0)
            {
                regenCooldownTimer -= Time.deltaTime;
            }
            else
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
            }
        }
    }
    
    

}
