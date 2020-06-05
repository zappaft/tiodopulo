using Prototipo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTemplate : MonoBehaviour
{
    [SerializeField] private GameObject[] templates;
    public static SpawnTemplate Instantiate;

    private void Start()
    {
        Instantiate = this;
        GameManager.Instance.StateChangeEvent.AddListener(OnGameStateIsPlaying);
    }
    private void OnGameStateIsPlaying(GameManager.GameState oldState, GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.Playing && oldState == GameManager.GameState.Menu)
        {
            Debug.Log("Instanciado spawntemplate");
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
