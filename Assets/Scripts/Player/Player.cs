using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    public InputAction movement;
    public InputAction boost;
    public InputAction rotate;

    public event Action triggRotation;

    private Vector2 movementInputVec;
    private float boostInput;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToReachApex = .4f;
    public float moveSpeed = 6;
    private float reachOfPlayer = 2;

    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;

    float gravity;
    float jumpVelocity;

    float veclocityXSmoothing;
    float veclocityYSmoothing;
    float knockBackSmoothing;

    int knockBackDir;
    bool inRange;

    public float knockBackSpeedX;
    public float knockBackSpeedY;

    Vector3 velocity;
    Controller2D controller;

    private Animator anim;
    int layerMask = 1 << 6; //this is hard code change it to get layer number through string

    public InteractionSystem interactions;
    public GameObject levelUI;

    private void Awake()
    {
        movement.performed += Movement_performed;
        movement.canceled += Movement_canceled;

        boost.performed += Boost_performed;
        boost.canceled += Boost_canceled;

        rotate.performed += Rotate_performed;
        layerMask = ~layerMask;
    }

    void Start()
    {
        controller = GetComponent<Controller2D>();
        anim = GetComponent<Animator>();

        anim.SetBool("isRunning", false);
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToReachApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToReachApex;

        StartCoroutine(CheckLeverProximity());
    }

    private void OnEnable()
    {
        movement.Enable();
        boost.Enable();
        rotate.Enable();
    }

    private void OnDisable()
    {
        movement.Disable();
        boost.Disable();
        rotate.Disable();
    }

    private void Boost_performed(InputAction.CallbackContext context)
    {
        boostInput = context.ReadValue<float>();
    }

    private void Boost_canceled(InputAction.CallbackContext obj)
    {
        boostInput = 0;
    }

    private void Movement_performed(InputAction.CallbackContext context)
    {
        movementInputVec = context.ReadValue<Vector2>();
    }

    private void Movement_canceled(InputAction.CallbackContext obj)
    {
        movementInputVec = new Vector2(0, 0);
    }

    private void Rotate_performed(InputAction.CallbackContext obj)
    {
        if(inRange)
        {
            if (triggRotation != null)
            {
                triggRotation();
            }
        }
    }

    private void Update()
    {
        if(GetComponent<Health>().GetHealth() == 0)
        {
            Destroy(gameObject);
            levelUI.SetActive(true);
        }

        if(GameObject.FindGameObjectWithTag("Player1") == null)
        {
            levelUI.GetComponent<LevelUI>().DisplayWinPrompt("Player2");
        }
        if (GameObject.FindGameObjectWithTag("Player2") == null)
        {
            levelUI.GetComponent<LevelUI>().DisplayWinPrompt("Player1");
        }

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        if(movementInputVec.y > 0 && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        if(boostInput == 1)
        {
            float boostVelocity = jumpVelocity;
            velocity.y = Mathf.SmoothDamp(velocity.y, boostVelocity, ref veclocityYSmoothing, 0.1f);
        }

        if (controller.collisions.knockBackX)
        {
            if (controller.collisions.left) knockBackDir = 1;
            if (controller.collisions.right) knockBackDir = -1;
            StartCoroutine(KnockBackX());
        }

        if (controller.collisions.knockBackY)
        {
            if (controller.collisions.below) knockBackDir = 1;
            if (controller.collisions.above) knockBackDir = -1;
            StartCoroutine(KnockBackY());
        }

        else
        {
            float targetVelocity = movementInputVec.x * moveSpeed;
            if (targetVelocity != 0)
            {
                anim.SetBool("isRunning", true);
            }
            else
            {
                anim.SetBool("isRunning", false);
            }
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref veclocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }

        velocity.y += gravity * Time.deltaTime; //gravity is negative here
        //if(controller.collisions.knockback) controller.collisions.knockback = false;
        controller.Move(velocity * Time.deltaTime);
    }

    private IEnumerator KnockBackX()
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, knockBackSpeedX * knockBackDir, ref knockBackSmoothing, 0.1f);
        yield return new WaitForSeconds(.1f);
        controller.collisions.knockBackX = false;
        yield break;
    }

    private IEnumerator KnockBackY()
    {
        velocity.y = Mathf.SmoothDamp(velocity.y, knockBackSpeedY * knockBackDir, ref knockBackSmoothing, 0.1f);
        yield return new WaitForSeconds(.1f);
        controller.collisions.knockBackY = false;
        yield break;
    }

    private IEnumerator CheckLeverProximity() 
    {
        while(true)
        {
            if (interactions.leverOnCoolDown)
            {
                yield return null;
            }

            else
            {
                Collider2D hitObj = Physics2D.OverlapCircle(transform.position, reachOfPlayer, 1 << LayerMask.NameToLayer("Lever"));
                if (hitObj)
                {
                    if (hitObj.gameObject.tag == "lever")
                    {
                        inRange = true;
                    }
                    else
                    {
                        inRange = false;
                    }
                }
            }

            yield return new WaitForSeconds(Random.Range(0.016f, 0.033f));
        }
    }
}
