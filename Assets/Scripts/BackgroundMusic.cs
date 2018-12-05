using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    static BackgroundMusic instance;
    AudioSource source;
    // Use this for initialization
    void Start()
    {
        instance = this;
        source = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.transform.position;
    }

    public static AudioClip clip {
        get { return instance.source.clip; }
        set { instance.source.clip = value; }
    }
    public static float volume {
        get { return instance.source.volume; }
        set { instance.source.volume = value; }
    }
    public static bool loop {
        get { return instance.source.loop; }
        set { instance.source.loop = value; }
    }
    public static bool mute {
        get { return instance.source.mute; }
        set { instance.source.mute = value; }
    }
    public static void Play() {
        instance.source.Play();
    }


    public static void Pause() {
        instance.source.Pause();
    }

    public static void UnPause() {
        instance.source.UnPause();
    }

    public static void Stop() {
        instance.source.Stop();
    }
}
