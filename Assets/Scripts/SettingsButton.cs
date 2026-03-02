using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var toggle = GetComponent<Button>();
        toggle.onClick.AddListener(delegate
        {
            if (GameManager.Instance != null)
                GameManager.Instance.LoadLevel("BluetoothDiscovery");
        });
        enabled = false;
    }

}
