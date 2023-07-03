using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera[] cameras; // Tablica przechowuj�ca kamery
    public KeyCode switchCameraKey = KeyCode.C; // Klawisz do zmiany kamery
    private int currentCameraIndex = 0; // Indeks obecnie wy�wietlanej kamery

    void Start()
    {
        // W��cz pierwsz� kamer� z tablicy przy starcie
        if (cameras.Length > 0)
        {
            EnableCamera(currentCameraIndex);
        }
    }

    void Update()
    {
        // Sprawd�, czy zosta� naci�ni�ty klawisz zmiany kamery
        if (Input.GetKeyDown(switchCameraKey))
        {
            // Prze��cz na nast�pn� kamer�
            SwitchToNextCamera();
        }
    }

    // Prze��czanie na nast�pn� kamer� w tablicy
    private void SwitchToNextCamera()
    {
        // Wy��cz obecn� kamer�
        cameras[currentCameraIndex].enabled = false;

        // Zwi�ksz indeks kamery o 1 i upewnij si�, �e jest w zakresie tablicy
        currentCameraIndex++;
        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = 0;
        }

        // W��cz now� kamer�
        EnableCamera(currentCameraIndex);
    }

    // W��czanie kamery o okre�lonym indeksie w tablicy
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
