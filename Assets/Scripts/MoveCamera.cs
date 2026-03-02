using UnityEngine;
using System.Collections;
public class MoveCamera : MonoBehaviour
{
    float lastInput;

    public Vector2 offset = new(4.0f, 2.0f);

    public bool restrictY = true;
    void Start(){
        OnEnable();
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(MoveCameraLoop());
    }

    void OnDisable()
        => StopAllCoroutines();

    // Update is called once per frame
    private IEnumerator MoveCameraLoop(){
        var player = GameManager.Instance.Player;
        while(true){
            if (player == null) yield break;

            //Debug.Log("moi");

            if (!Mathf.Approximately(player.PInput, 0.0f))
                lastInput = player.PInput;

            Vector3 playerPos = player.position;
            Vector3 cameraPos = transform.position;

            float targetX =  playerPos.x;
            if(!Mathf.Approximately(lastInput, 0.0f))
                targetX += lastInput < 0.0f ? offset.x : -offset.x;

            cameraPos.x = Mathf.Lerp(cameraPos.x, targetX, 6 * Time.deltaTime);

            if(!restrictY){
                float targetY = playerPos.y;
                if(player.MoveDir.y < -0.1f)
                    targetY -= offset.y;
                cameraPos.y = Mathf.Lerp(cameraPos.y, targetY, 6 * Time.deltaTime);
            }

            transform.position = cameraPos;
            
            yield return GameManager.FixedUpdateDelay; 
        }
    }

}