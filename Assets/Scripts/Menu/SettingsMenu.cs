using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    public GameObject fps;
    public Slider music, sfx;
    public Toggle map, speed, fpsCounter;
    public InputField maxFps;

    [SerializeField] AudioMixer mixer;

    [SerializeField] AudioController ac;
    // Start is called before the first frame update
    void Start()
    {
        music.value = GetSliderValue("Music");
        sfx.value = GetSliderValue("SFX");
        map.isOn = GetToggleValue("Map");
        speed.isOn = GetToggleValue("Gauge");
        fpsCounter.isOn = GetToggleValue("FPS Counter", false);
        maxFps.text = GetInputFieldValueInt("Max FPS").ToString();
    }

    int GetInputFieldValueInt(string key, int default_ = 60)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : default_;
    }

    float GetSliderValue(string key, float default_ = .5f)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : default_;
    }

    bool GetToggleValue(string key, bool default_ = true)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) == 1 : default_;
    }


    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetFloat("Music", music.value);
        PlayerPrefs.SetFloat("SFX", sfx.value);
        PlayerPrefs.SetInt("Map", map.isOn ? 1 : 0);
        PlayerPrefs.SetInt("Gauge", speed.isOn ? 1 : 0);
        PlayerPrefs.SetInt("FPS Counter", fpsCounter.isOn ? 1 : 0);
        fps.SetActive(fpsCounter.isOn);
        mixer.SetFloat("musicVol", Mathf.Lerp(-80, -3, Mathf.Sqrt(Mathf.Sqrt(music.value))));
        mixer.SetFloat("sfxVol", Mathf.Lerp(-80, -3, Mathf.Sqrt(Mathf.Sqrt(sfx.value))));
        
        /*if (ac != null)
        {
            foreach (Sound sound in ac.sounds)
            {
                switch (sound.soundType)
                {
                    case Sound.SoundType.Music:
                        ac.SetVolume(sound.name, music.value);
                        break;
                    case Sound.SoundType.SFX:
                        ac.SetVolume(sound.name, sfx.value);
                        break;
                    default:
                        break;
                }
            }
        }*/
    }
}
