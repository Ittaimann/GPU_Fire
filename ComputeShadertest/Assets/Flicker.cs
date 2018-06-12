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
	
	IEnumerator flick()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(0.03f, 0.1f));
            l.intensity = Random.Range(0.2f, 1.2f);
        }
    }
}
