using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ChallengeItem : MonoBehaviour
{
    [SerializeField] Button itemButton;
    [SerializeField] TMP_Text languageText;
    [SerializeField] Image difficultyImage;
    [SerializeField] GameObject clearedImage;
    [SerializeField] GameObject optimalImage;
    private int challengeNumber;

    private void Start()
    {
        itemButton.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        Challenges.SetCurrentChallenge(challengeNumber);
        SceneManager.LoadScene("Challenge");
    }

    public void SetChallenge(Challenge challenge)
    {
        challengeNumber = challenge.number;
        gameObject.name = "Challenge " + challengeNumber;
        languageText.text = challenge.description;
        if (challenge.completed)
            clearedImage.SetActive(true);
        if (challenge.minimal)
            optimalImage.SetActive(true);
        switch (challenge.difficulty)
        {
            case 0:
                difficultyImage.color = Color.green;
                break;
            case 1:
                difficultyImage.color = Color.yellow;
                break;
            case 2:
                difficultyImage.color = Color.red;
                break;
        }
    }
}
