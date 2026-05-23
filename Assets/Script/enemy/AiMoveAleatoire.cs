using UnityEngine;
using UnityEngine.AI;

public static class AiMoveAleatoire
{
    public static Vector3 PositionFutur(Vector3 positionInit, float raduis, NavMeshAgent _agent)
    {
        Vector3 posFutur = Random.onUnitSphere * raduis;
        
        if(NavMesh.SamplePosition(posFutur, out NavMeshHit hit,2f,NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return PositionFutur(positionInit, raduis/2, _agent);
        }
    }
}
