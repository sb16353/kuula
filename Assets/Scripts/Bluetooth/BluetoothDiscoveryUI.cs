using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using TechTweaking.Bluetooth;


public class BluetoothDiscoveryUI : MonoBehaviour
{
    public static BluetoothDiscoveryUI Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private Button deviceButtonPrefab;
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private GameObject connectingGraphic;
    [SerializeField] private TMP_Text connectingText; // Add this reference in the Inspector
    [SerializeField] private GameObject searchButton;
    [SerializeField] private Transform searchIcon;
    [SerializeField] private float searchIconRotationSpeed = 150f;
    private readonly Dictionary<string, Button> deviceButtons = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }

        Instance = this;
    }


    private void Update()
    {
        bool isSearching = BluetoothDiscovery.Instance != null && BluetoothDiscovery.Instance.IsSearching;
        searchButton.SetActive(!isSearching);
        searchIcon.gameObject.SetActive(isSearching);

        if (isSearching)
            searchIcon.Rotate(Vector3.forward, searchIconRotationSpeed * Time.deltaTime);

    }

    /// <summary>
    /// Controls visibility of UI and optionally sets connecting status text.
    /// </summary>
    /// <param name="isConnecting">True to show connecting UI, false to show device list.</param>
    /// <param name="statusText">Optional text to show while connecting.</param>
    public void ShowConnectingUI(bool isConnecting, string statusText = null)
    {
        buttonParent.gameObject.SetActive(!isConnecting);
        connectingGraphic.SetActive(isConnecting);

        if (connectingText != null && statusText != null) {
            connectingText.text = statusText;
        }
    }


    public void RemoveButton(BluetoothDevice device)
        => RemoveButton(device.MacAddress);
    
    public void RemoveButton(string _macAddress)
    {
        if (deviceButtons.ContainsKey(_macAddress))
            return;

        var gameObject = deviceButtons[_macAddress].gameObject;
        deviceButtons.Remove(_macAddress);
        Destroy(gameObject);
    }

    public void RemoveAllButtons()
    {
        deviceButtons.Clear();
        for (int i = buttonParent.childCount - 1; i >= 0; i--)
        {
            Destroy(buttonParent.GetChild(i).gameObject);
        }
    }

    public void AddDeviceButton(BluetoothDevice device, short rssi)
    {
        if (deviceButtons.ContainsKey(device.MacAddress)) return;

        GameObject buttonGO = Instantiate(deviceButtonPrefab.gameObject, buttonParent);
        Button btn = buttonGO.GetComponent<Button>();
        btn.name = device.MacAddress;

        RectTransform rectTransform = (RectTransform)buttonGO.transform;
        float buttonHeight = rectTransform.sizeDelta.y;
        rectTransform.anchoredPosition = new Vector3(0.0f, -(buttonHeight / 2.0f) - (buttonHeight * (buttonParent.childCount - 1)), 0.0f);
        
        var listSize = buttonParent.sizeDelta;
        listSize.y += buttonHeight;
        buttonParent.sizeDelta = listSize;

        TMP_Text text = buttonGO.GetComponentInChildren<TMP_Text>();
        text.text = device.Name;

        btn.onClick.AddListener(() =>
        {
            if (BluetoothDiscovery.Instance != null)
                BluetoothDiscovery.Instance.AttemptToConnect(device);
        });
        deviceButtons[device.MacAddress] = btn;
    }

    public void OnSearchPressed()
    {
        if (BluetoothDiscovery.Instance != null)
            BluetoothDiscovery.Instance.StartDiscovery();
    }
}
