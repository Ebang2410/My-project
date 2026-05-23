using UnityEngine;

[System.Serializable]
public class DataCustom
{
    private long id;
    public int idHead;
    public int idBody;
    public int idLeg;
    public int idAcce;
    public int idEqui;
    public int idWeapon;

    //function
    public DataCustom(long id, int head, int body, int leg, int acc, int equi, int weapon)
    {
        this.id = id;
        this.idHead = head;
        this.idBody = body;
        this.idLeg = leg;
        this.idAcce = acc;
        this.idEqui = equi;
        this.idWeapon = weapon;
    }
}
