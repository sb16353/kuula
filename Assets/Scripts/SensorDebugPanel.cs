using UnityEngine;
using TMPro;
public class SensorDebugPanel : MonoBehaviour
{
    [SerializeField] TMP_Text[] debugValues = new TMP_Text[BluetoothSensorReader.SENSOR_VALUE_COUNT];

    BluetoothSensorReader reader;

    public static SensorDebugPanel Instance
    {
        get => _instance;
    }
    static SensorDebugPanel _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        bool readerIsNull = reader == null;

        if (readerIsNull && GameManager.Instance != null && GameManager.Instance.Player != null) {
            reader = GameManager.Instance.Player.GetComponent<BluetoothSensorReader>();
            readerIsNull = reader == null;
        }

        if (!readerIsNull)
        {
            for (int i = 0; i < BluetoothSensorReader.SENSOR_VALUE_COUNT; ++i)
                debugValues[i].text = ((int)System.Math.Round(reader.sensorMappedValues[i])).ToString("D4");
        }
    }
}
