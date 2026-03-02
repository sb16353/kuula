using UnityEngine;

public class Obstacle : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("moi");
        if(other.attachedRigidbody != null && GameManager.Instance.Player != null && other.attachedRigidbody.gameObject == GameManager.Instance.Player.gameObject) {

            GameManager.Instance.Player.Die();
            return;

            GameManager.Instance.ReloadCurrentLevel();
        }
    }
}
