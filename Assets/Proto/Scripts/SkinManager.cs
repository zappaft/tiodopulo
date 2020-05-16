using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Prototipo {

    public class SkinManager : MonoBehaviour {

        [SerializeField] private string charName, skinName;
        private SpriteRenderer charRenderer, skinRenderer;
        [SerializeField] private List<SkinData> chars, skins;

        private void Start() {
            charRenderer = GetComponent<SpriteRenderer>();
            skinRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
            charName = PlayerPrefs.GetString("ActiveChar", chars[0]._name);
            skinName = PlayerPrefs.GetString("ActiveSkin", skins[0]._name);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Q)) {
                int ind = chars.FindIndex(c => c._name == charName);
                if (ind-- <= 0) ind = chars.Count - 1;
                charName = chars[ind]._name;
                PlayerPrefs.SetString("ActiveChar", charName);
                charRenderer.sprite = chars[ind].skin;
                charRenderer.color = chars[ind].color;
            }

            if (Input.GetKeyDown(KeyCode.W)) {
                int ind = chars.FindIndex(c => c._name == charName);
                if (ind++ >= chars.Count - 1) ind = 0;
                charName = chars[ind]._name;
                PlayerPrefs.SetString("ActiveChar", charName);
                charRenderer.sprite = chars[ind].skin;
                charRenderer.color = chars[ind].color;
            }

            if (Input.GetKeyDown(KeyCode.E)) {
                int ind = skins.FindIndex(c => c._name == skinName);
                if (ind-- <= 0) ind = skins.Count - 1;
                skinName = skins[ind]._name;
                PlayerPrefs.SetString("ActiveSkin", skinName);
                skinRenderer.sprite = skins[ind].skin;
                skinRenderer.color = skins[ind].color;

            }

            if (Input.GetKeyDown(KeyCode.R)) {
                int ind = skins.FindIndex(c => c._name == skinName);
                if (ind++ >= skins.Count - 1) ind = 0;
                skinName = skins[ind]._name;
                PlayerPrefs.SetString("ActiveSkin", skinName);
                skinRenderer.sprite = skins[ind].skin;
                skinRenderer.color = skins[ind].color;
            }
        }
    }

    [System.Serializable]
    public class SkinData {
        public string _name;
        public Sprite skin;
        public Color color;
    }
}