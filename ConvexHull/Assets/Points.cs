using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Points : MonoBehaviour
{
    [SerializeField]
    private GameObject pointPrefab;

    [SerializeField]
    private Vector2 spreadx = new Vector2(0,4);
    [SerializeField]
    private Vector2 spready = new Vector2(0, 4);

    [SerializeField]
    private int _quantity = 100;
    public static int quantity = -1;

    public static Point[] points;


    public void Generate()
    {

        for (int i = 0; i < quantity; i++)
        {
            if (points[i] != null)
                Destroy(points[i].gameObject);
            points[i] = null;

        }


        quantity = _quantity;
        points = new Point[quantity];


        for (int i = 0; i < quantity; i++)
        {
            GameObject point = Instantiate(pointPrefab, transform);
            point.transform.localPosition = new Vector3(Random.Range(spreadx.x, spreadx.y), Random.Range(spready.x, spready.y));
            points[i] = (point.GetComponent<Point>());
        }

    }


}
