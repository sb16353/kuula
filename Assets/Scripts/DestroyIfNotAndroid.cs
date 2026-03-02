using UnityEngine;

public class DestroyIfNotAndroid : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            Destroy(gameObject);
            return;
        }

        Destroy(this);
    }


}
