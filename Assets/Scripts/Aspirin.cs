using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class Aspirin : Collectible
{
    AudioSource source;
    public AudioClip clip;
    // Use this for initialization
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

   
    protected override void OnCollect(PlayerControl collector)
    {
        destroyDelay = clip.length;
        source.PlayOneShot(clip);
        collector.CollectedAspirin++;
    }


    
}
