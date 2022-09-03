using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] GameObject dot;

    public Image image;
    public Sprite minimapPng, first;
    public Vector2 offset;

    GameObject[] dots;
    PositionTracker[] trackers;

    Constants.GameMode gameMode;
    GameObject firstIndicator;
    RectTransform firstRc;
    // Start is called before the first frame update
    void Start()
    {
        gameMode = (Constants.GameMode)PlayerPrefs.GetInt("Game Mode");
        dots = new GameObject[8];
        image.sprite = minimapPng;
        trackers = FindObjectsOfType<PositionTracker>();
        for (int i = 0; i < trackers.Length; i++)
        {
            var car = trackers[i];
            Debug.Log(car);
            var d = Instantiate(dot, transform);
            d.GetComponent<Image>().color = car.GetComponent<PlayerMovement>().carBody.material.color;
            dots[i] = d;
        }
        if (gameMode == Constants.GameMode.SingleRace)
        {
            firstIndicator = Instantiate(dot, transform);
            firstIndicator.GetComponent<Image>().sprite = first;
            firstRc = firstIndicator.GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < trackers.Length; i++)
        {
            var car = trackers[i];
            RectTransform rc = dots[i].GetComponent<RectTransform>();
            rc.anchoredPosition3D = Convert(car.transform.position);
            rc.SetSiblingIndex(trackers.Length - car.place);
            if (car.gameObject.name == "Car(Clone)") rc.localScale = new Vector3(1.5f, 1.5f, 1);
            if (car.place == 1 && gameMode == Constants.GameMode.SingleRace)
            {
                firstRc.anchoredPosition3D = Convert(car.transform.position);
                firstRc.SetAsLastSibling();
            }
        }
        image.transform.SetAsFirstSibling();
    }

    private Vector3 Convert(Vector3 pos)
    {
        return new Vector3(pos.x / 5 + 126.2f, pos.z / 5 + 102, 0);
    }

}
