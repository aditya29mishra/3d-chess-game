using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public Server server;
    public Client client;
    public static GameUI Instance { set; get ;}

    [SerializeField] private Animator menuAnimator;
    [SerializeField] private TMP_InputField addressInput;

    private void Awake()
    {
        Instance = this;
    }

    public void OnLocalGameButton()
    {
        menuAnimator.SetTrigger("InGameMenu");
        server.Init(8005);
        client.Init("127.0.0.1", 8005);
    }
    public void OnOnlineGameButton()
    {
        menuAnimator.SetTrigger("OnlineMenu");
    }
    public void OnOnlineHostButton()
    {
        server.Init(8005);
        client.Init("127.0.0.1", 8005);
        menuAnimator.SetTrigger("HostMenu");
    }
    public void OnOnlineConnectButton()
    {
        client.Init(addressInput.text, 8005);
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
}
