using UnityEngine;

public class CustomizationEncoder : MonoBehaviour
{
    public static Color GetColour(int index)
    {
        Color[] colours = new Color[15] {
            Color.red,
            Color.blue,
            new Color(.4f, 1, 0),
            new Color(1, .89f, 0),
            new Color(1f, .6f, 0),
            new Color(.6f, 0, 1),
            new Color(0f, .69f, 1),
            new Color(1f, .4f, .9f),
            Color.white,
            new Color(.34f, .93f, .63f),
            new Color(.5f, 0f, 0f),
            new Color(0, .5f, 0),
            new Color(.5f, .25f, 0),
            new Color(.3f, .3f, .3f),
            new Color(.05f, .05f, .05f)
            };
        try
        {
            return colours[index];
        }
        catch (System.IndexOutOfRangeException)
        {
            return Color.red;
        }
    }

}
