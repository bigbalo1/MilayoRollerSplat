using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 15;

    bool isTraveling;
    Vector3 travelDirection;
    Vector3 nextCollisionPosition;

    public int minSwipeRecognition = 500;
    Vector2 swipePoseLasFrame;
    Vector2 swipePoseCurrentFrame;
    Vector2 currentSwipe;

    private AudioSource playerAudio;
    public AudioClip swiptSound;
    public ParticleSystem finishExposure;

    Color solveColor;

    private void Start()
    {
        solveColor = Random.ColorHSV(0.5f, 1);
        GetComponent<MeshRenderer>().material.color = solveColor;
        playerAudio = GetComponent<AudioSource>();
    }


    private void FixedUpdate()
    {
        if (isTraveling)
        {
            rb.velocity = speed * travelDirection;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position - (Vector3.up / 2), 0.05f);
        int i = 0;
        while(i < hitColliders.Length)
        {
            GroundPiace ground = hitColliders[i].transform.GetComponent<GroundPiace>();
            if(ground && !ground.isColored)
            {
                ground.ChangeColor(solveColor);
            }
            i++;
        }

        if(nextCollisionPosition != Vector3.zero)
        {
            if(Vector3.Distance(transform.position, nextCollisionPosition) < 1)
            {
                isTraveling = false;
                travelDirection = Vector3.zero;
                nextCollisionPosition = Vector3.zero;
            }
        }

        if (isTraveling)
            return;

        if (Input.GetMouseButton(0))
        {
            swipePoseCurrentFrame = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if(swipePoseCurrentFrame != Vector2.zero)
            {
                currentSwipe = swipePoseCurrentFrame - swipePoseLasFrame;

                if(currentSwipe.sqrMagnitude < minSwipeRecognition)
                {
                    return;
                }

                currentSwipe.Normalize();

                // UP /DOWN
                if(currentSwipe.x > -0.5f && currentSwipe.x < 0.5f)
                {
                    // GO UP/DOWN
                    SetDestination(currentSwipe.y > 0 ? Vector3.forward : Vector3.back);
                }

                //LEFT / RIGHT
                if (currentSwipe.y > -0.5f && currentSwipe.y < 0.5f)
                {
                    // GO LEFT/RIGHT
                    SetDestination(currentSwipe.x > 0 ? Vector3.right : Vector3.left);
                }
                playerAudio.PlayOneShot(swiptSound, 1.0f);
                finishExposure.Play();
            }

            swipePoseLasFrame = swipePoseCurrentFrame;
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            swipePoseLasFrame = Vector2.zero;
            currentSwipe = Vector2.zero;
        }
        
    }

    void SetDestination(Vector3 direction)
    {
        travelDirection = direction;

        RaycastHit hit;
        if(Physics.Raycast(transform.position, direction, out hit, 100f))
        {
            nextCollisionPosition = hit.point;
        }

        isTraveling = true;
    }

}
