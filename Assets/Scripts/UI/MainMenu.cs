using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject challengeMenu;
    [SerializeField] Animator challengeMenuAnimator;
    [SerializeField] Button tutorialButton, sandboxButton, challengesButton, backButton;
    [SerializeField] GameObject challengeContainer;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] FadeOverlay fadeOverlay;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();

        tutorialButton.onClick.AddListener(delegate { ChangeScene("Tutorial"); });
        sandboxButton.onClick.AddListener(delegate { ChangeScene("Sandbox"); });
        challengesButton.onClick.AddListener(ShowChallenges);
        backButton.onClick.AddListener(ShowMenu);

        challengeMenu.transform.localScale = Vector3.zero;

        PopulateChallenges();
    }

    private void Update()
    {
        // Move menu to user height
        transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z);
    }

    void ChangeScene(string sceneName)
    {
        fadeOverlay.FadeToBlack().OnComplete(() => SceneManager.LoadScene(sceneName));
    }

    void ShowChallenges()
    {
        challengeMenu.SetActive(true);
        Sequence seq = DOTween.Sequence();
        seq.Append(tutorialButton.transform.DOScale(0f, 1.0f));
        seq.Join(sandboxButton.transform.DOScale(0f, 1.0f));
        seq.Join(challengesButton.transform.DOScale(0f, 1.0f));
        seq.Append(challengeMenu.transform.DOScale(1.0f, 1.0f).SetEase(Ease.OutBack));
    }

    void ShowMenu()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(challengeMenu.transform.DOScale(0f, 1.0f));
        seq.Append(sandboxButton.transform.DOScale(1.0f, 1.0f).SetEase(Ease.OutBack));
        seq.Join(tutorialButton.transform.DOScale(1.0f, 1.0f).SetEase(Ease.OutBack));
        seq.Join(challengesButton.transform.DOScale(1.0f, 1.0f).SetEase(Ease.OutBack));
    }

    void PopulateChallenges()
    {
        foreach (Challenge challenge in Challenges.challenges)
        {
            GameObject challengeItem = Instantiate(itemPrefab, challengeContainer.transform);
            ChallengeItem challengeItemScript = challengeItem.GetComponent<ChallengeItem>();
            challengeItemScript.SetChallenge(challenge);
        }
    }
}
