using System;
using TMPro;
using UnityEngine;

public enum CameraAngle{
    menu = 0,
    whiteTeam = 1,
    blackTeam = 2

}

public class GameUI : MonoBehaviour
{
    public Server server;
    public Client client;
    public static GameUI Instance { set; get ;}

    [SerializeField] private Animator menuAnimator;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private GameObject[] cameraAngle;

    public Action<bool> setLocalGame;
    private void Awake()
    {
        Instance = this;
        RegisterEvents();
    }
    // camera angle
    public void SetCameraAngle(CameraAngle angle)
    {
        for (int i = 0; i < cameraAngle.Length; i++)
        {
            cameraAngle[i].SetActive(false);
        }
        cameraAngle[(int)angle].SetActive(true);
    }
    public void OnLocalGameButton()
    {
        menuAnimator.SetTrigger("InGameMenu");
        setLocalGame?.Invoke(true);
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }
    public void OnOnlineHostButton()
    {
        setLocalGame?.Invoke(false);
        server.Init(8007);
        client.Init("127.0.0.1", 8007);
        menuAnimator.SetTrigger("HostMenu");
    }
    public void OnOnlineConnectButton()
    {
                setLocalGame?.Invoke(false);

        client.Init(addressInput.text, 8007);
    }

    public void OnOnlineBackButton()
    {
        menuAnimator.SetTrigger("StartMenu");
    }
    public void OnOnHostBackButton()
    {
        server.ShutDown();
        client.ShutDown();
        menuAnimator.SetTrigger("OnlineMenu");
    }

    public void OnLeaveFromGameMenu()
    {
        SetCameraAngle(CameraAngle.menu);
        menuAnimator.SetTrigger("StartMenu");
 

    }
    #region 
    private void RegisterEvents()
    {
        NetUtility.C_START_GAME += OnStartGameClient;
        
    }


    private void UnRegisterEvents()
    {
        NetUtility.C_START_GAME -= OnStartGameClient;
    }
    private void OnStartGameClient(NetMessage message)
    {
        menuAnimator.SetTrigger("InGameMenu");
    }

    #endregion
}
