using UnityEngine;
using System.Collections;

public class Elevator : MonoBehaviour
{
    public Vector2[] path = new Vector2[0];
    
    void OnEnable(){
        StopAllCoroutines();
        StartCoroutine(MoveLoop());
    }
    void OnDisable()
        => StopAllCoroutines();

    public float moveSpeed = 1.0f;

    Rigidbody2D rb;
    void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }

    private IEnumerator MoveLoop(){
        Vector2 target, current;
        while(true){
            rb.position = path[0];
            for(int i = 1; i < path.Length; ++i){
                while(true){
                    target = path[i];
                    current = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.deltaTime);
                    rb.MovePosition(current);
                    yield return GameManager.FixedUpdateDelay;
                    if(Mathf.Approximately((current - target).sqrMagnitude, 0.0f))
                        break;
                }             
            }        
        }

    }
}
