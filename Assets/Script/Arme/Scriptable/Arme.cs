using UnityEngine;

[CreateAssetMenu(fileName = "Arme", menuName = "Scriptable Objects/Arme")]
public class Arme : ScriptableObject
{
    [Header("arme caracteristique")]
    public int id;
    public string name;
    public float degat;
    public TypeArme typeArme;
    [Tooltip("Capacite de l'armee")]
    public int capacite;
    [Tooltip("Capacite de balle a transporter")]
    public int capaciteMax;
    public float timeNextBall;

    [Header("Ball caracteristique")]
    public float distanceVie;
    public float rayonDegat;   
    public float speedBall;

    [Header("Vitesse du joueur avec l'arme")]
    public float run;
    public float walk;

   
}

public enum TypeArme
{
    Infantry,
    Heavy,
    Hangun,
    Knife,
    RocketLauncher,
    All
}