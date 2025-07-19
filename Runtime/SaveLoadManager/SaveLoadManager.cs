using UnityEngine;

public abstract class SaveLoadManager<T> : MonoBehaviour where T : SavableData, new()
{
    [SerializeField] string _dataKey;
    [SerializeField] private int _dataVersionNumber;
    [Space]
    [SerializeReference] T _savedData;

    public T SavedData
    {
        get { return _savedData; }
        private set { _savedData = value; }
    }

    public int DataVersionNumber => _dataVersionNumber;

    public virtual void Awake()
    {
        Load();
    }

    public virtual void OnDestroy()
    {
        Save();
    }

    public virtual void Load()
    {
        var data = JsonUtility.FromJson<T>(PlayerPrefs.GetString(_dataKey, ""));
        if (data != null) SavedData = data;
    }

    public virtual void Save()
    {
        if (SavedData == null) return;

        SavedData.DataVersionNumber = DataVersionNumber;

        PlayerPrefs.SetString(_dataKey, JsonUtility.ToJson(SavedData));
        PlayerPrefs.Save();
    }

    public virtual void OnApplicationFocus(bool focus)
    {
        if (focus == false) Save();
    }
}