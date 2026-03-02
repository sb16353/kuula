using UnityEngine;

public class GameOver : MonoBehaviour
{
    public void Retry()
        => GameManager.Instance.ReloadCurrentLevel();

    public void Quit()
        => GameManager.Instance.LoadMainMenu();


    void LateUpdate(){
        if(Input.GetKeyDown(KeyCode.Return))
            Retry();
    }
}
