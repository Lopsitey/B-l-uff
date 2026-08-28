using UnityEngine;
using System.Collections;

public class TmpCameraShifter : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(ShiftCamera());
    }

    public IEnumerator ShiftCamera()
    {
        if (transform.parent == null) yield break;

        while (transform.parent.position.x < 55f)
        {
            Vector3 pos = transform.parent.position;
            pos.x += 0.02f;
            transform.parent.position = pos;
            yield return new WaitForSeconds(0.01f);
        }
    }
    
}
