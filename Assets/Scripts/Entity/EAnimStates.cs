using UnityEngine;
using System.Collections;

public class EAnimStates : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private EntityStates currentState;
    public AudioSource audioSource;
    public AudioSource audioSource2;

    public void SetState(EntityStates newState)
    {
        if (currentState == newState)
            return;

        currentState = newState;

        UpdateAnimation();
    }

    public void UpdateAnimation()
    {

        switch (currentState)
        {
            case EntityStates.idle:
                animator.CrossFade("Creep|Idle1_Action", 0.2f);
                break;

            case EntityStates.walking:
                animator.CrossFade("Creep|Walk1_Action", 0.2f);
                AudioManager.Instance.Play("Footsteps", audioSource2);
                break;

            case EntityStates.investigate:
                animator.CrossFade("Creep|Crouch_Action", 0.2f);
                AudioManager.Instance.Play("Sniffing", audioSource);
                break;

            case EntityStates.running:
                animator.CrossFade("Creep|Walk2_Action", 0.2f);
                AudioManager.Instance.Play("Heartbeat", audioSource);
                break;

            case EntityStates.sniffing:
                animator.CrossFade("Creep|Sniff_Action", 0.2f);
                AudioManager.Instance.Play("Sniffing", audioSource);
                break;

            case EntityStates.jumpscare:
                animator.CrossFade("Creep|Bite_Action", 0.1f);
                AudioManager.Instance.StopAll();
                AudioManager.Instance.Play("Jumpscare", audioSource);
                break;
        }
    }
}

    public enum EntityStates
    {
        idle,
        walking,
        running,
        investigate,
        sniffing,
        jumpscare  // opening hiding place states ? 
    }

