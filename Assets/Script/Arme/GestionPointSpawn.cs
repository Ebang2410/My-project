using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestionPointSpawn : MonoBehaviour
{
    public bool isCrouch;
    [SerializeField]
    List<Transform> pointSpawns;
    [SerializeField]
    List<Transform> pointSpawnsCrouch;

    Transform pointSpawn;
    Transform pointSpawnCrouch;
    [SerializeField]
    Transform originSpawn;
    [SerializeField]
    Transform originSpawnCrouch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        isCrouch = false;
        if(pointSpawns.Count > 0)
        {
            pointSpawns.Clear();
        }

        foreach (Transform item in originSpawn)
        {
            if(item != null)
                pointSpawns.Add(item);
        }

        foreach (Transform item in originSpawnCrouch)
        {
            if(item != null)
                pointSpawnsCrouch.Add(item);
        }

        Init();
    }


    void Init()
    {
        if(pointSpawns.Count > 0)
        {
            foreach (Transform item in pointSpawns)
            {
                item.gameObject.SetActive(false);
            }
        }
    }

    public void SpawnPointActive(Transform pointTrans)
    {
        if (!isCrouch)
        {
            if(pointSpawn != null)
            {
                pointSpawn.gameObject.SetActive(false);
                return;
            }

            foreach (Transform item in pointSpawns)
            {
                if(item == pointTrans)
                {
                    pointSpawn = pointTrans;
                    pointSpawn.gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            if(pointSpawnCrouch != null)
            {
                pointSpawnCrouch.gameObject.SetActive(false);
                return;
            }

            foreach (Transform item in pointSpawnsCrouch)
            {
                if(item == pointTrans)
                {
                    pointSpawn = pointTrans;
                    pointSpawn.gameObject.SetActive(true);
                    break;
                }
            }
        }

        
    }

    public void SpawnPointOff(Transform pointTrans)
    {
        if(pointSpawn != null)
        {
            pointSpawn.gameObject.SetActive(false);
        }
    }

    public void CrouchChange(bool newValue)
    {
        isCrouch = newValue;
    }
}
