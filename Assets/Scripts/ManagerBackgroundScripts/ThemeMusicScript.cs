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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex == 0)
        {
            Destroy(gameObject);
        }
        else if (currentSceneIndex == 1 || currentSceneIndex == 2)
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
