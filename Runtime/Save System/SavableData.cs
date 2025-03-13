using UnityEngine;

public abstract class SavableData<T> : MonoBehaviour where T : class
{
    [SerializeField] string dataName;
    [SerializeField] T savedData;

    public T SavedData
    {
        get
        {
            return savedData;
        }
        set
        {
            savedData = value;
        }
    }

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
        var data = JsonUtility.FromJson<T>(PlayerPrefs.GetString(dataName, ""));

        if(data != null)
            SavedData = data;
    }
    public virtual void Save()
    {
        if (SavedData == null)
            return;

        PlayerPrefs.SetString(dataName, JsonUtility.ToJson(SavedData));
        PlayerPrefs.Save();
    }
    public virtual void OnApplicationFocus(bool focus)
    {
        if(focus == false)
        {
            Save();
        }
    }
}
