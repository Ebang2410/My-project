using UnityEngine;

public class SoliderGun : EnemyScript
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        /* if(IsServer) */
            if(modeMove == ModeMove.Aleatoire)
                MoveAleatoire(5f);
            else if(modeMove == ModeMove.Fixed)
                MoveFixed();

           AnimeMode();
    }
}
