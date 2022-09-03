using UnityEngine;
using UnityEngine.UI;

public class SpeedIndicator : MonoBehaviour
{
    Rigidbody rb;
    PlayerMovement pm;
    public Text display;
    public RectTransform rt;

    private void Start()
    {
        rb = GameObject.Find("Car(Clone)").GetComponent<Rigidbody>();
        pm = GameObject.Find("Car(Clone)").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        try
        {
            display.text = Mathf.Round(rb.velocity.magnitude * 3.6f).ToString();
        }
        catch (System.NullReferenceException)
        {
            return;
        }
        rt.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, -90, rb.velocity.magnitude / (66 + 2 / 3)));
    }
}
