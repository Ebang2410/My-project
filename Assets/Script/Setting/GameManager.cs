using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
public class GameManager : MonoBehaviour
{ 
    public static GameManager gameManager;
    [SerializeField] TMP_Text infoReseau;
    
    private void Awake() {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad( GameObject.FindWithTag("MainCamera"));
        if(gameManager == null)
            gameManager = this;
    }

    private void Start() {
        // S'abonner aux événements pour debug
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong obj)
    {
        if(infoReseau != null)
            infoReseau.text = "DisConnected";
    }

    private void OnClientConnected(ulong obj)
    {
        if(infoReseau != null)
            infoReseau.text = "Connected";

        /* LoadScene.loadScene.ChangeScene(0); */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void VerificationConnection()
    {
        if(NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsClient)
        {
            if(NetworkManager.Singleton.IsConnectedClient)
            {
                
            }
            else
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
    }

    void OnDestroy()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }
        }


}

public enum GameMode
{
    Br,
    Dm,
    DmG,
    Survie,
    Zone
}
