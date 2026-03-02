    using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Localization.PropertyVariants;
using System.Collections;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    TMP_Text levelSetName;
    [SerializeField]
    TMP_Text levelCompletionTimes;

    [SerializeField]
    int maxLines = 10; // max lines per column

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        levelSetName.GetComponent<GameObjectLocalizer>().enabled = false;
        levelSetName.text = string.Format(levelSetName.text, GameManager.Instance.CurrentLevelSetName);

        if(!GameManager.Instance.TimerEnabled)
            yield break;

        List<string> times = GameManager.Instance.levelCompletionTimes;
        int totalLevels = times.Count;

        // Split times into columns
        int numColumns = Mathf.CeilToInt((float)totalLevels / maxLines);
        List<string>[] columns = new List<string>[numColumns];

        for (int i = 0; i < numColumns; i++)
            columns[i] = new ();
        
        for (int i = 0; i < totalLevels; i++)
        {
            int colIndex = i / maxLines;
            columns[colIndex].Add($"Kenttä {i + 1}: {times[i]}");
        }

        // Create side-by-side columns
        StringBuilder sb = new();
        for (int row = 0; row < maxLines; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (row < columns[col].Count)
                {
                    sb.Append(columns[col][row].PadRight(25));
                }
            }
            sb.AppendLine();
        }

        levelCompletionTimes.text = sb.ToString();
    }

    void LateUpdate()
    {
        if(Input.touchCount > 0 || Input.GetKeyDown(KeyCode.Return))
            GameManager.Instance.LoadMainMenu();
    }
}
