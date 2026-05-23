using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;

namespace NetworkDiscoveryUnity
{
    public class NetworkManagerDiscovery : MonoBehaviour
    {
        [SerializeField] UnityTransport transport;
        [SerializeField] TMP_Text statut;
        bool wifiHost ;
        [Header("Client")]
        [SerializeField] GameObject panelClient;
        [SerializeField] Dictionary<string, NetworkDiscovery.DiscoveryInfo> servers = new Dictionary<string, NetworkDiscovery.DiscoveryInfo>();
        [SerializeField] GameObject ServerDetail;
        [SerializeField] Transform content;
        [SerializeField] Button  buttonHost;
        [SerializeField] private NetworkDiscovery networkDiscovery;

        [Header("Server")]
        [SerializeField] GameObject panelServer;
        [SerializeField] GameObject panelAddHost;
        [SerializeField] TMP_InputField nameServer;
        [SerializeField] TMP_Dropdown typeParty;
        
        int numberPlayerConnect;
        private string serverLocalIP; // TON ADRESSE IP
        [SerializeField] SettingHostNetwork settingHostNetwork;
        void Start()
        {
            wifiHost = false;
            
            transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            //Lancement du client Udp
            NetworkDiscovery.Instance.onReceivedServerResponse.AddListener(OnServerDiscovered);
            NetworkDiscovery.Instance.SendBroadcast();
            
            InvokeRepeating("SearchServers", 0f, 5f);

            numberPlayerConnect = 0;

            statut.text = "Recherche des serveurs...";

            InitListServer();
            panelClient.SetActive(true);
            //panelAddHost.SetActive(false);
            panelServer.SetActive(false);
        }

        void SearchServers()
        {
            NetworkDiscovery.Instance.SendBroadcast();
        }

        void OnServerDiscovered(NetworkDiscovery.DiscoveryInfo info)
        {
            // Éviter les doublons          
            if (!servers.ContainsKey(info.EndPoint.Address.ToString()))
            {
                CreateServerEntry(info);
            }
        }
        
        void CreateServerEntry(NetworkDiscovery.DiscoveryInfo info)
        {
            GameObject entry = Instantiate(ServerDetail, content);
            
            string ip = info.EndPoint.Address.ToString();
            ushort port = info.GetGameServerPort();
            
            string ipText = $"{ip}:{port}";
            string nameserver = "Server";
            string typePartyStr = "Default";
            string numberPlayerStr = "0/8";
            
            if (info.KeyValuePairs.TryGetValue("GameName", out string map))
                nameserver = map;
                
            if (info.KeyValuePairs.TryGetValue("Party", out string party))
                typePartyStr = party;

            if (info.KeyValuePairs.TryGetValue("CountPlayer", out string currentPlayers) &&
                info.KeyValuePairs.TryGetValue("MaxPlayers", out string maxPlayers))
                numberPlayerStr = $"{currentPlayers}/{maxPlayers}";

            UdpDetail detail = entry.transform.GetComponent<UdpDetail>();
            detail.UdpInfo(nameserver, typePartyStr, numberPlayerStr, ipText, info);
            
            Button button = detail.getJoinParty();
            button.onClick.AddListener(() => ConnectToServer(info));
            
            servers.Add(ip,info);
        }

        // Client connection serveur
        public void ConnectToServer(NetworkDiscovery.DiscoveryInfo info)
        {
            try
            {
                // Arrêter NetworkManager s'il est déjà en cours
                if (NetworkManager.Singleton.IsListening)
                {
                    NetworkManager.Singleton.Shutdown();
                    System.Threading.Thread.Sleep(100);
                }
                
                string ip = info.EndPoint.Address.ToString();
                ushort port = info.GetGameServerPort();
                
                Debug.Log($"Tentative de connexion à {ip}:{port}");
                
                // VÉRIFIER QUE C'EST LA BONNE IP
                
                if (info.KeyValuePairs.TryGetValue("ServerIP", out string serverIP))
                {
                    ip = serverIP;
                    Debug.Log($"Utilisation de l'IP du serveur: {ip}");
                }

                // Configurer le transport
                transport.ConnectionData.Address = ip;
                transport.ConnectionData.Port = port;
                
                Debug.Log($"Configuration transport: {transport.ConnectionData.Address}:{transport.ConnectionData.Port}");
                
                // Démarrer le client
                bool success = NetworkManager.Singleton.StartClient();
                
                if (success)
                {
                    CloseClient();
                    statut.text = "Client connecte au server ip:" + ip;
                    panelClient.SetActive(false);
                    panelServer.SetActive(true);  

                    StartCoroutine(SpawnInfoPlayer(true));          
                }
                else
                {
                    Debug.LogError("Échec du démarrage client");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Erreur de connexion: {e.Message}\nStack: {e.StackTrace}");
            }
        }

        //Arret client Udp
        public void CloseClient()
        {
            networkDiscovery.onReceivedServerResponse.RemoveListener(OnServerDiscovered);
            NetworkDiscovery.Instance.CloseClientUdpClient();
            InitListServer();
            CancelInvoke("SearchServers");
        }

        void InitListServer()
        {
            if (servers.Count > 0)
            {
                servers.Clear();
            }

            foreach (Transform item in content)
            {
                Destroy(item.gameObject);
            }
        }

        //Arret server Udp
        public void CloseServer()
        {
            NetworkDiscovery.Instance.CloseServerUdpClient();
        }

        //Stop du server UDP et NGo, puis active upd pour tout les client du reseau (lance par le host)
        public void CloseSeverSwichtStarClientByAll()
        {
            //Arret server 
            CloseServer();
            //Lancement du client Udp
            NetworkDiscovery.Instance.onReceivedServerResponse.AddListener(OnServerDiscovered);
            NetworkDiscovery.Instance.SendBroadcast();

            InitListServer();

            statut.text = "Recherche des serveurs...";
            
            InvokeRepeating("SearchServers", 0f, 5f);

            numberPlayerConnect = 0;
            settingHostNetwork.CloseHostServerRpc();
        }

        //stop NGO et start server UDP pour un client 
        public void CloseSeverSwichtStarClientByOne()
        {
            //Arret server 
            CloseServer();
            //Lancement du client Udp
            NetworkDiscovery.Instance.onReceivedServerResponse.AddListener(OnServerDiscovered);
            NetworkDiscovery.Instance.SendBroadcast();

            InitListServer();
            
            InvokeRepeating("SearchServers", 0f, 5f);

            numberPlayerConnect = 0;

            panelClient.SetActive(true);
            //panelAddHost.SetActive(false);
            panelServer.SetActive(false);
            settingHostNetwork.CloseHostOne();
        }

        public void CreateHost()
        {
            nameServer.text = "";
            typeParty.value = 0;
            panelClient.SetActive(false);
            panelAddHost.SetActive(true);
            statut.text = "Creaction d'une partie";
        }

        public void CreateHostClose()
        {
            panelAddHost.SetActive(false);
            panelClient.SetActive(true);
            statut.text = "Recherche des serveurs...";
        }

        public void StartServer()
        {
            try
            {
                if(typeParty.value == 0)
                {
                    statut.text = "Selection le type partie";
                    return;
                }

                if(string.IsNullOrEmpty(nameServer.text))
                {
                    statut.text = "entrer le nom de la partie";
                    return;
                }
                // Arrêter le client de découverte
                CloseClient();
                
                ushort serverPort = 7777; // Port du jeu
                
                // Configurer le transport du serveur
                transport.ConnectionData.Port = serverPort;
                transport.ConnectionData.ServerListenAddress = "0.0.0.0"; // IMPORTANT: écouter sur toutes les interfaces
                
                Debug.Log($"Serveur configuré sur: {serverLocalIP}:{serverPort}");
                
                // Configurer la découverte avec l'IP CORRECTE du serveur
                NetworkDiscovery.Instance.EnsureServerIsInitialized();
                
                // Ajouter l'IP du serveur dans les données de découverte
                NetworkDiscovery.Instance.RegisterResponseData("ServerIP", serverLocalIP);
                NetworkDiscovery.Instance.RegisterResponseData("GameName", nameServer.text);
                NetworkDiscovery.Instance.RegisterResponseData("Party", typeParty.options[typeParty.value].text);
                NetworkDiscovery.Instance.RegisterResponseData("MaxPlayers","8");
                NetworkDiscovery.Instance.RegisterResponseData("CountPlayer", "0");
                NetworkDiscovery.Instance.RegisterResponseData("GamePort", serverPort.ToString());
                
                // Démarrer le host
                bool success = NetworkManager.Singleton.StartHost();
                
                if (success)
                {
                    statut.text = $"Serveur est démarré avec succès sur {serverLocalIP}:{serverPort}";
                    panelClient.SetActive(false);
                    panelServer.SetActive(true);

                
                }
                else
                {
                    statut.text = "Échec du démarrage du serveur" ;
                }
            }
            catch (System.Exception e)
            {
                statut.text = $"Erreur lors du démarrage du serveur: {e.Message}";
            }
        }

        void UpdatePlayerCount()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                int currentPlayers = NetworkManager.Singleton.ConnectedClients.Count;
                NetworkDiscovery.Instance.RegisterResponseData("CountPlayer", currentPlayers.ToString());
                Debug.Log($"Joueurs connectés: {currentPlayers}/{numberPlayerConnect}");
            }
        }
        

        void Update() {
            if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || 
                Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
            {
                if(!wifiHost)
                {
                    GetLocalIp();
                    if(serverLocalIP == "127.0.0.1")
                        wifiHost = false;
                    else
                        wifiHost = true;

                    buttonHost.interactable = wifiHost;
                }
            }
            else
            {
                buttonHost.interactable = false ;
                wifiHost = false;
            }
        }

        //Ip du serveur
        void GetLocalIp()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    serverLocalIP = ip.ToString();
                    break;
                }
            }
        }

        public void UpdateCountPlayer(int count)
        {
            NetworkDiscovery.Instance.RegisterResponseData("CountPlayer", ""+count);
        }


        // test simple
        public void StartServerOne()
        {
            try
            {
                ushort serverPort = 7777; // Port du jeu
                
                // Configurer le transport du serveur
                transport.ConnectionData.Port = serverPort;
                transport.ConnectionData.ServerListenAddress = "0.0.0.0"; // IMPORTANT: écouter sur toutes les interfaces
                
                Debug.Log($"Serveur configuré sur: {serverLocalIP}:{serverPort}");
                
                // Démarrer le host
                bool success = NetworkManager.Singleton.StartHost();
                
                if (success)
                {
                    // Configurer la découverte avec l'IP CORRECTE du serveur
                    NetworkDiscovery.Instance.EnsureServerIsInitialized();
                    
                    // Ajouter l'IP du serveur dans les données de découverte
                    NetworkDiscovery.Instance.RegisterResponseData("ServerIP", serverLocalIP);
                    NetworkDiscovery.Instance.RegisterResponseData("GameName", "First");
                    NetworkDiscovery.Instance.RegisterResponseData("Party", "match");
                    NetworkDiscovery.Instance.RegisterResponseData("MaxPlayers","8");
                    NetworkDiscovery.Instance.RegisterResponseData("CountPlayer", "0");
                    NetworkDiscovery.Instance.RegisterResponseData("GamePort", serverPort.ToString());
                    
                    statut.text = $"Serveur est démarré avec succès sur {serverLocalIP}:{serverPort}";
                    panelClient.SetActive(false);
                    panelServer.SetActive(true);

                    StartCoroutine(SpawnInfoPlayer(false));
                }
                else
                {
                    statut.text = "Échec du démarrage du serveur" ;
                }
            }
            catch (System.Exception e)
            {
                statut.text = $"Erreur lors du démarrage du serveur: {e.Message}";
            }
        }

        IEnumerator SpawnInfoPlayer(bool client)
        {
            yield return new  WaitForSeconds(1.5f);
            settingHostNetwork.Info(client);
        }
    }
}