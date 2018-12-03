using UnityEngine;
using TMPro;

public class StatisticsTextUpdater : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        SaveManager.OnStatSaved += SaveManager_OnStatSaved;
        SaveManager_OnStatSaved();
    }

    private void SaveManager_OnStatSaved()
    {
        int markAccuracy = SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksGained) == 0 ? 0 :
            (int)((float)SaveManager.instance.GetSavedStat(SaveManager.SavedStat.GamePointsGained) / SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksGained) * 100);
        int shotAccuracy = SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsFired) == 0 ? 0 :
            (int)((float)SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsHit) / SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsFired) * 100);

        text.text =
            "Kills: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.Kills) + "</color>\n" +
            "Deaths: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.Deaths) + "</color>\n\n" +
            "Marks Gained: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksGained) + "</color>\n" +
            "Marks Destroyed: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.MarksDestroyed) + "</color>\n" +
            "Game-points Gained: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.GamePointsGained) + "</color>\n" +
            "Mark Accuracy: <color=yellow>" + markAccuracy + "%</color>\n\n" +
            "Shots Fired: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsFired) + "</color>\n" +
            "Shots Hit: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.ShotsHit) + "</color>\n" +
            "Accuracy: <color=yellow>" + shotAccuracy + "%</color>\n\n" +
            "Pickups Collected: <color=yellow>" + SaveManager.instance.GetSavedStat(SaveManager.SavedStat.PickupsCollected) + "</color>";
    }

    private void OnDisable()
    {
        SaveManager.OnStatSaved -= SaveManager_OnStatSaved;
    }
}
