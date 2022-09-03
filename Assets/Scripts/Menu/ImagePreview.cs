using UnityEngine;
using UnityEngine.UI;

public class ImagePreview : MonoBehaviour
{
    Material image;
    public Image minimap;
    public Sprite[] sprites, minimaps;
    public void SetImage(int index)
    {
        image = GameObject.Find("tv").GetComponent<Renderer>().materials[1];
        image.SetTexture("_BaseMap", sprites[index].texture);
        minimap.sprite = minimaps[index];
    }
}
