using UnityEngine;
using System.Text;

public class BluetoothSensorReader : MonoBehaviour
{
    public const int SENSOR_VALUE_COUNT = 9;

    public readonly int[] sensorRawValues = new int[SENSOR_VALUE_COUNT];
    public readonly float[] sensorMappedValues = new float[SENSOR_VALUE_COUNT];

    // UUSI: Interpoloidut arvot
    public readonly float[] sensorInterpolatedValues = new float[SENSOR_VALUE_COUNT];

    const float MAPPED_VALUE_MULTIPLIER = (float)byte.MaxValue; // = 255
    const float MAPPED_VALUE_DIVIDER = 99.0f;   
    public float interpolationFactor = 6.0f;

    void OnDisable()
    {
        ResetValues();
    }

    void ResetValues() {
        for (int i = 0; i < SENSOR_VALUE_COUNT; ++i) {
            sensorRawValues[i] = 0;
            sensorMappedValues[i] = 0.0f;    
            sensorInterpolatedValues[i] = 0.0f;
        }
    }

    void Update()
    {
        if (BluetoothDiscovery.Instance != null)
        {
            var device = BluetoothDiscovery.Instance.ConnectedDevice;
            if (device != null)
            {
                if (device.IsReading)
                {
                    byte[] array = device.read();

                    if (array != null)
                    {
                        string[] valueArray = Encoding.ASCII.GetString(array).Split(',');

                        int parsedValue;

                        for (int i = 0; i < valueArray.Length && i < SENSOR_VALUE_COUNT; ++i)
                        {
                            parsedValue = int.Parse(valueArray[i]);

                            sensorRawValues[i] = parsedValue;
                            sensorMappedValues[i] = (float)parsedValue * MAPPED_VALUE_MULTIPLIER / MAPPED_VALUE_DIVIDER;

                            // Interpoloidaan arvo nykyiseen interpolated-taulukkoon
                            sensorInterpolatedValues[i] = Mathf.Lerp(
                                sensorInterpolatedValues[i],
                                sensorMappedValues[i],
                                interpolationFactor * Time.deltaTime
                            );
                        }
                    }
                }
                else if (!device.IsConnected)
                {
                    ResetValues();
                }
            }
        }
    }
}
