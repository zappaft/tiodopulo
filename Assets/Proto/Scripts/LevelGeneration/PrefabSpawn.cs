using Prototipo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawn : MonoBehaviour
{
    [SerializeField] private GameObject[] spawnObjs;


    private void Start()
    {
        int rand = Random.Range(0, spawnObjs.Length);
        Instantiate(spawnObjs[rand], transform.position, Quaternion.identity);
    }
}
