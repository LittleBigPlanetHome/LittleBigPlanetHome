using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class ZomZomIdleIndex : MonoBehaviour
{
    private Animator anim;

    IEnumerator Start()
    {
        anim = GetComponent<Animator>();    
        while (true)
        { 
            yield return new WaitForSeconds(9);
            anim.SetTrigger("Idle");
            anim.SetInteger("IdleIndex", Random.Range(0, 5));
        }
    }
}
