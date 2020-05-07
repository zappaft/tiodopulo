using Prototipo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

namespace Prototipo {

    public class JumpbarChangeEvent : UnityEvent<float, Vector2> { }

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour, IStatedBehaviour {

        #region PlayerState
        public enum PlayerState {
            Grounded,
            Jumping
        }

        private PlayerState state;
        #endregion

        private GameObject lastCollision;

        private bool canJump;
        [SerializeField] private Vector2 jumpPowerbarRange;
        [SerializeField] private float jumpPowerbarModifier;
        [SerializeField] private float verticalJumpModifier;
        [SerializeField] private float horizontalJumpModifier;
        private bool positiveJumpbarPower;
        private bool shouldJump;

        private float _jumpbarPower;
        [HideInInspector] public float Score { get; private set; }
        [SerializeField] private Text scoreText;

        private float JumpbarPower {
            get => _jumpbarPower;
            set {
                _jumpbarPower = value;
                onJumpbarChangeEvent?.Invoke(value, jumpPowerbarRange);
            }
        }

        private Rigidbody2D rb;

        public static JumpbarChangeEvent onJumpbarChangeEvent;

        private void Awake() {
            if (onJumpbarChangeEvent == null) onJumpbarChangeEvent = new JumpbarChangeEvent();
        }

        private void Start() {
            GameManager.Instance.stateChangeEvent?.AddListener(OnStateChange);
            rb = GetComponent<Rigidbody2D>();
            canJump = true;
        }

        private void Update() {
            if (!GameManager.Instance.InGame) return;
            PlayerInput();
            VerifyPlayerState();
        }

        private void FixedUpdate() {
            if (!GameManager.Instance.InGame) return;
            Jump();
        }

        public void OnStateChange(GameManager.GameState oldState, GameManager.GameState newState) {
            Debug.Log($"playercontroller state changed: {oldState} => {newState}");
        }

        /// <summary>
        /// Responsável por pegar o Input do jogador, em jogo apenas!
        /// Input de Menu ou outra coisa qualquer deve ser feito separadamente.
        /// </summary>
        private void PlayerInput() {
            if (!canJump) return;
            if (Input.GetKeyDown(KeyCode.Space)) if (state == PlayerState.Grounded) StartCoroutine("IncreasePowerbar");
            if (Input.GetKeyUp(KeyCode.Space)) if (state == PlayerState.Grounded) ReleaseJump();
        }

        /// <summary>
        /// Aumenta ou diminui a barra de força do pulo, baseado no limite definido no vetor jumpPowerRange.
        /// </summary>
        /// <returns></returns>
        private IEnumerator IncreasePowerbar() {
            positiveJumpbarPower = true;
            JumpbarPower = jumpPowerbarRange.x;
            while (true) {
                if (JumpbarPower >= jumpPowerbarRange.y) positiveJumpbarPower = false;
                if (JumpbarPower <= jumpPowerbarRange.x) positiveJumpbarPower = true;

                JumpbarPower += jumpPowerbarModifier * Time.deltaTime * (positiveJumpbarPower ? 1 : -1);
                yield return null;
            }
        }

        /// <summary>
        /// Permite que o FixedUpdate execute a física do pulo.
        /// </summary>
        private void ReleaseJump()
        {
            StopCoroutine("IncreasePowerbar");
            shouldJump = true;
            ScoreCount();
        }
        /// <summary>
        /// entrega o valor do score em texto com base em quantos pulos forem bem sucedidos
        /// </summary>
        private void ScoreCount()
        {
            Score++;
            scoreText.text = Score.ToString();
        }

        /// <summary>
        /// Adiciona impulso no rigidbody baseado no verticalJumpModifier, horizontalJumpModifier and JumpbarPower.
        /// </summary>
        private void Jump() {
            if (shouldJump) {
                shouldJump = false;
                rb.AddForce(((Vector2.up * verticalJumpModifier) + (Vector2.right * horizontalJumpModifier)) * JumpbarPower, ForceMode2D.Impulse);
            }
        }

        /// <summary>
        /// Verifica a velocidade do rigidbody para definir o estado do jogador.
        /// </summary>
        private void VerifyPlayerState() {
            state = rb.velocity == Vector2.zero ? PlayerState.Grounded : PlayerState.Jumping;
        }

        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject != lastCollision) 
            {
                lastCollision = collision.gameObject;
                if (GameManager.Instance.OnlyOneJump) canJump = true;
            } 
            else 
            {
                if (GameManager.Instance.OnlyOneJump) canJump = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if (collision.CompareTag("Death")) {
                GameManager.Instance.GameOver();
            }
        }
    }

}