using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    public Text text;


    // Update is called once per frame
    void Update()
    {
        text.text = ((int)(1 / Time.unscaledDeltaTime)).ToString();
    }
}
