using UnityEngine;

public abstract class SaveLoadManager<T> : MonoBehaviour where T : SavableData, new()
{
    [SerializeField] private string _dataKey;
    [SerializeField] private int _dataVersionNumber;
    [Space]
    [SerializeField] private T _defaultData;
    [SerializeField] private T _savedData;

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

        if (data == null)
            SavedData = CloneDefault();
        else
        {
            if(SavedData.DataVersionNumber < DataVersionNumber)
            {
                SavedData = Migrate(data, data.DataVersionNumber, DataVersionNumber);
            }
            else
            {
                SavedData = data;
            }
        }
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

    protected virtual T Migrate(T oldData, int oldVer, int newVer)
    {
        return oldData;
    }

    private T CloneDefault()
    {
        string json = JsonUtility.ToJson(_defaultData);
        return JsonUtility.FromJson<T>(json);
    }
}