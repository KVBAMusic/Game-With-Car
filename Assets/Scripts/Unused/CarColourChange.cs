using UnityEngine;

public class CarColourChange : MonoBehaviour
{
    Material material;
    Color c1, c2;
    float startTick, endTick = -1f;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        c1 = material.color;
        c2 = material.color;
    }

    void ChangeColours()
    {

        while (Time.time <= endTick)
        {

        }

    }

    void Update()
    {
        if (Time.time > endTick)
        {
            c1 = c2;
            startTick = Time.time;
            endTick = startTick + 1;
            c2 = new Color(Random.value, Random.value, Random.value);
        }
        else
        {
            material.color = Color.Lerp(c1, c2, Time.time - startTick / endTick - startTick);
        }
    }
}
