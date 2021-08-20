using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace SkeletonTracing
{

    public class SkeletonTracer
    {
        private int _w;
        private int _h;
        private int[] _im;
        private Rects _rects;

        private bool ThinningZsIteration(int iter)
        {
            bool diff = false;
            for(int i = 1; i< _h-1; i++)
            {
                for(int j = 1; j < _w-1; j++)
                {
                    int p2 = _im[(i - 1) * _w + j] & 1;
                    int p3 = _im[(i - 1) * _w + j + 1] & 1;
                    int p4 = _im[(i) * _w + j + 1] & 1;
                    int p5 = _im[(i + 1) * _w + j + 1] & 1;
                    int p6 = _im[(i + 1) * _w + j] & 1;
                    int p7 = _im[(i + 1) * _w + j - 1] & 1;
                    int p8 = _im[(i) * _w + j - 1] & 1;
                    int p9 = _im[(i - 1) * _w + j - 1] & 1;
                    int A = ((p2 == 0 && p3 == 1) ? 1 : 0) + ((p3 == 0 && p4 == 1) ? 1 : 0) +
                 ((p4 == 0 && p5 == 1) ? 1 : 0) + ((p5 == 0 && p6 == 1) ? 1 : 0) +
                 ((p6 == 0 && p7 == 1) ? 1 : 0) + ((p7 == 0 && p8 == 1) ? 1 : 0) +
                 ((p8 == 0 && p9 == 1) ? 1 : 0) + ((p9 == 0 && p2 == 1) ? 1 : 0);
                    int B = p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;
                    int m1 = iter == 0 ? (p2 * p4 * p6) : (p2 * p4 * p8);
                    int m2 = iter == 0 ? (p4 * p6 * p8) : (p2 * p6 * p8);

                    if (A == 1 && (B >= 2 && B <= 6) && m1 == 0 && m2 == 0)
                    {
                        _im[i * _w + j] |= 2;
                    }
                }
            }
            for(int i = 0; i < _h*_w; i++)
            {
                int marker = _im[i] >> 1;
                int old = _im[i] & 1;
                _im[i] = old & (marker == 0 ? 1 : 0);
                if((!diff) && (_im[i] != old))
                {
                    diff = true;
                }
            }
            return diff;
        }

        private void ThinningZs()
        {
            bool diff = true;
            do
            {
                diff &= ThinningZsIteration(0);
                diff &= ThinningZsIteration(1);
            }
            while (diff);
        }

        private bool NotEmpty(int x, int y, int w, int h)
        {
            for(int i = y; i< y+h; i++)
            {
                for(int j = x; j< x+w; j++)
                {
                    if (_im[i * _w + j] > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool MergeImpl(Polyline c0, Polyline c1i, int sx, int isv, int mode)
        {
            bool b0 = (mode >> 1 & 1) > 0;
            bool b1 = (mode >> 0 & 1) > 0;
            Polyline c0j = null;
            int md = 4;
            Point p1 = b1 ? c1i.Head : c1i.Tail;
            if (Math.Abs((isv > 0 ? p1.Y : p1.X) - 1) > 0)
            {
                return false;
            }
            Polyline it = c0;
            while (it != null)
            {
                Point p0 = b0 ? it.Head : it.Tail;
                if (Math.Abs((isv > 0 ? p0.Y : p0.X) - sx) > 1) 
                {
                    it = it.Next;
                    continue;
                }
                int d = Math.Abs(isv > 0 ? p0.X : p0.Y - isv > 0 ? p1.X : p1.Y);
                if(d < md)
                {
                    c0j = it;
                    md = d;
                }
                it = it.Next;

            }

            if(c0j != null)
            {
                if(b0&& b1)
                {
                    Polyline.ReversePolyline(c1i);
                    Polyline.CatHeadPolyline(c0j, c1i);
                }
                else if(!b0 && b1)
                {
                    Polyline.CatTailPolyline(c0j, c1i);
                }
                else if (b0 && !b1)
                {
                    Polyline.CatHeadPolyline(c0j, c1i);
                }
                else
                {
                    Polyline.ReversePolyline(c1i);
                    Polyline.CatTailPolyline(c0j, c1i);
                }
                return true;
            }
            return false;
        }

        private Polyline MergeFrags(Polyline c0, Polyline c1, int sx, int dr)
        {
            if(c0 is null)
            {
                return c1;
            }
            if(c1 is null)
            {
                return c0;
            }
            Polyline it = c1;
            while(it != null){
                Polyline temp = it.Next;
                if (dr == (int)Orientation.Horizontal)
                {
                    if (MergeImpl(c0, it, sx, 0, 1)) goto rem;
                    if (MergeImpl(c0, it, sx, 0, 3)) goto rem;
                    if (MergeImpl(c0, it, sx, 0, 0)) goto rem;
                    if (MergeImpl(c0, it, sx, 0, 2)) goto rem;
                }
                else
                {
                    if (MergeImpl(c0, it, sx, 1, 1)) goto rem;
                    if (MergeImpl(c0, it, sx, 1, 3)) goto rem;
                    if (MergeImpl(c0, it, sx, 1, 0)) goto rem;
                    if (MergeImpl(c0, it, sx, 1, 2)) goto rem;
                }

                goto next;
                rem:
                if(it.Prev is null)
                {
                    c1 = it.Next;
                    if (it.Next != null)
                    {
                        it.Next.Prev = null;
                    }
                }
                else
                {
                    it.Prev.Next = it.Next;
                    if(it.Next is null)
                    {
                        it.Next.Prev = it.Prev;
                    }
                }
                it = null;
                next:
                it = temp;
            }

            it = c1;
            while(it != null)
            {
                Polyline temp = it.Next;
                it.Prev = null;
                it.Next = null;
                c0 = Polyline.PrependPolyline(c0, it);
                it = temp;
            }
            return c0;
        }

        private Polyline ChunkToFrags(int x, int y, int w, int h)
        {
            Polyline frags = null;
            int fsize = 0;
            bool on = false;
            int li = -1, lj = -1;
            for(int k = 0; k< h+h+w+w-4; k++)
            {
                int i, j;
                if (k < w)
                {
                    i = y + 0; j = x + k;
                }
                else if (k < w + h - 1)
                {
                    i = y + k - w + 1; j = x + w - 1;
                }
                else if (k < w + h + w - 2)
                {
                    i = y + h - 1; j = x + w - (k - w - h + 3);
                }
                else
                {
                    i = y + h - (k - w - h - w + 4); j = x + 0;
                }
                if (_im[i * _w + j]>0)
                { // found an outgoing pixel
                    if (!on)
                    {     // left side of stroke
                        on = true;
                        Polyline f = new Polyline();
                        Polyline.AddPointToPolyline(f, j, i);
                        Polyline.AddPointToPolyline(f, x + w / 2, y + h / 2);
                        frags = Polyline.PrependPolyline(frags, f);
                        fsize++;
                    }
                }
                else
                {
                    if (on)
                    {// right side of stroke, average to get center of stroke
                        frags.Head.X = (frags.Head.X + lj) / 2;
                        frags.Head.Y = (frags.Head.Y + li) / 2;
                        on = false;
                    }
                }
                li = i;
                lj = j;
            }

            if(fsize == 2)
            {
                Polyline f = new Polyline();
                Polyline.AddPointToPolyline(f, frags.Head.X, frags.Head.Y);
                Polyline.AddPointToPolyline(f, frags.Next.Head.X, frags.Next.Head.Y);
                frags = f;
            }else if (fsize > 2)
            {
                int ms = 0;
                int mi = -1;
                int mj = -1;
                // use convolution to find brightest blob
                for (int i = y + 1; i < y + h - 1; i++)
                {
                    for (int j = x + 1; j < x + w - 1; j++)
                    {
                        int s =
                          (_im[i * _w - _w + j - 1]) + (_im[i * _w - _w + j]) + (_im[i * _w - _w + j - 1 + 1]) +
                          (_im[i * _w + j - 1]) + (_im[i * _w + j]) + (_im[i * _w + j + 1]) +
                          (_im[i * _w + _w + j - 1]) + (_im[i * _w + _w + j]) + (_im[i * _w + _w + j + 1]);
                        if (s > ms)
                        {
                            mi = i;
                            mj = j;
                            ms = s;
                        }
                        else if (s == ms && Math.Abs(j - (x + w / 2)) + Math.Abs(i - (y + h / 2)) < Math.Abs(mj - (x + w / 2)) + Math.Abs(mi - (y + h / 2)))
                        {
                            mi = i;
                            mj = j;
                            ms = s;
                        }
                    }
                }
                if (mi != -1)
                {
                    Polyline it = frags;
                    while (it!=null)
                    {
                        it.Tail.X = mj;
                        it.Tail.Y = mi;
                        it = it.Next;
                    }
                }
            }

            return frags;
        }

        private Polyline TraceSkeleton(int x, int y, int w, int h, int iter)
        {
            Polyline frags = null;

            if (iter >= Constants.MAX_ITER)
            { // gameover
                return frags;
            }
            if (w <= Constants.CHUNK_SIZE && h <= Constants.CHUNK_SIZE)
            { // recursive bottom
                frags = ChunkToFrags(x, y, w, h);
                return frags;
            }

            int ms = int.MaxValue; // number of white pixels on the seam, less the better
            int mi = -1; // horizontal seam candidate
            int mj = -1; // vertical   seam candidate

            if (h > Constants.CHUNK_SIZE)
            { // try splitting top and bottom
                for (int i = y + 3; i < y + h - 3; i++)
                {
                    if (_im[i * _w + x]>0 || _im[(i - 1) * _w + x]>0 || _im[i * _w + x + w - 1]>0 || _im[(i - 1) * _w + x + w - 1]>0)
                    {
                        continue;
                    }
                    int s = 0;
                    for (int j = x; j < x + w; j++)
                    {
                        s += _im[i * _w + j];
                        s += _im[(i - 1) * _w + j];
                    }
                    if (s < ms)
                    {
                        ms = s; mi = i;
                    }
                    else if (s == ms && Math.Abs(i - (y + h / 2)) < Math.Abs(mi - (y + h / 2)))
                    {
                        // if there is a draw (very common), we want the seam to be near the middle
                        // to balance the divide and conquer tree
                        ms = s; mi = i;
                    }
                }
            }

            if (w > Constants.CHUNK_SIZE) 
            { // same as above, try splitting left and right
                for (int j = x + 3; j < x + w - 3; j++)
                {
                    if (_im[_w * y + j]>0 || _im[_w * (y + h) - _w + j]>0 || _im[_w * y + j - 1]>0 || _im[_w * (y + h) - _w + j - 1]>0)
                    {
                        continue;
                    }
                    int s = 0;
                    for (int i = y; i < y + h; i++)
                    {
                        s += _im[i * _w + j]>0 ? 1 : 0;
                        s += _im[i * _w + j - 1]>0 ? 1 : 0;
                    }
                    if (s < ms)
                    {
                        ms = s;
                        mi = -1; // horizontal seam is defeated
                        mj = j;
                    }
                    else if (s == ms && Math.Abs(j - (x + w / 2)) < Math.Abs(mj - (x + w / 2)))
                    {
                        ms = s;
                        mi = -1;
                        mj = j;
                    }
                }
            }

            int L0 = -1; int L1 = 0; int L2 = 0; int L3 = 0;
            int R0 = -1; int R1 = 0; int R2 = 0; int R3 = 0;
            int dr = 0;
            int sx = 0;
            if (h > Constants.CHUNK_SIZE && mi != -1)
            { // split top and bottom
                L0 = x; L1 = y; L2 = w; L3 = mi - y;
                R0 = x; R1 = mi; R2 = w; R3 = y + h - mi;
                dr = (int)Orientation.Vertical;
                sx = mi;
            }
            else if (w > Constants.CHUNK_SIZE && mj != -1)
            { // split left and right
                L0 = x; L1 = y; L2 = mj - x; L3 = h;
                R0 = mj; R1 = y; R2 = x + w - mj; R3 = h;
                dr = (int)Orientation.Horizontal;
                sx = mj;
            }
            _rects = new Rects();
            if (dr != 0 && NotEmpty(L0, L1, L2, L3))
            { // if there are no white pixels, don't waste time
                if (Constants.SAVE_RECTS > 0)
                {
                    _rects.AddRect(R0, R1, R2, R3);
                }
                Task.Run(() =>
                {
                    frags = TraceSkeleton(L0, L1, L2, L3, iter + 1);
                });
                
            }
            if (dr != 0 && NotEmpty(R0, R1, R2, R3))
            {
                if (Constants.SAVE_RECTS>0)
                {
                    _rects.AddRect(R0, R1, R2, R3);
                }
                Task.Run(() =>
                {
                    frags = MergeFrags(frags, TraceSkeleton(R0, R1, R2, R3, iter + 1), sx, dr);
                });
            }

            if (mi == -1 && mj == -1)
            { // splitting failed! do the recursive bottom instead
                frags = ChunkToFrags(x, y, w, h);
            }

            return frags;
        }

        public void Trace(int[] img, int w, int h)
        {
            _w = w;
            _h = h;
            _im = null;
            _rects = null;
            _im = img;
            ThinningZs();
            Polyline p = TraceSkeleton(0, 0, w, h, 0);
            //while (p.Next != null)
            //{
            //    Console.WriteLine(p.Head.X+ " "+p.Head.Y);
            //    p = p.Next;
            //}
            
        }
    }
}
