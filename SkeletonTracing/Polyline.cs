using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonTracing
{
    public class Polyline
    {
        public Point Head;
        public Point Tail;
        public Polyline Prev;
        public Polyline Next;
        public int Size;

        public static void ReversePolyline(Polyline q)
        {
            if(q is null || q.Size < 2)
            {
                return;
            }
            q.Tail.Next = q.Head;
            Point it0 = q.Head;
            Point it1 = it0.Next;
            Point it2 = it1.Next;
            for (int i = 0; i < q.Size - 1; i++) 
            {
                it1.Next = it0;
                it0 = it1;
                it1 = it2;
                it2 = it2.Next;
            }

            Point qHead = q.Head;
            q.Head = q.Tail;
            q.Tail = qHead;
            q.Tail.Next = null;
        }

        public static void CatTailPolyline(Polyline q0, Polyline q1)
        {
            if (q1 is null) return;
            if(q0 is null)
            {
                q0 = new Polyline();
            }
            if(q0.Head is null)
            {
                q0.Head = q1.Head;
                q0.Tail = q1.Tail;
                return;
            }
            q0.Tail.Next = q1.Head;
            q0.Tail = q1.Tail;
            q0.Size += q1.Size;
            q0.Tail.Next = null;
        }

        public static void CatHeadPolyline(Polyline q0, Polyline q1)
        {
            if (q1 is null) return;
            if(q0 is null)
            {
                q0 = new Polyline();
            }
            if (q1.Head is null) return;
            if(q0.Head is null)
            {
                q0.Head = q1.Head;
                q0.Tail = q1.Tail;
                return;
            }
            q1.Tail.Next = q0.Head;
            q0.Head = q1.Head;
            q0.Size += q1.Size;
            q0.Tail.Next = null;
        }

        public static void AddPointToPolyline(Polyline q, int x, int y)
        {
            Point p = new Point() { X = x, Y = y, Next = null };
            if(q.Head is null)
            {
                q.Head = p;
                q.Tail = p;
            }
            else
            {
                q.Tail.Next = p;
                q.Tail = p;
            }
            q.Size++;
        }

        public static Polyline PrependPolyline(Polyline q0, Polyline q1)
        {
            if(q0 is null)
            {
                return q1;
            }
            q1.Next = q0;
            q0.Prev = q1;
            return q1;
        }
    }
}
