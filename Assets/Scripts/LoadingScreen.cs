using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using System;
public class LoadingScreen : MonoBehaviour
{
    
    [SerializeField]
    TMP_Text textMesh;
    [SerializeField] LocalizedString loadingText;
    void Start()
    {
        if(!onEnableCalled){
            OnEnable();
        }
    }
    bool onEnableCalled;
    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(LoadingTextLoop());
        onEnableCalled = true;
    }

    void OnDisable()
    {
        StopAllCoroutines();
        onEnableCalled = false;
    }

    public float changeDelay = (float)Fractions.OneThird;
    IEnumerator LoadingTextLoop(){
        int numOfPeriods = 0;
        WaitForSeconds delay = new(changeDelay);
        Func<string> loadText = () =>
        {
            string text = loadingText.GetLocalizedString() + new string('.', 1 + numOfPeriods++);
            numOfPeriods %= 3;
            return text;
        };
        string loadedText = loadText();
        while (true)
        {
            textMesh.text = loadedText;
            loadedText = loadText();
            yield return delay;
        }
    }
}
