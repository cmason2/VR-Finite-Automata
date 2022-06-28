using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] Button tutorialButton, sandboxButton, challengesButton;
    // Start is called before the first frame update
    void Awake()
    {
        tutorialButton.onClick.AddListener(delegate { ChangeScene("Sandbox"); });
        sandboxButton.onClick.AddListener(delegate { ChangeScene("Sandbox"); });
        challengesButton.onClick.AddListener(ShowChallenges);
    }

    void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void ShowChallenges()
    {
        StartCoroutine(ShowChallengesMenu());
    }

    IEnumerator ShowChallengesMenu()
    {
        tutorialButton.gameObject.SetActive(false);
        sandboxButton.gameObject.SetActive(false);
        challengesButton.gameObject.SetActive(false);

        yield return null;
    }
}
