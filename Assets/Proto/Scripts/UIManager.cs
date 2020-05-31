using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Prototipo {

    public class DevMenuEvent : UnityEvent<DevOpts> { }

    public struct DevOpts {
        public bool onlyOneJump;
        public float camSpeed;
        public float minJumpbar;
        public float maxJumpbar;
        public float jumpbarModifier;
        public float verticalJump;
        public float horizontalJump;
    }

    public class UIManager : MonoBehaviour {

        public static DevOpts initializer;

        private Gradient powerbarGradient;

        [SerializeField] private Slider powerbarSlide;
        [SerializeField] private Image powerbarFill;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text endScoreText;
        //[SerializeField] private Text positionText;
        [SerializeField] private Text highScore;

        #region Screens
        //screens
        [SerializeField] private GameObject highScoreScreen;
        [SerializeField] private GameObject aboutScreen;
        #endregion

        #region Animations
        [SerializeField] private GameObject introObj;
        private Animation introAnimation;

        [SerializeField] private GameObject endObj;
        #endregion

        #region devMenu
        [Header("DevMenu")]
        [SerializeField] private RectTransform menuContent;
        [SerializeField] private RectTransform menuScroll;

        [SerializeField] private GameObject pauseObj;

        [SerializeField] private Toggle onlyOneJump;
        [SerializeField] private Slider camSpeed;
        [SerializeField] private InputField minJumpbar;
        [SerializeField] private InputField maxJumpbar;
        [SerializeField] private Slider JumpbarModifier;
        [SerializeField] private InputField verticalJump;
        [SerializeField] private InputField horizontalJump;

        public static DevMenuEvent DevMenuEvent { get; private set; }
        #endregion

        private List<int> scores;

        private void Initialize() {
            onlyOneJump.isOn = initializer.onlyOneJump;
            camSpeed.value = initializer.camSpeed;
            minJumpbar.text = initializer.minJumpbar.ToString();
            maxJumpbar.text = initializer.maxJumpbar.ToString();
            JumpbarModifier.value = initializer.jumpbarModifier;
            verticalJump.text = initializer.verticalJump.ToString();
            horizontalJump.text = initializer.horizontalJump.ToString();
        }

        private void Awake() {
            if (DevMenuEvent == null) DevMenuEvent = new DevMenuEvent();
        }

        private void Start() {
            PlayerController.OnJumpbarChangeEvent.AddListener(OnPowerbarChange);
            introObj.GetComponent<GenericAnim>().SetListener(IntroOut);
            introAnimation = introObj.GetComponent<Animation>();
            GameManager.Instance.StateChangeEvent.AddListener(OnStateChange);
            GameManager.ScoreChangeEvent.AddListener(OnScoreChange);
            AdjustMenu();
            powerbarGradient = GetGradient();
            scores = new List<int>();
            highScore.text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }

        private void Update() {
            if (GameManager.Instance.InMenu) {
                IntroInput();
            }
            if (GameManager.Instance.InEndGame) {
                EndGameInput();
            }
        }

        /// <summary>
        /// Callback do evento da barra de força do pulo.
        /// </summary>
        /// <param name="newValue">Valor atual da barra de força.</param>
        /// <param name="powerbarRange">Vetor com valor inicial e final da barra.</param>
        private void OnPowerbarChange(float newValue) {
            powerbarFill.color = powerbarGradient.Evaluate(newValue);
            powerbarSlide.value = newValue;
        }

        private Gradient GetGradient() {
            Vector2 powerbarRange = Vector2.up;
            powerbarGradient = new Gradient();
            GradientColorKey[] colorKey = new GradientColorKey[2];
            colorKey[0].color = Color.red;
            colorKey[0].time = powerbarRange.x;
            colorKey[1].color = Color.green;
            colorKey[1].time = powerbarRange.y;
            GradientAlphaKey[] alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1;
            alphaKey[0].time = powerbarRange.x;
            alphaKey[1].alpha = 1;
            alphaKey[1].time = powerbarRange.y;
            powerbarGradient.SetKeys(colorKey, alphaKey);
            return powerbarGradient;
        }

        private void IntroInput() {
            if (Input.GetKeyUp(KeyCode.Space)) {
                introAnimation.Play();
            }
        }

        private void EndGameInput() {
            if (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                GameSceneManager.Instance.ReloadScene();
            }
        }

        public void IntroOut() {
            Initialize();
            introObj.SetActive(false);
            GameManager.Instance.StartGame();
        }

        public void EndScreen() {
            endObj.SetActive(true);
        }

        private void AdjustMenu() {
            try {
                int childCount = menuContent.childCount;
                float childHeight = menuContent.GetChild(0).GetComponent<RectTransform>().rect.height;
                float spacing = menuContent.GetComponent<VerticalLayoutGroup>().spacing;
                float contentHeight = childHeight * childCount + spacing * (childCount - 1);
                menuContent.sizeDelta = new Vector2(menuContent.rect.width, contentHeight);
            } catch {
                Debug.Log("Não foi possível ajustar a lista de menu");
            }
        }

        public void OnValueChange() {
            PlayerPrefs.SetInt("OnlyOneJump", onlyOneJump.isOn ? 1 : 0);
            PlayerPrefs.SetFloat("CamSpeed", camSpeed.value);
            PlayerPrefs.SetFloat("MinJumpbar", float.Parse(minJumpbar.text));
            PlayerPrefs.SetFloat("MaxJumpbar", float.Parse(maxJumpbar.text));
            PlayerPrefs.SetFloat("JumpbarModifier", JumpbarModifier.value);
            PlayerPrefs.SetFloat("VerticalJump", float.Parse(verticalJump.text));
            PlayerPrefs.SetFloat("HorizontalJump", float.Parse(horizontalJump.text));

            DevMenuEvent?.Invoke(new DevOpts {
                onlyOneJump = onlyOneJump.isOn,
                camSpeed = camSpeed.value,
                minJumpbar = int.Parse(minJumpbar.text),
                maxJumpbar = int.Parse(maxJumpbar.text),
                jumpbarModifier = JumpbarModifier.value,
                verticalJump = int.Parse(verticalJump.text),
                horizontalJump = int.Parse(horizontalJump.text)
            });
        }

        private void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            if (newState == GameManager.GameState.EndGame) {
                EndScreen();
            }

            if (newState == GameManager.GameState.Paused) {
                pauseObj.SetActive(true);
            }

            if (oldState == GameManager.GameState.Paused) {
                pauseObj.SetActive(false);
            }
        }

        private void OnScoreChange(int newScore) {
            scoreText.text = newScore.ToString();
            endScoreText.text = newScore.ToString();
            if (newScore > PlayerPrefs.GetInt("HighScore", 0))
            {
                PlayerPrefs.SetInt("HighScore", newScore);
                highScore.text = newScore.ToString();
                scores.Add(newScore);
            }
        }
    
        #region Buttons Methods and Methods
        //Buttons events
        public void OnStartBTNClick() => IntroOut();
        public void OnHighScoreBTNClick() => SetActiveUI(highScoreScreen.name);
        
        public void OnAboutBTNClick() => SetActiveUI(aboutScreen.name);
        public void OnReturnBTNCLick(GameObject nowScreen)
        {
            nowScreen.SetActive(!nowScreen.activeSelf);
            SetActiveUI(introObj.name);
        }

       //methods
        private void SetActiveUI(string activeUI)
        {
            highScoreScreen.SetActive(activeUI.Equals(highScoreScreen.name));
            aboutScreen.SetActive(activeUI.Equals(aboutScreen.name));
            introObj.SetActive(activeUI.Equals(introObj.name));
        }
        #endregion

        
    }
}