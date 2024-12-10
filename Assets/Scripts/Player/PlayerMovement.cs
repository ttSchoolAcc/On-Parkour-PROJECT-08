using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed = 1f;      // Movement speed
    public float airMoveSpeed = 0.5f;
    public float maxMoveSpeed = 5f;
    public float runningMoveSpeed = 100;
    public float walkMoveSpeed = 30;
    public float maxRunSpeed = 10f;
    public float maxWalkSpeed = 5f;
    public float maxFallSpeed = 10;
    public float jumpForce = 5f;      // Jumping force
    public float wallJumpForce = 5f; 
    public float gravityForce = 2f; // Custom gravity multiplier
    float storedGravityForce;
    public float groundedGravityMult = 80;
    public float deccelerationRate = 0.70f;
    public float airDeccelerationRate = 0.85f;
    public float slideDeccelerationRate = 0.85f;

    public float groundedOffset;
    public float storedGroundedOffset = 0.76f;
    public float mouseSens;


    public Transform playerObj;
    public Transform camObj;
    public CinemachineVirtualCamera cineCam;
    public Rigidbody rb;


    private bool isGrounded;
    public float xRotation = 0f;
    public bool running;

    float mouseX;
    float mouseY;
    float storedMoveSpeed;
    public LayerMask groundLayerMask;

    public float mouseSpeedVariable;
    public static float mouseSensMult = 1;
    public float addedVertRot;

    public PlayerWallCheck playerWallCheck;
    public PlayerMantleCheck playerMantleCheck;

    public bool stopMovement;
    public bool stopRotation;
    Vector3 mantleStart;
    Vector3 mantleEnd;
    float mantleStartTime;
    float mantleLength;
    bool mantleOnce;
    float mantleCooldown;
    public float mantleCooldownMax = 1;

    public Animator anim;

    public bool leftWallRun;
    public bool rightWallRun;
    public bool disableGravity;
    public int jumpCount;
    public int jumpCountMax = 0;
    public float mantleSpringTimer;
    public float mantleSpringTimerMax = 0.3f;
    public float mantleSpringForwardForce = 10;

    public float inputTimeExtension = 0.2f;
    float crouchTimer;
    float jumpTimer;
    public float minSpeedStrafeEnable;
    public CapsuleCollider capsuleCollider;
    public CapsuleCollider wallHullColl;
    public float storedCapCollHeight = 2;
    public float storedWallHullHeight = 1.83f;
    bool crouching;
    float slideBoostTime;
    public float slideBoostTimeMax = 1;
    public float slideBoostStrength = 10;
    bool slideBoostOnce;

    float lungeCoolDown;
    public float lungeCoolDownMax = 3;
    public float lungeForce = 20;
    

    bool diveOnce = true;
    float diveCancelTime;
    public float diveCancelTimeMax = 0.1f;
    bool cancelDive;
    bool jumping;
    float stopGroundGravityTimer;

    float groundedMagnetismDistance;
    float groundMagnetismCheckDist = 1.5f;
    bool groundedOnce;

    public Slider lungeSlider;

    Vector3 lowerPlayerTransform;

    float previousYVel;

    [SerializeField]
    Slider mouseSlider;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        storedMoveSpeed = moveSpeed;
        lungeCoolDown = lungeCoolDownMax;
        storedGravityForce = gravityForce;
        //ChangeMouseSens();
    }

    void Update()
    {
        Jump();
        GroundCheck();
        Mantling();
        RunCheck();
        InputExtensions();
        //FOVSpeed(); //Disabled bc it's misbehaving
        //MantleSpring();
        //Dive();
        CrouchNSlide();
        //Lunge();
        //Wallrun();
    }

    void LateUpdate()
    {
        LookAround();
    }

    void FixedUpdate()
    {
        Move();
        Gravity();
    }

    void InputExtensions()
    {
        if(Input.GetKeyDown(KeyCode.Mouse4) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouchTimer = inputTimeExtension;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            jumpTimer = inputTimeExtension;
        }

        if(crouchTimer > 0)
        {
            crouchTimer -= Time.deltaTime;
        }
        if(jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
    }

    void Gravity()
    {
        // Apply custom gravity
        if (!isGrounded) //If in mid air
        {
            if (rb.velocity.y > -maxFallSpeed && !disableGravity)
            {
                rb.AddForce(transform.up * -gravityForce * rb.mass);
            }
        }
        else
        {
            if(!jumping) //Tried putting this in Update() for smoothness, transform y doesn't update right or seomthing
            {
                if (Physics.SphereCast(transform.position, 0.2f, Vector3.down, out RaycastHit hit, groundMagnetismCheckDist, groundLayerMask))
                {
                    if(transform.position.y - previousYVel <= 0) //If The player's velocity y is not changing, ie. going up a slope then set vel to 0 to not store y vel.
                    {
                        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    }
                    previousYVel = transform.position.y;

                    transform.position = new Vector3(transform.position.x, hit.point.y + groundedMagnetismDistance, transform.position.z);
                    //rb.AddForce(-hit.normal * groundedGravityMult);
                }
            }
            
            // Physics.CheckSphere(lowerPlayerTransform, 0.45f, groundLayerMask);
            // if (rb.velocity.y > -maxFallSpeed * 2f && !disableGravity)
            // {
            //     hit.normal
            //     rb.AddForce(transform.up * -gravityForce * rb.mass * 2f);
            // }
        }
    }

    void RunCheck()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            running = true;
        }


        if(running && isGrounded && !crouching)
        {
            anim.SetBool("running", true);
            //maxMoveSpeed = maxRunSpeed;
            moveSpeed = runningMoveSpeed;
        }
        else
        {
            anim.SetBool("running", false);
            //maxMoveSpeed = maxWalkSpeed;
            moveSpeed = walkMoveSpeed;
        }
    }

    void Move()
    {
        if(!stopMovement)
        {
            float moveHorizontal = Input.GetAxisRaw("Horizontal");
            float moveVertical = Input.GetAxisRaw("Vertical");



            Vector3 onlyXZVector = rb.velocity;

            Vector3 rbVelYZero = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z);
            rbVelYZero = transform.InverseTransformDirection(rbVelYZero);

            Vector3 movementDir = new Vector3(moveHorizontal, 0.0f, moveVertical);

            if(isGrounded)
            {
                if(!crouching)
                {
                    //rb.drag = 5;
                    onlyXZVector.x = onlyXZVector.x * deccelerationRate;
                    onlyXZVector.z = onlyXZVector.z * deccelerationRate;
                    //if (onlyXZVector.magnitude < maxMoveSpeed) //This is disabled bc it causes stuttering movement, probably bc it is in fixedupdate
                    {
                        rb.AddRelativeForce(movementDir.normalized * moveSpeed);
                    }
                }
                else
                {
                    onlyXZVector.x = onlyXZVector.x * slideDeccelerationRate;
                    onlyXZVector.z = onlyXZVector.z * slideDeccelerationRate;
                    rb.AddRelativeForce(movementDir.normalized * moveSpeed / 20);

                    if(!slideBoostOnce && rbVelYZero.magnitude < maxMoveSpeed * 2)
                    {
                        if(slideBoostTime <= 0)
                        {
                            rb.AddForce(rb.velocity * slideBoostStrength, ForceMode.Impulse);
                            slideBoostTime = slideBoostTimeMax;
                        }
                        slideBoostOnce = true;
                    }
                }
            }
            else
            {
                //rb.drag = 5;
                onlyXZVector.x = onlyXZVector.x * airDeccelerationRate; //Make it so that air decceleration is weaker
                onlyXZVector.z = onlyXZVector.z * airDeccelerationRate; //Make it so that air decceleration is weaker

                float dirVelDiff = 1 - Vector3.Dot(movementDir.normalized, rbVelYZero.normalized);

                //if(rb.velocity.magnitude > maxMoveSpeed && movementDir.magnitude > 0)
                {
                    dirVelDiff = Mathf.Clamp(dirVelDiff, 0.4f, 1f); //If moving in vel direction make min 0.01 not 0 so that player can counteract the air decceleration
                    rb.AddRelativeForce(movementDir.normalized * dirVelDiff * airMoveSpeed);
                    //Debug.Log(dirVelDiff);
                }

                //if(rb.velocity.magnitude < maxMoveSpeed && movementDir.magnitude > 0)
                //{
                //    if(movementDir.x != 0)
                //    {
                //        rb.AddRelativeForce(movementDir.normalized * airMoveSpeed);
                //    }
                //}


                //Vector3 projVel = Vector3.Project(rb.velocity, movementDir);
                //bool isAway = Vector3.Dot(movementDir, projVel) <= 0f;
                //if(isAway)
                //{
                //    rb.AddRelativeForce(movementDir.normalized * 10 * moveSpeed);
                //}
                //moveHorizontal /= 100;
                //moveVertical /= 100;
            }

            rb.velocity = onlyXZVector;

            

            if(movementDir.magnitude <= 0)
            {
                running = false;
            }
        }
    }

    void LookAround()
    {
        if(!stopRotation)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSens * mouseSensMult * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSens * mouseSensMult * Time.deltaTime;

            xRotation -= mouseY;
            //xRotation = -mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            ///////camObj.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            ///////camObj.Rotate(Vector3.right * xRotation * mouseSpeedVariable * Time.deltaTime);
            ///////camObj.localRotation = Quaternion.Euler(xRotation - addedVertRot, 0f, 0f);

            ///////camObj.Rotate(Vector3.right * (xRotation - addedVertRot));
            
            ////float clampXRot = camObj.localRotation.x;
            ////clampXRot = Mathf.Clamp(clampXRot, -90f, 90f);

            ///////camObj.localRotation = Quaternion.Euler(clampXRot, 0, 0);
            ///////camObj.Rotate(Vector3.right * xRotation);
            ///
            camObj.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            playerObj.Rotate(Vector3.up * mouseX);
        }
    }

    void Jump()
    {
        if(Input.GetButtonDown("Jump"))
        {
            if(jumpCount < jumpCountMax) //prob don't need isGrounded
            {
                if(!rightWallRun && !leftWallRun)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
            //else if(jumpCount < jumpCountMax)
            //{
            //    rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            //    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //}
            

            jumpCount++;

            if(rightWallRun)
            {
                jumpCount = 0;
                rb.AddForce(-transform.right * wallJumpForce / 1.3f, ForceMode.Impulse);
                rb.AddForce(Vector3.up * wallJumpForce, ForceMode.Impulse);
                rb.AddForce(transform.forward * wallJumpForce / 1.3f, ForceMode.Impulse);
            }

            if(leftWallRun)
            {
                jumpCount = 0;
                rb.AddForce(transform.right * wallJumpForce / 1.3f, ForceMode.Impulse);
                rb.AddForce(Vector3.up * wallJumpForce, ForceMode.Impulse);
                rb.AddForce(transform.forward * wallJumpForce / 1.3f, ForceMode.Impulse);
            }

            jumping = true;
            stopGroundGravityTimer = 0.3f;
        }
    }

    void GroundCheck()
    {
        lowerPlayerTransform = transform.position;
        lowerPlayerTransform.y = lowerPlayerTransform.y - groundedOffset;
        isGrounded = Physics.CheckSphere(lowerPlayerTransform, 0.45f, groundLayerMask);
        if(isGrounded)
        {
            if(stopGroundGravityTimer <= 0)
            {
                jumping = false;
            }
            jumpCount = 0;

            if(!groundedOnce)
            {
                if(rb.velocity.y <= -2) //Only decelerate if dropping fast
                {
                    rb.velocity = rb.velocity / 1.25f; //Decelerate on landing, maybe make decelerate amount dependent on falling speed
                }
                groundedOnce = true;                
            }
        }
        else
        {
            groundedOnce = false;
        }

        if(stopGroundGravityTimer > 0)
        {
            stopGroundGravityTimer -= Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 lowerPlayerTransformTest = transform.position;
        lowerPlayerTransformTest.y = lowerPlayerTransformTest.y - groundedOffset;

        Gizmos.DrawSphere(lowerPlayerTransformTest, 0.45f);
    }

    void Mantling()
    {
        if(mantleCooldown > 0)
        {
            mantleCooldown -= Time.deltaTime;
        }
        else
        {
            if(!isGrounded && playerWallCheck.lookingAtWall && playerMantleCheck.mantleAreaClear && Input.GetAxisRaw("Vertical") > 0)
            {
                if(!mantleOnce)
                {
                    MantleStart();
                }
            }
        }
        

        if(mantleOnce)
        {
            if(transform.position != mantleEnd && Input.GetAxisRaw("Vertical") > 0)
            {
                // Distance moved equals elapsed time times speed..
                float distCovered = (Time.time - mantleStartTime) * 5;
        
                // Fraction of journey completed equals current distance divided by total distance.
                float fractionOfJourney = distCovered / mantleLength;
        
                // Set our position as a fraction of the distance between the markers.
                transform.localPosition = Vector3.Lerp(mantleStart, mantleEnd, fractionOfJourney);
            }
            else
            {
                anim.Play("Normal"); //Cancel mantle anim
                stopMovement = false;
                stopRotation = false;
                mantleOnce = false;
            }
        }
    }

    void MantleStart()
    {
        anim.Play("Mantle");
        mantleCooldown = mantleCooldownMax;
        stopMovement = true;
        stopRotation = true;
        mantleStart = transform.localPosition;

        //mantleEnd = transform.localPosition + transform.InverseTransformPoint(0, 1.88f, 1);
        //Debug.Log(transform.InverseTransformPoint(0, 1.88f, 1));
        //mantleEnd = transform.position + transform.InverseTransformPoint(new Vector3(0, 1.88f, 1));
        mantleEnd = transform.position + (transform.up * 1.4f);
        mantleEnd = mantleEnd + (transform.forward * 1);

        mantleStartTime = Time.time;
        mantleLength = Vector3.Distance(mantleStart, mantleEnd);
        mantleOnce = true;
    }

    public void WallrunStart()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
    }

    public void WallrunLeft()
    {
        if(leftWallRun && !isGrounded)
        {
            if(!isGrounded)
            {
                gravityForce = 0.4f;
                //rb.AddForce(transform.forward * 1, ForceMode.Impulse);
                anim.SetBool("wallrunLeft", true);
            }
        }
        else
        {
            anim.SetBool("wallrunLeft", false);
        }

        if(!leftWallRun && !rightWallRun)
        {
            DisableWallrun();
        }
    }

    public void WallrunRight()
    {
        if(rightWallRun && !isGrounded)
        {
            if(!isGrounded)
            {
                gravityForce = 0.4f;
                //rb.AddForce(transform.forward * 1, ForceMode.Impulse);
                anim.SetBool("wallrunRight", true);
            }
        }
        else
        {
            anim.SetBool("wallrunRight", false);
        }

        if(!leftWallRun && !rightWallRun)
        {
            DisableWallrun();
        }
    }

    public void WallRunForce(Vector3 wallNormal)
    {
        if(!isGrounded)
        {
            rb.AddForce(wallNormal * 8); //negative bc normal points OUT; changed to pos bc closest point doesn't work on mesh coll. Using transform.right for now
        }
    }

    void DisableWallrun()
    {
        disableGravity = false;
        anim.SetBool("wallrunRight", false);
        anim.SetBool("wallrunLeft", false);
        gravityForce = storedGravityForce;
    }

    public void MantleSpringTimerStart()
    {
        mantleSpringTimer = mantleSpringTimerMax;
    }

    void MantleSpring()
    {
        if(mantleSpringTimer > 0)
        {
            mantleSpringTimer -= Time.deltaTime;
            if(jumpTimer > 0 && crouchTimer > 0)
            {
                Debug.Log("hit");
                jumpCount = 0;
                rb.AddForce(transform.forward * mantleSpringForwardForce, ForceMode.Impulse); //Is this fine in Update?
                rb.AddForce(transform.up * 1, ForceMode.Impulse); //Is this fine in Update?
                mantleSpringTimer = 0;
            }
        }
    }

    void FOVSpeed()
    {
        //if(rb.velocity.magnitude > 8)
        {
            Vector3 onlyXZVector2 = rb.velocity;
            onlyXZVector2.y = 0; //Only get the xz not y for speed fov
            float fovTarget = 62 + onlyXZVector2.magnitude * 1.15f;
            //Debug.Log(onlyXZVector2);
            
            //if(fovTarget > 70 && fovTarget < 150)
            {
                if(cineCam.m_Lens.FieldOfView < fovTarget)
                {
                    cineCam.m_Lens.FieldOfView += Time.deltaTime * 40;
                }
                else
                {
                    cineCam.m_Lens.FieldOfView -= Time.deltaTime * 40;
                }
            }
            //cineCam.m_Lens.FieldOfView = 66 + rb.velocity.magnitude / 2; //Since fov 70 is default, make base 60 and rb.vel is 10 minimum //I've changed it so not too releveant
            cineCam.m_Lens.FieldOfView = Mathf.Clamp(cineCam.m_Lens.FieldOfView, 70, 150);
        }
    }

    void Dive()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            diveCancelTime = diveCancelTimeMax;
        }

        if(diveCancelTime > 0)
        {
            diveCancelTime -= Time.deltaTime;
        }

        if(!isGrounded)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                if(diveCancelTime < 0)
                {
                    rb.velocity = new Vector3(0,0,0);
                    cancelDive = false;
                    //diveOnce = false;
                    //diveCancelTime = diveCancelTimeMax;
                }
                else
                {
                    cancelDive = true;
                }
                anim.Play("Dive");
            }
        }
        else
        {
            anim.SetBool("dive", false);
        }

        //if(diveCancelTime > 0)
        //{
        //    diveCancelTime -= Time.deltaTime;
        //    if(Input.GetKeyDown(KeyCode.Space))
        //    {
        //        cancelDive = true;
        //        diveOnce = true;
        //    }
        //}
        //else if(!cancelDive && !diveOnce)
        //{
        //    rb.velocity = new Vector3(0,0,0);
        //    anim.Play("Dive");
        //    //anim.SetTrigger("diveStart");
        //    anim.SetBool("dive", true);
        //    diveOnce = true;
        //}
        

        
        if(anim.GetCurrentAnimatorStateInfo(2).IsName("Diving"))
        {
            stopMovement = true;
            if(isGrounded)
            {
                anim.SetBool("dive", false);
            }
        }
        else
        {
            stopMovement = false;
        }
    }

    public void DiveDown() //Called by anim
    {
        if(!cancelDive)
        {
            rb.AddForce(-transform.up * 20, ForceMode.Impulse);
        }
    }

    void CrouchNSlide()
    {
        if(Input.GetKey(KeyCode.Mouse4) || Input.GetKey(KeyCode.LeftControl)) //Crouching
        {
            crouching = true;
            wallHullColl.height = storedWallHullHeight/2;
            capsuleCollider.height = storedCapCollHeight/2;
            groundedOffset = storedGroundedOffset/2;
            groundedMagnetismDistance = 0.4f;
            groundMagnetismCheckDist = 0.3f; //Disable for now
        }
        else //Standing
        {
            crouching = false;
            wallHullColl.height = storedWallHullHeight;
            capsuleCollider.height = storedCapCollHeight;
            groundedOffset = storedGroundedOffset;
            groundedMagnetismDistance = 1.05f;
            groundMagnetismCheckDist = 1.5f;
        }


        if(Input.GetKeyDown(KeyCode.Mouse4) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            slideBoostOnce = false;
            //if(slideBoostTime > 0)
            //{
            //    rb.AddRelativeForce(transform.forward * slideBoostStrength, ForceMode.Impulse);
            //    slideBoostTime = slideBoostTimeMax;
            //}
        }

        //SLIDEING WHEN GROUNDED IS HANDLED IN MOVEMENT()
        if(slideBoostTime > 0)
        {
            slideBoostTime -= Time.deltaTime;
        }
    }

    // void Lunge()
    // {
    //     lungeSlider.value = lungeCoolDown / lungeCoolDownMax;
    //     if(lungeCoolDown >= lungeCoolDownMax)
    //     {
    //         if(Input.GetMouseButtonDown(1) && isGrounded)
    //         {
    //             anim.SetTrigger("lunge");
    //             rb.AddForce(transform.forward * lungeForce, ForceMode.Impulse);
    //             lungeCoolDown = 0;
    //         }
    //     }
    //     else
    //     {
    //         lungeCoolDown += Time.deltaTime;
    //     }
    // }

    public void LungeEnd() //CALLED BY ANIM
    {
        rb.velocity = transform.forward * 3; //Not zero vel to make it smooth stop
    }

    //Notes on skating: The lunge animation is played but at the end velocity is stopped. Dive animation cancels that anmiation and allows velocity to continue
    //I needed to hardcode the jump cancelling the dive however to allow for this since dive also stops all velocity





    public void AnimationStopMovement()
    {
        stopMovement = true;
    }

    public void AnimationStartMovement()
    {
        stopMovement = false;
    }

    public void ChangeMouseSens()
    {
        mouseSensMult = mouseSlider.value;
    }
}