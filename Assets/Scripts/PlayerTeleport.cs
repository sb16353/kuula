using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    public Vector2 destination;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.attachedRigidbody != null && collision.attachedRigidbody.gameObject == GameManager.Instance.Player.gameObject) {
            GameManager.Instance.Player.position = destination;
        }
    }
}
