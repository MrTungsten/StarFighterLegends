using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoPlayerManager : MonoBehaviour
{
    
    [SerializeField] private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "StarFighterLegendsIntro.mp4");
        videoPlayer.loopPointReached += EndReached;
        StartCoroutine(VideoDelay(3));
    }

    private void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene("Main Menu");
    }

    private IEnumerator VideoDelay(float time)
    {
        yield return new WaitForSeconds(time);
        videoPlayer.Play();
    }

}
