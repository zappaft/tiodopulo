using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreList : MonoBehaviour
{
    [SerializeField] private Text positionText;
    [SerializeField] private Text scoreText;

    public void OnScoreCountInitialize(int position, int score)
    {
        positionText.text = position + "st";
        scoreText.text = score + " points";
    }
}
