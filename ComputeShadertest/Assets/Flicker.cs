using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flicker : MonoBehaviour {


    Light l;

	// Use this for initialization
	void Start () {
        l = GetComponent<Light>();
        StartCoroutine(flick());
	}
    private void Update()
    {
        l.intensity = Mathf.Lerp(.2f, 1.2f, (Mathf.Sin(Time.time) + 1 ) /2);

    }

    IEnumerator flick()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
            transform.position = new Vector3(0, Random.Range(.3f, .4f),0);
        }
    }
}
