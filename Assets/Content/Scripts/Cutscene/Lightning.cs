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
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 7f));
            spriteRenderer.color = new Color(1, 1, 1, 0.3f);
            yield return new WaitForSeconds(Random.Range(0.2f, 0.6f));
            spriteRenderer.color = new Color(1, 1, 1, 0);
        }

    }

}
