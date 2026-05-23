using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;


public class UICustomPlayer : NetworkBehaviour
{
    [SerializeField]
    TMP_Text TNumberHead;
    [SerializeField]
    TMP_Text TNumberBody;
    [SerializeField]
    TMP_Text TNumberLeg;
    [SerializeField]
    TMP_Text TNumberEqui;
    [SerializeField]
    TMP_Text TNumberAcc;
    [SerializeField]
    TMP_Text TtimeSelect;

    int idHead;
    int idLeg;
    int idAcce;
    int idEqui;
    int idBody;
    [SerializeField]
    float _timeSelect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DataCustom dataCustom = SaveScript.saveScript.LoadFromJson();
        if(dataCustom == null)
        {
            idAcce = 0;
            idBody = 0;
            idEqui = 0;
            idHead = 0;
            idLeg = 0;
        }
        else
        {
            idAcce = dataCustom.idAcce;
            idBody = dataCustom.idBody;
            idEqui = dataCustom.idEqui;
            idHead = dataCustom.idHead;
            idLeg = dataCustom.idLeg;
        }

        TNumberAcc.text = "" + idAcce;
        TNumberEqui.text = "" + idEqui;
        TNumberLeg.text = "" + idLeg;
        TNumberBody.text = "" + idBody;
        TNumberHead.text = "" + idHead;
    }

    void Update()
    {
        if(_timeSelect > 0 && IsServer)
        {
           UpdateTimeClientRpc( _timeSelect -= Time.deltaTime);
        } 
        else if(IsServer)
        {
            LoadScene.loadScene.ChangeScene(2);
        }
    }

    [ClientRpc]
    void UpdateTimeClientRpc(float timeSelect)
    {
        if(TtimeSelect != null)
            TtimeSelect.text = "" + (int)(timeSelect); 
    }

    public void NextHead()
    {
        idHead++;
        idHead = CustomSodier.customSodier.ActiveHead(idHead);
        TNumberHead.text = ""+idHead;
    }

    public void Save()
    {
        DataCustom dataCustom = new DataCustom(1,
            idHead,
            idBody,
            idLeg,
            idAcce,
            idEqui,
            1);

        SaveScript.saveScript.SaveToJson(dataCustom);
    }

    public void PrevHead()
    {
        idHead--;
        if(idHead >= 0)
            idHead = CustomSodier.customSodier.ActiveHead(idHead);
        else
            idHead = 0;
        
        TNumberHead.text = ""+idHead;
    }

    //Body
    public void NextBody()
    {
        idBody++;
        idBody = CustomSodier.customSodier.ActiveBody(idBody);
        TNumberBody.text = ""+idBody;
    }

    public void PrevBody()
    {
        idBody--;
        if(idBody >= 0)
            idBody = CustomSodier.customSodier.ActiveBody(idBody);
        else
            idBody = 0;
        
        TNumberBody.text = ""+idBody;
    }

    //Leg
    public void NextLeg()
    {
        idLeg++;
        idLeg = CustomSodier.customSodier.ActiveLeg(idLeg);
        TNumberLeg.text = ""+idLeg;
    }

    public void PrevLeg()
    {
        idLeg--;
        if(idLeg >= 0)
            idLeg = CustomSodier.customSodier.ActiveLeg(idLeg);
        else
            idLeg = 0;
        
        TNumberLeg.text = ""+idLeg;
    }

    //Equi
    public void NextEqui()
    {
        idEqui++;
        idEqui = CustomSodier.customSodier.ActiveEqui(idEqui);
        TNumberEqui.text = ""+idEqui;
    }

    public void PrevEqui()
    {
        idEqui--;
        if(idEqui >= 0)
            idEqui = CustomSodier.customSodier.ActiveEqui(idEqui);
        else
            idEqui = 0;
        
        TNumberEqui.text = ""+idEqui;
    }

    //Acc
    public void NextAcc()
    {
        idAcce++;
        idAcce = CustomSodier.customSodier.ActiveAcce(idAcce);
        TNumberAcc.text = "" + idAcce;
    }

    public void PrevAcc()
    {
        idAcce--;
        if(idAcce >= 0)
            idAcce = CustomSodier.customSodier.ActiveAcce(idAcce);
        else
            idAcce = 0;
        
        TNumberAcc.text = "" + idAcce;
    }
    
}
