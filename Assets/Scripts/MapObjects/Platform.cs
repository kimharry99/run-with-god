using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Collider2D col;
    private Vector3 pos;
    private double platformceil;
    private double playerbottom;
    protected void Start()
    {
        col = GetComponent<Collider2D>();
        pos = col.bounds.max;
        platformceil = pos.y;
    }

    protected void Update()
    {
        if(gameObject.layer == LayerMask.NameToLayer("Platform")&& PlayerController.inst.transform.position.y - 0.22> platformceil)
        { 
            gameObject.layer = LayerMask.NameToLayer("Ground");  
        }
        else
        {
            if(PlayerController.inst.transform.position.y -0.22 < platformceil)
            {
                gameObject.layer = LayerMask.NameToLayer("Platform");
            }
        }
    }
}