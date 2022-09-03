using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public int scene;
    public Slider music, sfx, progressBar;
    public Toggle map, speed, fpsCounter;
    public InputField maxFps;
    public GameObject loadingText;
    public HighscoreManager hm;
    // Start is called before the first frame update
    public void OnClick()
    {
        if (loadingText != null) loadingText.SetActive(true);
        if (maxFps != null) PlayerPrefs.SetInt("Max FPS", int.Parse(maxFps.text));
        PlayerPrefs.SetFloat("Music", music.value);
        PlayerPrefs.SetFloat("Sfx", sfx.value);
        PlayerPrefs.SetInt("Map", map.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Gauge", speed.isOn ? 1 : 0);
        PlayerPrefs.SetInt("FPS Counter", fpsCounter.isOn ? 1 : 0);

        hm.UpdateHighscores();
        StartCoroutine(LoadScene());
    }

    public void NextLevel()
    {
        int index = (SceneManager.GetActiveScene().buildIndex + 1) % Constants.numberOfTracks;
        if (index == 0) index = Constants.numberOfTracks;
        scene = index;
        OnClick();
    }

    public void SetSceneNumber(int index)
    {
        scene = index;
    }

    IEnumerator LoadScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(scene);
        while (!loading.isDone)
        {
            float progress = Mathf.Clamp01(loading.progress / .9f);
            if (progressBar != null) progressBar.value = progress;
            yield return null;
        }
    }

}