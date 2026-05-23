using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CustomSodier : MonoBehaviour
{
    public static CustomSodier customSodier;
    [SerializeField]
    List<GameObject> heads;
    [SerializeField]
    List<GameObject> legs;
    [SerializeField]
    List<GameObject> equis;
    [SerializeField]
    List<GameObject> acces;
    [SerializeField]
    List<GameObject> bodys;

    public int countHead;
    public int countLeg;
    public int countEqui;
    public int countAcce;
    public int countBody;

    [SerializeField]
    Joystick joystick;
    Transform cam;
    float turn = 0.14f;
    float smooth;

    int idHead;
    int idLeg;
    int idAcce;
    int idEqui;
    int idBody;
    private void Awake() {
        if(customSodier == null)
        {
            customSodier = this;
        }
        else
            Destroy(this.gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DesactiveAll();
        
        //init value
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

        ActiveOne();

        countAcce = acces.Count;
        countBody = bodys.Count;
        countEqui = equis.Count;
        countHead = heads.Count;
        countLeg = legs.Count;

        cam = GameObject.FindWithTag("MainCamera").transform;
    }

    private void Update() {
        if(joystick.Direction.sqrMagnitude >= 0.01f)
        {
            Vector2 move2d = joystick.Direction;
            float angle = Mathf.Atan2(move2d.x,move2d.y)*Mathf.Rad2Deg;
            float rotate = Mathf.SmoothDampAngle(transform.eulerAngles.y,angle,ref smooth,turn);
            transform.rotation = Quaternion.Euler(0,rotate,0);
        }
    }

    void DesactiveAll()
    {
        // head
        foreach (GameObject item in heads)
        {
            if(item != null)
                item.SetActive(false);
        }

        // body
        foreach (GameObject item in bodys)
        {
            if(item != null)
                item.SetActive(false);
        }

        // leg
        foreach (GameObject item in legs)
        {
            if(item != null)
                item.SetActive(false);
        }

        // acce
        foreach (GameObject item in acces)
        {
            if(item != null)
                item.SetActive(false);
        }

        // equi
        foreach (GameObject item in equis)
        {
            if(item != null)
                item.SetActive(false);
        }
    }

    void ActiveOne()
    {
        if(heads[idHead] != null)
            heads[idHead].SetActive(true);
        
        if(legs[idLeg] != null)
            legs[idLeg].SetActive(true);

        if(bodys[idBody] != null)
            bodys[idBody].SetActive(true);
            
        if(equis[idEqui] != null)
            equis[idEqui].SetActive(true);

        if(acces[idAcce] != null)
            acces[idAcce].SetActive(true);
    }

    public int ActiveHead(int id)
    {
        if(id >= countHead)
        {
            if(heads[idHead] != null)
                heads[idHead].SetActive(false);
            if(heads[0] != null)
                heads[0].SetActive(true);
        
            return idHead = 0;
        }
        else if(id == idHead)
        {
            return id;
        }

        if(heads[idHead] != null)
            heads[idHead].SetActive(false);
        if(heads[id] != null)
            heads[id].SetActive(true);

        return idHead = id;
    }

    public int ActiveLeg(int id)
    {
        if(id >= countLeg)
        {
            if(legs[idLeg] != null)
                legs[idLeg].SetActive(false);
            if(legs[0] != null)
                legs[0].SetActive(true);

            return idLeg = 0;
        }
        else if(id == idLeg)
        {
            return id;
        }

        if(legs[idLeg] != null)
            legs[idLeg].SetActive(false);
        if(legs[id] != null)
            legs[id].SetActive(true);

        return idLeg = id;
    }

    public int ActiveEqui(int id)
    {
        if(id >= countEqui)
        {
            if(equis[idEqui] != null)
                equis[idEqui].SetActive(false);

            return idEqui = 0;
        }
        else if(id == idEqui)
        {
            return id;
        }

        if(equis[idEqui] != null)
            equis[idEqui].SetActive(false);
        if(equis[id] != null)
            equis[id].SetActive(true);

        this.idEqui = id;
        return id;
    }

    public int ActiveAcce(int id)
    {
        if(id >= countAcce)
        {
            if(acces[idAcce] != null)
                acces[idAcce].SetActive(false);
            idAcce = 0;
            return 0;
        }
        else if(id == idAcce)
        {
            return id;
        }

        if(acces[idAcce] != null)
            acces[idAcce].SetActive(false);
        if(acces[id] != null)
            acces[id].SetActive(true);

        return idAcce = id;
    }

    public int ActiveBody(int id)
    {
        if(id >= countBody)
        {
            if(bodys[idBody] != null)
                bodys[idBody].SetActive(false);
            if(bodys[0] != null)
                bodys[0].SetActive(true);

            return idBody = 0;
        }
        else if(id == idBody)
        {
            return id;
        }

        if(bodys[idBody] != null)
        {
            bodys[idBody].SetActive(false);
        }
        if(bodys[id] != null)
            bodys[id].SetActive(true);

        return idBody = id;
    }

}
