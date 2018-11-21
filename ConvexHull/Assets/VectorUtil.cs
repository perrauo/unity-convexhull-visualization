using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class VectorUtil
{

    public class ClockwiseComparer : IComparer<Point>
    {
        public int Compare(Point v1, Point v2)
        {
            if (v1.x >= 0)
            {
                if (v2.x < 0)
                {
                    return -1;
                }
                return -Comparer<float>.Default.Compare(v1.y, v2.y);
            }
            else
            {
                if (v2.x >= 0)
                {
                    return 1;
                }
                return Comparer<float>.Default.Compare(v1.y, v2.y);
            }
        }
    }




}

