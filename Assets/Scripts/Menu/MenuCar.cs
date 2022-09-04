using UnityEngine;
using UnityEngine.UI;

public class MenuCar : MonoBehaviour
{
    public Menu menu;
    public Dropdown engineSelector;
    private void Awake()
    {
        int i = PlayerPrefs.HasKey("Car Model") ? PlayerPrefs.GetInt("Car Model") : 0;
        GameObject carModel = GetComponent<CarLoader>().GetCarPrefab(i, 0, true, false);
        Instantiate(carModel, new Vector3(4, 1, -1), Quaternion.Euler(0, 180, 0));
        menu.ChangeCarDescription(carModel.GetComponent<PlayerMovement>());
        GameObject carBody = GameObject.FindGameObjectWithTag("Car Body");
        carBody.GetComponent<Renderer>().material.color = CustomizationEncoder.GetColour(PlayerPrefs.HasKey("Car Colour") ? PlayerPrefs.GetInt("Car Colour") : 0);
        if (!PlayerPrefs.HasKey("Engine")) PlayerPrefs.SetInt("Engine", 0);
        engineSelector.value = PlayerPrefs.GetInt("Engine");
    }

    public void ChangeColour(int index)
    {
        GameObject carBody = GameObject.FindGameObjectWithTag("Car Body");
        carBody.GetComponent<Renderer>().material.color = CustomizationEncoder.GetColour(index);
        PlayerPrefs.SetInt("Car Colour", index);
    }

    public void ChangeCar(int index)
    {
        // Get car model
        Destroy(GameObject.FindGameObjectWithTag("Car"));
        GameObject carModel = GetComponent<CarLoader>().GetCarPrefab(index, 0, true, false);
        carModel.GetComponent<PlayerMovement>().carBody.sharedMaterial.color = CustomizationEncoder.GetColour(PlayerPrefs.HasKey("Car Colour") ? PlayerPrefs.GetInt("Car Colour") : 0);
        Instantiate(carModel, new Vector3(4, 1, -1), Quaternion.Euler(0, 180, 0));
        PlayerPrefs.SetInt("Car Model", index);

        menu.ChangeCarDescription(carModel.GetComponent<PlayerMovement>());

    }

    public void ChangeEngineType(int index)
    {
        Debug.Log(index);
        PlayerPrefs.SetInt("Engine", index);
    }
}
