using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class PlayerItemNetwork : MonoBehaviour {
    public ulong id;
    public string name;
    public bool play;
    public TMP_Text namePlayer;
    [SerializeField]
    GameObject valider; 
    [SerializeField]
    GameObject invalider; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        valider.SetActive(false);
        invalider.SetActive(true);

        if(string.IsNullOrEmpty(name))
        {
            namePlayer.text = "player";
        }
        else
        {
            namePlayer.text = name;
        }
    }


}
