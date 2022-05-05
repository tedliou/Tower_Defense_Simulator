using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Build());
    }

    private IEnumerator Build()
    {
        var scale = transform.localScale;
        scale.y = 0;
        transform.localScale = scale;
        var targetScaleY = Random.Range(1, 6);

        while (transform.localScale.y < targetScaleY)
        {
            scale.y += .1f;
            transform.localScale = scale;
            yield return new WaitForSecondsRealtime(.02f);
        }
    }
}
