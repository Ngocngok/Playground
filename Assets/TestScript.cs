using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    private GameObject[] objects;

    int index = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            index++;
            if (index == objects.Length)
                index = 0;

            foreach (GameObject _object in objects)
            {
                _object.SetActive(false);
                objects[index].SetActive(true);
            }
        }
    }
}
