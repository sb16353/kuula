using UnityEngine;
using System.Collections;
public class MovingObstacle : Obstacle
{
    public Vector2[] path = new Vector2[0];

    void OnEnable(){
        StopAllCoroutines();
        StartCoroutine(MoveLoop());
    }
    void OnDisable()
        => StopAllCoroutines();

    public float moveSpeed = 1;

    private IEnumerator MoveLoop(){
        Vector2 target, current;
        while(true){
            for(int i = 0; i < path.Length; ++i){
                while(true){
                    target = path[i];
                    current = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
                    transform.position = current;
                    yield return null;
                    if(Mathf.Approximately((current - target).sqrMagnitude, 0.0f))
                        break;
                }
                
            }        
        }

    }
}
