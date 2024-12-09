using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponentInParent<PlayerController>().isJumped = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            GetComponentInParent<PlayerController>().isJumped = true;
        }
    }
}
