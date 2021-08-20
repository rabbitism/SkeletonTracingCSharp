using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonTracing
{
    public class Rects
    {
        public Rect Head;
        public Rect Tail;

        public void AddRect(int x, int y, int w, int h)
        {
            Rect r = new Rect() { X = x, Y = y, W = w, H = h, Next = null };
            if(Head is null)
            {
                Head = r;
                Tail = r;
            }
            else
            {
                Tail.Next = r;
                Tail = r;
            }
        }
    }
}
