using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ConvexHull : MonoBehaviour
{
    enum Algorithm
    {
        Greedy,
        Inductive,
        DnCPostProcess,
        DnCPreProcess
    }

    [SerializeField]
    private float waitforseconds = 1f;


    private Algorithm algorithm = Algorithm.Greedy;
    private LineRenderer linerendered;


    private List<Point> polygon;


    public void Reset()
    {
        foreach (Point p in polygon)
        {
            p.SetColor(Color.white);
            //Destroy(p.gameObject);
        }

        polygon.Clear();


        foreach (var ppp in Points.points)
        {
            ppp.color = Color.white;
        }
    }



    public void Start()
    {
        polygon = new List<Point>();
        linerendered = GetComponent<LineRenderer>();
    }


    /*
         A partir d'un probleme P de taille n :
               1 prendre un element d'un ensemble (relatif au probleme) ;
               2 le traiter completement (exactement une seule fois) ;
               3 passer au prochain element.   
         
    */


    public void Greedy()
    {
        Reset();
        StartCoroutine("CoroutineGreedy");

        foreach (var p in polygon)
        {
            p.SetColor(Color.green);
        }

    }

    IEnumerator CoroutineGreedy()
    {
        for (int i = 0; i < Points.quantity - 1; i++)
        {
            for (int j = i + 1; j < Points.quantity; j++)
            {
                int s = 0;
                int a = 0;
                for (int k = 0; k < Points.quantity; k++)
                {
                    var ic = Points.points[i].color;
                    var jc = Points.points[j].color;
                    var kc = Points.points[k].color;
                    Points.points[i].color = Color.blue;
                    Points.points[j].color = Color.cyan;
                    Points.points[k].color = Color.red;
                    yield return new WaitForSeconds(waitforseconds);

                    Points.points[i].color = ic;
                    Points.points[j].color = jc;
                    Points.points[k].color = kc;

                    var lhs = Points.points[k].pos - Points.points[i].pos;
                    var rhs = Points.points[j].pos - Points.points[i].pos;
                    a = (int)Mathf.Sign(Vector3.Cross(lhs, rhs).z);

                    Debug.Log(a);

                    if (s == 0)
                    {
                        s = (int)Mathf.Sign(a);
                    }
                    else if (s != Mathf.Sign(a))
                    {
                        break;
                    }

                }

                if (s == (int)Mathf.Sign(a))
                {
                    polygon.Add(Points.points[i]);
                    Points.points[i].SetColor(Color.green);
                    polygon.Add(Points.points[j]);
                    Points.points[j].SetColor(Color.green);

                    yield return new WaitForSeconds(waitforseconds);
                }


            }
        }


        yield return null;
    }


    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////

    // Incremental
    public void Inductive()
    {
        Reset();
        StartCoroutine("CoroutineInductive");
    }

    public static float t1, t2;
    public static Point pt;
    public static Point cw;
    public static Point ccw;
    public static Point next;
    public static Point rightmost;
    IEnumerator CoroutineInductive()
    {
        // System.Array.Sort(Points.points, new VectorUtil.ClockwiseComparer());

        /*
         * Pour eviter de verier si un point est a l'interieur de CH(k), il
            sut de trier tous les points par ordre croissant par rapport a
            la coordonnee en abscisse. Ainsi tous les points non examines
            seront toujours a l'exterieur de CH(k) a la condition de les
            examiner en ordre.
         * 
           */

        System.Array.Sort(Points.points, delegate (Point user1, Point user2)
        {
            return ((user1.x).CompareTo(user2.x));
        });


        rightmost = MakeTriangle(Points.points[0], Points.points[1], Points.points[2]);
        for (var i = rightmost.next; i != rightmost; i = i.next)
        {
            i.color = Color.green;
        }

        rightmost.color = Color.green;
        yield return new WaitForSeconds(waitforseconds);


        for (int k = 3; k < Points.quantity; k++)
        {
            Points.points[k].color = Color.magenta;

            pt = rightmost;
            cw = Clockwise(rightmost);
            ccw = CounterClockwise(rightmost);

            t1 = Product(Points.points[k], pt, ccw);
            while (cw != ccw)
            {//calibrate
                t2 = Product(Points.points[k], pt, cw);
                if (t1 * t2 >= 0)
                    break;

                Remove(pt);
                pt.color = Color.white;
                pt = cw;
                cw = Clockwise(pt);
            }


            if (cw == ccw)
            {
                cw = pt;
                Debug.Log("Cas limite atteint");
            }
            else if (t1 > 0)
            {
                ccw = pt;

                do
                {
                    next = Clockwise(cw);
                    t2 = Product(next, Points.points[k], cw);
                    if (t2 > 0)
                    {
                        cw = next;
                    }

                }
                while (t2 > 0);

            }
            else
            if (t2 < 0)
            {
                cw = pt;
                do
                {
                    next = CounterClockwise(ccw);
                    t1 = Product(ccw, Points.points[k], next);
                    if (t1 > 0)
                        ccw = next;
                }
                while (t1 > 0);
            }

            //rightmost.color = Color.magenta;
            cw.color = Color.blue;
            ccw.color = Color.red;
            yield return new WaitForSeconds(waitforseconds);


            rightmost = ExpandCH(Points.points[k], cw, ccw);

            foreach (var ppp in Points.points)
            {
                ppp.color = Color.white;
            }

            for (var i = rightmost.next; i != rightmost; i = i.next)
            {
                i.color = Color.green;
            }

            rightmost.color = Color.green;
            yield return new WaitForSeconds(waitforseconds);


        }


        for (var i = rightmost.next; i != rightmost; i = i.next)
        {
            i.color = Color.green;
        }

        rightmost.color = Color.green;


        yield return null;
    }


    Point ExpandCH(Point pt, Point p1, Point p2)
    {                   // Expand a convex hull from a new vertex w.r.t. the
                        // leftmost tangent point and the rightmost tangent point
                        // Point ptr = new Node; ptr->x = x; ptr->y = y;



        pt.next = p1; p1.prev = pt;
        pt.prev = p2; p2.next = pt;
        return pt;
    }

    void Remove(Point ptr)
    {                   // Remove a vertex from a convex hull
        //cout << "Remove(<" << ptr->x << "," << ptr->y << ">)" << endl;
        ptr.prev.next = ptr.next;
        ptr.next.prev = ptr.prev;
    }

    float Product(Point p0, Point p1, Point p2)
    {                   // Calculate the cross product
        float t;
        t = (p1.x - p0.x) * (p2.y - p0.y) - (p1.y - p0.y) * (p2.x - p0.x);

        return t;
    }

    Point Clockwise(Point ptr)
    {                   // Return the next vertex clockwise
        return ptr.next;
    }

    Point CounterClockwise(Point ptr)
    {                   // Return the next vertex counterclockwise
        return ptr.prev;
    }


    // return right most
    Point MakeTriangle(Point p1, Point p2, Point p3)
    {
        Point rightmost = p3;
        if (Product(p3, p2, p1) > 0)
        {
            rightmost.next = p1; p1.prev = rightmost;
            p1.next = p2; p2.prev = p1;
            p2.next = rightmost; rightmost.prev = p2;
        }
        else
        {
            rightmost.next = p2; p2.prev = rightmost;
            p2.next = p1; p1.prev = p2;
            p1.next = rightmost; rightmost.prev = p1;
        }

        return rightmost;

    }




    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////





    public void DnCPostProcess()
    {
        StartCoroutine("CoroutineDnCPostProcess");
    }

    IEnumerator CoroutineDnCPostProcess()
    {
        yield return null;
    }

    public void DnCPreProcess()
    {
        StartCoroutine("CoroutineDnCPreProcess");
    }

    IEnumerator CoroutineDnCPreProcess()
    {
        yield return null;
    }


}
