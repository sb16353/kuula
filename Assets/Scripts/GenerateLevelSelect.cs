using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro;
using System.Linq;

public class GenerateLevelSelect : MonoBehaviour
{

    [SerializeField]
    GameObject startButton;
    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    Transform buttonContainer;
    public float columnSpacing = 450f;
    public float rowSpacing = 150f;
    public float startY = -125f;    
    public int maxRows = 3;

    public void Generate()
    {
        if (startButton != null)
        {
            startButton.SetActive(false);
        }

        // Filter valid level sets
        var validSets = GameManager.Instance.levelSetData.levelSets
            .Where(set => set.levelSceneNames.Count > 0)
            .ToList();

        int totalButtons = validSets.Count;
        int actualColumns = Mathf.CeilToInt((float)totalButtons / maxRows);

        for (int index = 0; index < totalButtons; index++)
        {
            var set = validSets[index];
            string firstLevel = set.levelSceneNames[0];

            var button = Instantiate(buttonPrefab, buttonContainer);

            var tmpText = button.GetComponentInChildren<TMP_Text>();
            if (tmpText != null) {
                tmpText.text = set.name;
            }

            // Column-first layout: fill columns before rows
            int col = index / maxRows;
            int row = index % maxRows;

            // Center based on actual number of columns
            int middleCol = actualColumns / 2;
            float xOffset = (col - middleCol) * columnSpacing;

            if (actualColumns % 2 == 0) {
                xOffset += columnSpacing / 2f;
            }

            float y = startY - row * rowSpacing;

            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(xOffset, y);

            var buttonMono = button.GetComponent<Button>();
            buttonMono.onClick.AddListener(() =>
            {
                GameManager.Instance.DisplayLoading();
                SceneManager.LoadSceneAsync(firstLevel, LoadSceneMode.Single);
            });
        }
    }



}
