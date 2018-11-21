using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    MeshRenderer meshrenderer;

    public float x { get { return transform.position.x; } }
    public float y { get { return transform.position.y; } }

    public Vector3 pos { get { return transform.position; } set { transform.position = value; } }
    public Color color { get { return meshrenderer.material.color; } set { meshrenderer.material.color = value; } }

    public Point prev = null;
    public Point next = null;


    public void Start()
    {
        meshrenderer = GetComponent<MeshRenderer>();
    }


    public void SetColor(Color color)
    {
        meshrenderer.material.color = color;
    }

}
