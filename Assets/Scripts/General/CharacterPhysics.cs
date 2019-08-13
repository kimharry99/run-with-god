using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class CharacterPhysics : MonoBehaviour
{
    private Vector2 pos { get { return transform.position; } }
    private BoxCollider2D col = null;

    [SerializeField]
    private Vector2 velocity = Vector2.zero;

    private const float gravity = 9.8f;
    public float gravityScale = 1f;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(pos, pos + velocity / 10);
    }
#endif  

    public bool IsGround
    {
        get
        {
            float x = col.bounds.extents.x;
            float y = col.bounds.extents.y;
            return Physics2D.Linecast(pos + new Vector2(-x, -y), pos + new Vector2(x, -y),1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Ground Passable")).transform != null;
        }
    }

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y += 5.0f;
        }

        transform.Translate(velocity * Time.deltaTime);
        if (!IsGround)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }
    }
}
