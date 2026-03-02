using UnityEngine;
using UnityEngine.UI;

public class TimerToggle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(delegate
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ToggleTimer(toggle);
        });
        enabled = false;
    }

}
