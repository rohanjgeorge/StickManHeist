using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public float speed;
    private float originalSpeed;
    public GameObject xIndicator;
    public GameObject blackFade;

    private Rigidbody2D rb2d;
    private bool nearStaircase = false;
    private bool nearVase = false;
    private bool isCrouching = false;
    private bool isInteracting = false;
    private bool isHiding = false;

    private Transform vasePosition;
    private Transform targetPosition;
    private SpriteRenderer rend;

    public GameObject noiseCirclePrefab;
    public float noiseCircleLifetime = 0.5f;

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        rend = gameObject.GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        originalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHiding)
        {
            // Disable movement when hiding
            if (Input.GetKeyDown(KeyCode.X))
            {
                isHiding = false;
                xIndicator.SetActive(false);

                // Reset the player's sorting order
                rend.sortingOrder = 2;


                // Reset animation parameters when exiting hiding
                isCrouching = false;
                isInteracting = false;
                animator.SetBool("Crouch", isCrouching);
                animator.SetBool("Interact", isInteracting);
                animator.Play("Idle");

                // Re-enable player movement
            }
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        
        // Check if the control key is held down for crouching
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        if (horizontal != 0 && !isCrouching)
        {
            GenerateNoise();
        }

        if (isCrouching)
        {
            speed = originalSpeed * 0.8f; // Reduce speed by 20%
        }
        else
        {
            speed = originalSpeed; // Reset to original speed
        }

        // Check if the X button is pressed for interaction
        isInteracting = Input.GetKeyDown(KeyCode.X);

        MoveCharacter(horizontal);
        PlayMovementAnimation(horizontal);
        PlayCrouchAndInteractAnimations();

        // Check for X input when near a staircase
        if (nearStaircase && Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(TransitionToFloor());
        }
        else if (nearVase && Input.GetKeyDown(KeyCode.X))
        {
            HideBehindVase();
        }
    }

    private void MoveCharacter(float horizontal)
    {

        // move character horizontally
        Vector3 position = transform.position;
        position.x += horizontal * speed * Time.deltaTime;
        transform.position = position;
    }

    private void PlayMovementAnimation(float horizontal)
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontal));

        Vector3 scale = transform.localScale;
        if (horizontal < 0)
        {
            scale.x = -1f * Mathf.Abs(scale.x);
        }
        else if (horizontal > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;
    }

    private void PlayCrouchAndInteractAnimations()
    {
        // Set animator parameters for crouching and interacting
        animator.SetBool("Crouch", isCrouching);
        animator.SetBool("Interact", isInteracting);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Staircase"))
        {
            nearStaircase = true;
            targetPosition = collision.GetComponent<StaircaseController>().targetPosition;
            xIndicator.SetActive(true); // Show "X" sprite
        }
        else if (collision.CompareTag("Vase"))
        {
            nearVase = true;
            vasePosition = collision.transform;
            xIndicator.SetActive(true);
        }
        else if (collision.CompareTag("GuardLight"))
        {
            if (!isHiding)
            {
                // Player got caught in the guard's light
                Debug.Log("Game Over");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Staircase"))
        {
            nearStaircase = false;
            xIndicator.SetActive(false); // Hide "X" sprite
        }
        else if (collision.CompareTag("Vase"))
        {
            nearVase = false;
            xIndicator.SetActive(false);
        }
    }

    private void HideBehindVase()
    {
        isHiding = true;
        xIndicator.SetActive(true); // Show "X" to indicate the player can exit hiding
        // Transport the player behind the vase
        rend.sortingOrder = 0;
        // Disable player movement
    }

    private IEnumerator TransitionToFloor()
    {
        // Activate the BlackFade GameObject
        blackFade.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        // Move player to the target position
        transform.position = targetPosition.position;

        // Deactivate the BlackFade GameObject
        yield return new WaitForSeconds(0.1f);
        blackFade.SetActive(false);
    }

    private void GenerateNoise()
    {
        GameObject noiseCircle = Instantiate(noiseCirclePrefab, transform.position, Quaternion.identity);
        Destroy(noiseCircle, noiseCircleLifetime);
    }
}
