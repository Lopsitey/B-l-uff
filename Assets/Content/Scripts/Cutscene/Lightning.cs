using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class Lightning : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SpriteRenderer spriteRenderer;

    public AudioClip thunderSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FlashLightning());
    }

    private IEnumerator FlashLightning()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 6f));

            audioSource.pitch = Random.Range(0.7f, 1.2f);
            audioSource.PlayOneShot(thunderSound, Random.Range(0.2f, 0.4f));

            yield return new WaitForSeconds(1f);
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);



            yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));

            spriteRenderer.color = new Color(1, 1, 1, 0);
        }

    }

}
