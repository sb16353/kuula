using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class UIHelper : MonoBehaviour
{
        
    public void LoadMainMenu()
        => SceneManager.LoadScene("main");
    public void SetCurrentLocale(int localeIndex)
        => LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[Mathf.Clamp(localeIndex, 0, LocalizationSettings.AvailableLocales.Locales.Count - 1)];
}
