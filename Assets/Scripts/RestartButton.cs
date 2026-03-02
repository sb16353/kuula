using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    int confirm;
    [SerializeField] Button button;
    [SerializeField] TMP_Text text;
    [SerializeField]
    string[] textStages = new string[]{
        "Aloita alusta",
        "Oletko varma?"
    };


    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();

        if (text == null)
            text = GetComponentInChildren<TMP_Text>();
    }

    public void Push()
    {
        if (++confirm >= textStages.Length) {
            confirm = 0;
            GameManager.Instance.ReloadCurrentLevel();
        }

        SetText();

        pushTime = Time.time;
    }

    float pushTime;

    const float RESET_TIME = 20.0f;

    void Update()
    {
        if (confirm != 0 && (Time.time - pushTime) > RESET_TIME) {
            confirm = 0;
            SetText();        
        }
    }

    void SetText()
    {
        if (text != null && textStages != null && textStages.Length > 0)
            text.text = textStages[confirm % textStages.Length];
    }
}
