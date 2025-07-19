using Unity.Collections;
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
        {
            data = CloneDefault();
            data.DataVersionNumber = DataVersionNumber;
            SavedData = InitData(data);
        }
        else
        {
            if(data.DataVersionNumber < DataVersionNumber)
            {
                SavedData = MigrateData(data, data.DataVersionNumber, DataVersionNumber);
                SavedData.DataVersionNumber = DataVersionNumber;
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

        PlayerPrefs.SetString(_dataKey, JsonUtility.ToJson(SavedData));
        PlayerPrefs.Save();
    }

    public virtual void OnApplicationFocus(bool focus)
    {
        if (focus == false) Save();
    }

    protected virtual T MigrateData(T oldData, int oldVer, int newVer)
    {
        return oldData;
    }

    protected virtual T InitData(T data)
    {
        return data;
    }

    private T CloneDefault()
    {
        string json = JsonUtility.ToJson(_defaultData);
        return JsonUtility.FromJson<T>(json);
    }
}