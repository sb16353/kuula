using UnityEngine;
public class Goal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("moi");
       
        if(other.attachedRigidbody && other.attachedRigidbody.gameObject.layer == LayerMask.NameToLayer("Player")) {
            GameManager.Instance.StopTimer();
            GameManager.Instance.LoadNextLevel();
        }
    }
}
