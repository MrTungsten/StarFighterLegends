using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeMusicScript : MonoBehaviour
{

    [SerializeField] private AudioClip washingOverdriveClip;
    [SerializeField] private AudioClip spaceFighterClip;
    private static ThemeMusicScript Instance;
    private AudioSource themeMusicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        themeMusicSource = GetComponent<AudioSource>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.Equals("VideoScreen"))
        {
            Destroy(gameObject);
        }
        else if (currentSceneName.Equals("Main Menu") || currentSceneName.Equals("Instructions"))
        {
            if (themeMusicSource.clip != washingOverdriveClip)
            {
                themeMusicSource.clip = washingOverdriveClip;
                themeMusicSource.pitch = 1.25f;
                themeMusicSource.Play();
            }
        }
        else
        {
            if (themeMusicSource.clip != spaceFighterClip)
            {
                themeMusicSource.clip = spaceFighterClip;
                themeMusicSource.pitch = 1f;
                themeMusicSource.Play();
            }
        }
    }

}
