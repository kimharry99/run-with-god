using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sickle : MonoBehaviour
{
    private Butcher butcher;
    private StateMachine stateMachine = new StateMachine();
    public bool isForAttack = false;

    private void Start()
    {
        butcher = transform.GetComponentInParent<Butcher>();
    }

    private void InitStateMachine()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isForAttack)
            {
                PlayerController.inst.GetDamaged();
            }
            else
            {

            }
        }
        else if (collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Ground")
        {
            
        }
    }
}
