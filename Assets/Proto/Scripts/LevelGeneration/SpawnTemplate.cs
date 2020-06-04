using Prototipo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTemplate : MonoBehaviour
{
    [SerializeField] private GameObject[] templates;
    private void Start()
    {
        GameManager.Instance.StateChangeEvent.AddListener(OnGameStateIsPlaying);
    }
    private void OnGameStateIsPlaying(GameManager.GameState oldState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing)
        {
            InstantiateNewTemplate();
        }
    }
    public void InstantiateNewTemplate()
    {
        int rand = Random.Range(0, templates.Length);
        Instantiate(templates[rand], transform.position, Quaternion.identity);
    }
    /*
     */
}
