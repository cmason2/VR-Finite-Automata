using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject challengeMenu;
    [SerializeField] Animator challengeMenuAnimator;
    [SerializeField] Button tutorialButton, sandboxButton, challengesButton, backButton;
    private Animator tutorialAnimator, sandboxAnimator, challengesAnimator;
    private Camera mainCamera;
    // Start is called before the first frame update
    void Awake()
    {
        tutorialAnimator = tutorialButton.GetComponent<Animator>();
        sandboxAnimator = sandboxButton.GetComponent<Animator>();
        challengesAnimator = challengesButton.GetComponent<Animator>();
        mainCamera = FindObjectOfType<Camera>();

        tutorialButton.onClick.AddListener(delegate { ChangeScene("Sandbox"); });
        sandboxButton.onClick.AddListener(delegate { ChangeScene("Sandbox"); });
        challengesButton.onClick.AddListener(ShowChallenges);
        backButton.onClick.AddListener(ShowMenu);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, transform.position.z);
    }

    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void ShowChallenges()
    {
        StartCoroutine(ShowChallengesMenu());
    }

    void ShowMenu()
    {
        StartCoroutine(ShowMainMenu());
    }

    IEnumerator ShowChallengesMenu()
    {
        tutorialButton.enabled = false;
        sandboxButton.enabled = false;
        challengesButton.enabled = false;

        tutorialAnimator.SetTrigger("Hide");
        sandboxAnimator.SetTrigger("Hide");
        challengesAnimator.SetTrigger("Hide");

        yield return new WaitForSeconds(0.8f);

        challengeMenu.SetActive(true);
        challengeMenuAnimator.SetTrigger("Show");
    }

    IEnumerator ShowMainMenu()
    {
        challengeMenuAnimator.SetTrigger("Hide");

        yield return new WaitForSeconds(0.6f);

        tutorialAnimator.SetTrigger("Show");
        sandboxAnimator.SetTrigger("Show");
        challengesAnimator.SetTrigger("Show");

        yield return new WaitForSeconds(0.6f);

        tutorialButton.enabled = true;
        sandboxButton.enabled = true;
        challengesButton.enabled = true;
    }
}
