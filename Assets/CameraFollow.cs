using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera[] cameras; // Tablica przechowuj¹ca kamery
    public KeyCode switchCameraKey = KeyCode.C; // Klawisz do zmiany kamery
    private int currentCameraIndex = 0; // Indeks obecnie wyœwietlanej kamery

    void Start()
    {
        // W³¹cz pierwsz¹ kamerê z tablicy przy starcie
        if (cameras.Length > 0)
        {
            EnableCamera(currentCameraIndex);
        }
    }

    void Update()
    {
        // SprawdŸ, czy zosta³ naciœniêty klawisz zmiany kamery
        if (Input.GetKeyDown(switchCameraKey))
        {
            // Prze³¹cz na nastêpn¹ kamerê
            SwitchToNextCamera();
        }
    }

    // Prze³¹czanie na nastêpn¹ kamerê w tablicy
    private void SwitchToNextCamera()
    {
        // Wy³¹cz obecn¹ kamerê
        cameras[currentCameraIndex].enabled = false;

        // Zwiêksz indeks kamery o 1 i upewnij siê, ¿e jest w zakresie tablicy
        currentCameraIndex++;
        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = 0;
        }

        // W³¹cz now¹ kamerê
        EnableCamera(currentCameraIndex);
    }

    // W³¹czanie kamery o okreœlonym indeksie w tablicy
    private void EnableCamera(int index)
    {
        cameras[index].enabled = true;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i != index)
            {
                cameras[i].enabled = false;
            }
        }
    }
}
