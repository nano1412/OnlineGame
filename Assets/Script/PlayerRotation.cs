using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Transform player;
    private bool touchStart = false;
    private Vector2 pointA;
    private Vector2 pointB;
    private float middleOfScreen = Screen.width / 2;

    public GameObject rotationCircle;
    public GameObject rotationOuterCircle;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > middleOfScreen)
        {

            pointA = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            rotationCircle.transform.position = pointA;
            rotationOuterCircle.transform.position = pointA;
            rotationCircle.SetActive(true);
            rotationOuterCircle.SetActive(true);

        }
        if (Input.GetMouseButton(0))
        {
            touchStart = true;
            pointB = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        else
        {
            touchStart = false;
        }

    }
    private void FixedUpdate()
    {
        if (touchStart)
        {
            if (Input.mousePosition.x > middleOfScreen)
            {
                Vector2 offset = pointB - pointA;
                Vector2 direction = Vector2.ClampMagnitude(offset, 1.0f);
                rotateCharacter(direction);
                rotationCircle.transform.position = new Vector2(pointA.x + offset.x, pointA.y + offset.y);
            }
        }
        else
        {
            rotationCircle.SetActive(false);
            rotationOuterCircle.SetActive(false);
        }

    }

    void rotateCharacter(Vector2 rotation)
    {
        player.transform.rotation = quaternion.Euler(0, 0, Mathf.Atan2(rotation.x, rotation.y) * -1);
    }
}
