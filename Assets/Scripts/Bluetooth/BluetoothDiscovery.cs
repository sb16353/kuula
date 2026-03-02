    using UnityEngine;
using TechTweaking.Bluetooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Localization;

public class BluetoothDiscovery : MonoBehaviour
{
    public static BluetoothDiscovery Instance { get; private set; }

    private readonly Dictionary<string, BluetoothDevice> discoveredDevices = new();
    public IReadOnlyDictionary<string, BluetoothDevice> DiscoveredDevices => discoveredDevices;

    private Coroutine discoveryCoroutine;
    private Coroutine connectCoroutine;

    public BluetoothDevice ConnectedDevice { get; private set; }



    private void Awake()
    {/*
        if (!Application.isEditor && Application.platform != RuntimePlatform.Android)
        {
            LoadMainMenu();
            return;
        }
*/

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        BluetoothAdapter.OnDeviceDiscovered += HandleOnDeviceDiscovered;
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            BluetoothAdapter.OnDeviceDiscovered -= HandleOnDeviceDiscovered;
        }
    }

    private void HandleOnDeviceDiscovered(BluetoothDevice device, short rssi)
    {
        if (!string.IsNullOrEmpty(device.Name) && !string.IsNullOrEmpty(device.MacAddress) && !discoveredDevices.ContainsKey(device.MacAddress))
        {
            discoveredDevices.Add(device.MacAddress, device);
            if(BluetoothDiscoveryUI.Instance != null)
                BluetoothDiscoveryUI.Instance.AddDeviceButton(device, rssi);
        }
    }

    public bool IsSearching => discoveryCoroutine != null;
    public bool IsConnecting => connectCoroutine != null;

    public void StartDiscovery()
    {
        StopDiscovery();

        discoveredDevices.Clear();
        BluetoothDiscoveryUI.Instance.RemoveAllButtons();

        discoveryCoroutine = StartCoroutine(DiscoveryCoroutine());
    }

    public void StopDiscovery()
    {
        if (discoveryCoroutine != null)
            StopCoroutine(discoveryCoroutine);

        discoveryCoroutine = null;
    }

    private IEnumerator DiscoveryCoroutine()
    {
        BluetoothAdapter.askEnableBluetooth();

        int t = 0;

        var delay = new WaitForSeconds(0.5f);

        while (!BluetoothAdapter.isBluetoothEnabled() && t++ < maxAttemptTimeSeconds * 2)
            yield return delay;
        
        if(BluetoothAdapter.refreshDiscovery())
            yield return new WaitForSecondsRealtime(10f);

        discoveryCoroutine = null;
    }

    [SerializeField] LocalizedString textConnecting;
    [SerializeField] LocalizedString textConnectionSuccess;
    [SerializeField] LocalizedString textConnectionFail;

    public void AttemptToConnect(BluetoothDevice _device)
    {
        if (_device == null || string.IsNullOrEmpty(_device.MacAddress))
            return;

        if (connectCoroutine != null)
            StopCoroutine(connectCoroutine);

        connectCoroutine = StartCoroutine(ConnectCoroutine(_device));

        if (BluetoothDiscoveryUI.Instance != null)
            BluetoothDiscoveryUI.Instance.ShowConnectingUI(true, textConnecting.GetLocalizedString());
    }

    public int maxAttemptTimeSeconds = 15;

    private IEnumerator ConnectCoroutine(BluetoothDevice _device)
    {
        if (_device != null)
        {
            yield return new WaitForSecondsRealtime(0.5f);

            _device.connect();
            //yritä yhdistää maxAttemptTimeSeconds:in verran sekunteja
            int t = maxAttemptTimeSeconds;

            var delay = new WaitForSecondsRealtime(1.0f);

            bool connected;
            do
            {
                yield return delay;
                connected = _device.IsConnected;
            }
            while (!connected && --t > 0);

            if (connected)
            {
                if (ConnectedDevice != null) {
                    ConnectedDevice.close();
                    ConnectedDevice.Dispose();
                }
                ConnectedDevice = _device;
            }
                

            BluetoothDiscoveryUI.Instance.ShowConnectingUI(true,  connected ? textConnectionSuccess.GetLocalizedString() : textConnectionFail.GetLocalizedString());

            t = 2;
            while (t-- > 0) 
                yield return delay;
            
            if (BluetoothDiscoveryUI.Instance != null)
                BluetoothDiscoveryUI.Instance.ShowConnectingUI(false);
        }

        connectCoroutine = null;
    }
}
