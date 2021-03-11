using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Usado por ejemplo en las notificaciones del Journal y Social mensajes sin leer

public class PopScaleIntervalEffect : MonoBehaviour
{

    public float Speed = 0.0035f;
    public float ToScale = 1.075f;
    public float SleepTime = 5;

    private float NextAnimTime = 0;

    private int CurrentStep = 1;

    public bool AddScaleMethod = false;

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (NextAnimTime < Time.time)
            {
                StartCoroutine(DoPopScaleAnim());
                NextAnimTime = Time.time + SleepTime;
            }
        }
    }

    // TODO: REEMPLAZAR POR DOTWEEN

    IEnumerator DoPopScaleAnim()
    {
        Vector3 originalScale = transform.localScale;

        Vector3 finalScale;
        finalScale = new Vector3(ToScale, ToScale, ToScale);


        int cStep = 1;

        while (cStep != 3)
        {
            // Step 1, scale from originalScale to finalScale
            while (transform.localScale.x < finalScale.x)
            {
                float calcCord = transform.localScale.x + Speed;
                Vector3 nextScale;
                nextScale = new Vector3(calcCord, calcCord, calcCord);
                transform.localScale = nextScale;
                yield return null;
            }

            // Step 2, scale from finalScale to originalScale
            while (transform.localScale.x > originalScale.x)
            {
                float calcCord = transform.localScale.x - Speed;
                Vector3 nextScale;
                nextScale = new Vector3(calcCord, calcCord, calcCord);
                transform.localScale = nextScale;
                yield return null;
            }

            cStep++;
            yield return null;
        }

        // Fix localScale after anim
        //if (GetComponent<Transform>().localScale.x != originalScale.x)
        //{
        transform.localScale = originalScale;
        //}
    }
}
