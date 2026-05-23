using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
public class SaveScript : MonoBehaviour
{
    public static SaveScript saveScript;

    private void Awake() {
        if(saveScript == null)
            saveScript = this;
        else
            transform.gameObject.SetActive(false);
    }
    
    string cle = "A@ierpP&skd83ds3kjsjah9320!,*%#.";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveToJson(DataCustom dataCustom)
    {
        string dataJson = JsonUtility.ToJson(dataCustom); // conversion en json
        string filePath = Application.persistentDataPath + "/Data.json";

        Debug.Log(filePath);

        System.IO.File.WriteAllText(filePath,dataJson);
        Debug.Log("Save ok");
    }

    public DataCustom LoadFromJson()
    {
        string filePath = Application.persistentDataPath + "/Data.json";
        if(System.IO.File.Exists(filePath))
        {
            string data = System.IO.File.ReadAllText(filePath);

            DataCustom dataCustom = JsonUtility.FromJson< DataCustom >(data);

            return dataCustom;
        }
        else
            return null;
    }

    string Encrypt(string text)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(cle.PadRight(32));
        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                byte[] encrypted = encryptor.TransformFinalBlock(textBytes,0,textBytes.Length);

                byte[] combined = new byte[aes.IV.Length + encrypted.Length];
                System.Buffer.BlockCopy(aes.IV,0,combined,0,aes.IV.Length);
                System.Buffer.BlockCopy(encrypted,0,combined,aes.IV.Length,encrypted.Length);

                return System.Convert.ToBase64String(combined);
            }
        }
    }

    string Decrypt(string encrytedText)
    {
        byte[] combined = System.Convert.FromBase64String(encrytedText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(cle.PadRight(32));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            byte[] iv = new byte[16];
            byte[] encrypted = new byte[combined.Length - 16];

            System.Buffer.BlockCopy(combined,0,iv,0,16);
            System.Buffer.BlockCopy(combined,16,encrypted,0,encrypted.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key,aes.IV))
            {
                byte[] decrypted = decryptor.TransformFinalBlock(encrypted,0,encrypted.Length);

                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}
