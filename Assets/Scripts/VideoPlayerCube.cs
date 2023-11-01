using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerCube : MonoBehaviour
{
    public VideoClip videoClip; // Assign your video clip in the Unity Inspector
    private VideoPlayer videoPlayer;
    private bool isVideoPlaying = false;

    void Start()
    {
        // Create a VideoPlayer component and attach it to the cube
        videoPlayer = gameObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = true; // Optional: Loop the video.

        // Create a RenderTexture to display the video on the cube's side
        RenderTexture renderTexture = new RenderTexture(1024, 1024, 16);
        videoPlayer.targetTexture = renderTexture;

        // Disable the MeshRenderer initially to make the cube invisible
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!isVideoPlaying)
            {
                videoPlayer.clip = videoClip;
                videoPlayer.Play();
                isVideoPlaying = true;

                // Enable the MeshRenderer when the 'P' key is pressed to make the cube visible
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.enabled = true;
            }
            else
            {
                videoPlayer.Pause();
                isVideoPlaying = false;

                // Disable the MeshRenderer when 'P' key is pressed again to hide the cube
                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
            }
        }
    }
}
