using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ChallengeItem : MonoBehaviour
{
    [SerializeField] Button itemButton;
    [SerializeField] TMP_Text languageText;
    [SerializeField] Image difficultyImage;
    [SerializeField] GameObject clearedImage;
    [SerializeField] GameObject optimalImage;
    [SerializeField] AudioClip buttonPressClip;
    private AudioSource audioSource;
    private FadeOverlay fadeOverlay;
    private int challengeNumber;

    private void Start()
    {
        fadeOverlay = FindObjectOfType<FadeOverlay>();
        audioSource = FindObjectOfType<AudioSource>();
        itemButton.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        audioSource.clip = buttonPressClip;
        audioSource.Play();
        Challenges.SetCurrentChallenge(challengeNumber);
        fadeOverlay.FadeToBlack().OnComplete(() => SceneManager.LoadScene("Challenge"));
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
