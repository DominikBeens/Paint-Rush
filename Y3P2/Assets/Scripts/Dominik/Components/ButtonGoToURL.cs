using UnityEngine;
using UnityEngine.UI;

public class ButtonGoToURL : MonoBehaviour
{

    private Button button;

    [SerializeField] private string url;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => GoToURL());
    }

    private void GoToURL()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(() => GoToURL());
    }
}
