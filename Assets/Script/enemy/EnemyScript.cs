using UnityEngine;/* 
using Unity.Netcode; */
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class EnemyScript : /* Network */MonoBehaviour 
{
    public ModeMove modeMove;

    NavMeshAgent navMeshAgent;
    Animator animator;
    public Rigidbody rb;

    public Arme arme;
    public GunScript gunScript;
    public KnifeScript knifeScript;
    public TypeArme typeArme;

    public List<Transform> posMoves;
    public Transform posInit;
    public float raduis;
    public float timeLook;

    float countTimeLook;

    public int posFuturFixed;

    Vector3 posFutur;
    bool OnMove;
    public bool AllPosition;
    bool firstMoveFixed;


    public void Init() {
        TryGetComponent(out navMeshAgent);
        TryGetComponent(out animator);
        TryGetComponent(out rb);

        posFutur = Vector3.zero;

        posFuturFixed = 0;
        countTimeLook = 0;
        rb.isKinematic = true;
        OnMove = false;
        AllPosition = false;
        firstMoveFixed = false;
    }

    public void MoveFixed()
    {
        if(posMoves.Count > 0)
        {
            if(!firstMoveFixed)
            {
                navMeshAgent.SetDestination(posMoves[posFuturFixed].position);
                navMeshAgent.speed = arme.walk;
                navMeshAgent.stoppingDistance = 0.2f; 
                OnMove = true;
                firstMoveFixed = true;
            }
            else if(navMeshAgent.remainingDistance > 0.2f && !OnMove)
            {
                navMeshAgent.speed = arme.walk;
                navMeshAgent.stoppingDistance = 0f; 
                OnMove = true;
                 AnimeMove(1,false,.5f, 0, 1);
            }
            else if(navMeshAgent.remainingDistance <= 0.2f && countTimeLook <= 0 )
            {
                AnimeMove(1,false, 0, 0, 1);
                if(!AllPosition)
                {
                    posFuturFixed ++;
                    if(posFuturFixed == posMoves.Count - 1)
                        AllPosition = true;
                }
                else
                {
                    posFuturFixed --;
                    if(posFuturFixed == 0)
                        AllPosition = false;
                }

                navMeshAgent.SetDestination(posMoves[posFuturFixed].position);

                OnMove =false;
                countTimeLook = timeLook;
            }
            else if(navMeshAgent.remainingDistance <= 0.2f)
            {
                AnimeMove(1,false, 0, 0, 1);
                countTimeLook -= Time.deltaTime;
            }

        }
    }

    public void MoveAleatoire(float timeLook)
    {
        if((posFutur == Vector3.zero || navMeshAgent.remainingDistance < 0.2f) && countTimeLook <= 0.0f)
        {
            posFutur = PositionFutur(posInit.position,raduis,navMeshAgent);
            navMeshAgent.SetDestination(posFutur);
        }
        else if(navMeshAgent.remainingDistance > 0.2f && !OnMove)
        {
            navMeshAgent.speed = arme.walk;
            navMeshAgent.stoppingDistance = 0.2f; 
            OnMove = true;
            countTimeLook = timeLook;
            AnimeMove(1,false,.5f, 0, 1);
        }
        else if(navMeshAgent.remainingDistance <= 0.2f)
        {
            AnimeMove(1,false, 0, 0, 1);
            OnMove = false;
            countTimeLook -= Time.deltaTime;
        }   
    }

    public void Unique()
    {
        
    }

    public void MoveForPlayer(Transform playerPos)
    {
        navMeshAgent.SetDestination(playerPos.position);
        navMeshAgent.speed = arme.run;
    }


    public void AnimeMode()
    {
        switch (typeArme)
        {
            case TypeArme.Hangun :
                animator.SetFloat("type",0f);
            break;
            case TypeArme.Heavy :
                animator.SetFloat("type",0.25f);
            break;
            case TypeArme.Infantry :
                animator.SetFloat("type",0.5f);
            break;
            case TypeArme.Knife :
                animator.SetFloat("type",0.75f);
            break;
            case TypeArme.RocketLauncher :
                animator.SetFloat("type",1f);
            break;
        }
    }

    Vector3 PositionFutur(Vector3 positionInit, float raduisSphere, NavMeshAgent _agent)
    {
        Vector3 posFutur = positionInit + (Random.onUnitSphere * raduisSphere);
        
        posFutur.y = positionInit.y;
        
        if(NavMesh.SamplePosition(posFutur, out NavMeshHit hit,2f,NavMesh.AllAreas))
        {
            Debug.Log(hit.position);
            return hit.position;
        }
        else
        {
            return positionInit;
        }
    }

    public void AnimeMove(float ActArme,bool crouch,float speedMove, float x, float y)
    {
        animator.SetFloat("positionX",x);
        animator.SetFloat("positionY",y);
        animator.SetFloat("speed",speedMove);
        animator.SetFloat("arme",ActArme);
        animator.SetBool("crouch",crouch);
    }

    public void Attack()
    {
        animator.SetTrigger("attack");
    }

    #if UNITY_EDITOR
        private void OnDrawGizmos() {
            if(modeMove == ModeMove.Aleatoire)
            {
                Gizmos.DrawWireSphere(posInit.position,raduis);
            }
        }
    #endif

}

public enum ModeMove
{
    Fixed,
    Aleatoire,
    Unique
}
