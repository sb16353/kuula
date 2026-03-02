using UnityEngine;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "LevelSetData", menuName = "Game/Level Set Data")]
public class LevelSetData : ScriptableObject
{
    [System.Serializable]
    public class LevelSet
    {


        public string name
        {
            get
            {
                string localized = _name.GetLocalizedString();
                
                if (localized.Length < 1)
                    return new StringBuilder("Set ").Append(setNumber).ToString();

                return localized;            
            }
        }
        [SerializeField]
        private LocalizedString _name;
        public int setNumber;
        public List<string> levelSceneNames = new();
    }

    public List<LevelSet> levelSets = new();
}
