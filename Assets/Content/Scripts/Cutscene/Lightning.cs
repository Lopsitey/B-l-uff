using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(FlashLightning());
    }

    private IEnumerator FlashLightning()
    {
        if (spriteRenderer == null) yield break;

        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 7f));
            if (spriteRenderer != null) spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
            if (spriteRenderer != null) spriteRenderer.color = new Color(1, 1, 1, 0);
        }
    }

}
