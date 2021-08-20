using SkeletonTracing;
using System;
using System.Drawing;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            SkeletonTracer tracer = new SkeletonTracer();
            TraceSkeleton tracer2 = new TraceSkeleton();
            Bitmap image = new Bitmap("./opencv-thinning-src-img.png");
            int[] im = new int[image.Width * image.Height];
            bool[] im2 = new bool[image.Width * image.Height];
            for(int i = 0; i< image.Width; i++)
            {
                for(int j = 0; j< image.Height; j++)
                {
                    im[i + j * image.Width] = (image.GetPixel(i, j).R > 0.5) ? 1 : 0;
                    im2[i + j * image.Width] = (image.GetPixel(i, j).R > 0.5);
                }
            }
            DateTime now = DateTime.Now;
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            // Console.WriteLine(image.Width+ " "+ image.Height);
            for(int i =0; i< 1000; i++)
            {
                tracer.Trace(im, image.Width, image.Height);
            }
            Console.WriteLine((DateTime.Now - now).TotalMilliseconds);


            now = DateTime.Now;
            // Console.WriteLine(image.Width+ " "+ image.Height);
            for (int i = 0; i < 1000; i++)
            {
                tracer2.trace(im2, image.Width, image.Height, 10);
            }
            Console.WriteLine((DateTime.Now - now).TotalMilliseconds);
        }

        
    }
}
