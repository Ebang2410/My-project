using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace NetworkDiscoveryUnity
{
    public class UdpDetail : MonoBehaviour
    {
        [SerializeField]
        TMP_Text nameServer;
        [SerializeField]
        TMP_Text typePartie;
        [SerializeField]
        TMP_Text numberPlayer;
        [SerializeField]
        Button joinParty;

        NetworkDiscovery.DiscoveryInfo info;
        
        string id;

        public void UdpInfo(string name, string type, string number, string id, NetworkDiscovery.DiscoveryInfo info)
        {
            nameServer.text = name;
            typePartie.text = type;
            numberPlayer.text = number;
            this.id = id;
            this.info = info;
        }

        public Button getJoinParty()
        {
            return this.joinParty;
        }
    }
}