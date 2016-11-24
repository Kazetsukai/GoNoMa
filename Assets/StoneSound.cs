using UnityEngine;
using System.Collections;

public class StoneSound : MonoBehaviour {

    public AudioClip[] StoneSounds;

	// Use this for initialization
	void Start () {
        int soundIndex = Random.Range(0, StoneSounds.Length);

        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = StoneSounds[soundIndex];
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
