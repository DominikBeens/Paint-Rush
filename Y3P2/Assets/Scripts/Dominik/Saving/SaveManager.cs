using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveManager : MonoBehaviour
{

    public static SaveManager instance;
    public static SaveData saveData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        LoadAppSettings();
    }

    private void LoadAppSettings()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveData.xml"))
        {
            saveData = LoadAppSettingsFromFile();

            Debug.LogWarning("LOAD SAVEDATA");
        }
        else
        {
            saveData = new SaveData();
            SaveAppSettingsToFile(saveData);

            Debug.LogWarning("CREATE NEW SAVEDATA");
        }
    }

    private SaveData LoadAppSettingsFromFile()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        using (FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Open))
        {
            return serializer.Deserialize(stream) as SaveData;
        }
    }

    public void SaveAppSettingsToFile(SaveData toSave)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        using (FileStream stream = new FileStream(Application.persistentDataPath + "/SaveData.xml", FileMode.Create))
        {
            serializer.Serialize(stream, toSave);
        }
    }

    public void OnApplicationQuit()
    {
        SaveAppSettingsToFile(saveData);
    }
}
