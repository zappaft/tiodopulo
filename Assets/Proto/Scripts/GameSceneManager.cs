using Prototipo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {

    public static GameSceneManager Instance { get; private set; }

    private void Awake() {
        if (!Instance) Instance = this;
        else Destroy(this.gameObject);
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.GameBeginning();
    }
}
