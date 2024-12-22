using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_14 : MonoBehaviour
{
   private GameObject EnvPrefab;
    private float nextSpawnTime;

    [SerializeField]
    private float spawnDelay = 10;

    private void Awake()
    {
      var env = Resources.Load("Level_14");
      EnvPrefab = env as GameObject; 
    }

    private void Start()
    {
       // if(ShouldSpawn())
     //  {
            Spawn();
       //}
    }

    private void Spawn()
    {
     // nextSpawnTime = Time.time + spawnDelay;
       Instantiate(EnvPrefab, transform.position, transform.rotation); 
    }

    // private bool ShouldSpawn()
    // {
    //     return Time.time > nextSpawnTime;
    // }
}
