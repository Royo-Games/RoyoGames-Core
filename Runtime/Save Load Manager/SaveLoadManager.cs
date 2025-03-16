using UnityEngine;

public abstract class SaveLoadManager<T> : MonoBehaviour where T : SavableData, new()
{
    [SerializeField] string _dataKey;
    [SerializeField] private int _dataVersionNumber;
    [Space]
    [SerializeField] T _data;

    public T Data
    {
        get { return _data; }
        private set { _data = value; }
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
        if (data != null) Data = data;
    }

    public virtual void Save()
    {
        if (Data == null) return;

        Data.DataVersionNumber = DataVersionNumber;

        PlayerPrefs.SetString(_dataKey, JsonUtility.ToJson(Data));
        PlayerPrefs.Save();
    }

    public virtual void OnApplicationFocus(bool focus)
    {
        if (focus == false) Save();
    }
}