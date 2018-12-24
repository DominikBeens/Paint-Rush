using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System;

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;

    public enum SavedStat
    {
        Kills,
        Deaths,
        MarksGained,
        MarksDestroyed,
        GamePointsGained,
        ShotsFired,
        ShotsHit,
        PickupsCollected
    }

    public static event Action OnStatSaved = delegate { };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SaveStat(SavedStat statToSave)
    {
        if (PlayerPrefs.HasKey(statToSave.ToString()))
        {
            PlayerPrefs.SetInt(statToSave.ToString(), PlayerPrefs.GetInt(statToSave.ToString()) + 1);
        }
        else
        {
            PlayerPrefs.SetInt(statToSave.ToString(), 1);
        }

        OnStatSaved();
    }

    public int GetSavedStat(SavedStat stat)
    {
        return PlayerPrefs.GetInt(stat.ToString());
    }

    public void ResetAllStats()
    {
        for (int i = 0; i < Enum.GetValues(typeof(SavedStat)).Length; i++)
        {
            string key = Enum.GetName(typeof(SavedStat), i);

            if (PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt(key, 0);
            }
        }

        OnStatSaved();
    }

    //private void LoadAppSettings()
    //{
    //    if (File.Exists(Application.persistentDataPath + "/SaveData.xml"))
    //    {
    //        saveData = LoadAppSettingsFromFile();
    //    }
    //    else
    //    {
    //        saveData = new SaveData();
    //        SaveAppSettingsToFile(saveData);
    //    }
    //}

    //private SaveData LoadAppSettingsFromFile()
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
    //    using (FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Open))
    //    {
    //        return serializer.Deserialize(stream) as SaveData;
    //    }
    //}

    //public void SaveAppSettingsToFile(SaveData toSave)
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
    //    using (FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Create))
    //    {
    //        serializer.Serialize(stream, toSave);
    //    }
    //}

    //public void OnApplicationQuit()
    //{
    //    SaveAppSettingsToFile(saveData);
    //}
}
