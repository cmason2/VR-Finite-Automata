using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarTwinkle : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Color initialColor;
    private Color finalColor;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        initialColor = new Color(1, 1, 1, 0.2f);
        finalColor = new Color(1, 1, 1, 1.0f);
        StartCoroutine(Twinkle());
    }

    IEnumerator Twinkle()
    {
        while (true)
        {
            float rand = Random.Range(2f, 5f);
            float elapsedTime = 0.0f;

            while (elapsedTime < rand)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            elapsedTime = 0.0f;
            float twinkleTime = 1f;

            while (elapsedTime < twinkleTime)
            {
                elapsedTime += Time.deltaTime;
                meshRenderer.material.color = Color.Lerp(initialColor, finalColor, elapsedTime / twinkleTime);
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            elapsedTime = 0.0f;

            while (elapsedTime < twinkleTime)
            {
                elapsedTime += Time.deltaTime;
                meshRenderer.material.color = Color.Lerp(finalColor, initialColor, elapsedTime / twinkleTime);
                yield return null;
            } 
        }
    }
}
