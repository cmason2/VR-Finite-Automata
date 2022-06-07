using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEdge : MonoBehaviour
{

    private AudioSource audioSource;
    [SerializeField] AudioClip audioclip;

    private void Start()
    {
        audioSource = FindObjectOfType<AudioSource>();
    }

    public void DestroyEdge()
    {
        audioSource.clip = audioclip;
        audioSource.Play();
        Destroy(gameObject);
    }
}
