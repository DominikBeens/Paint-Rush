using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameSparksManager : MonoBehaviour
{

    public static GameSparksManager instance;
    private bool canUseButtons = true;

    [Header("Register")]
    [SerializeField] private TMP_InputField setUserNameField;
    [SerializeField] private TMP_InputField setDisplayNameField;
    [SerializeField] private TMP_InputField setPasswordField;
    [SerializeField] private UnityEvent OnRegister;

    [Header("Login")]
    [SerializeField] private TMP_InputField userNameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private UnityEvent OnLogin;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }

        OnRegister.AddListener(() => canUseButtons = true);
        OnLogin.AddListener(() => canUseButtons = true);
    }

    public void RegisterButton()
    {
        if (!canUseButtons)
        {
            return;
        }

        canUseButtons = false;

        new GameSparks.Api.Requests.RegistrationRequest()
            .SetDisplayName(setDisplayNameField.text)
            .SetPassword(setPasswordField.text)
            .SetUserName(setUserNameField.text)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    ResetAllText();
                    userNameField.text = response.UserId;

                    OnRegister.Invoke();
                    Debug.Log("AUTH: Player Registered");
                }
                else
                {
                    Debug.Log("AUTH: Error Registering Player");
                }
            });
    }

    public void LoginButton()
    {
        if (!canUseButtons)
        {
            return;
        }

        canUseButtons = false;

        new GameSparks.Api.Requests.AuthenticationRequest()
            .SetUserName(userNameField.text)
            .SetPassword(passwordField.text)
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    ResetAllText();
                    Photon.Pun.PhotonNetwork.NickName = response.DisplayName;

                    OnLogin.Invoke();
                    Debug.Log("AUTH: Player Authenticated...");
                }
                else
                {
                    Debug.Log("AUTH: Error Authenticating Player...");
                }
            });
    }

    private void ResetAllText()
    {
        setUserNameField.text = "";
        setDisplayNameField.text = "";
        setPasswordField.text = "";

        userNameField.text = "";
        passwordField.text = "";
    }
}
