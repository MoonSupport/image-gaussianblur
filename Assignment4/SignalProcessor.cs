using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

namespace Assignment4
{
    class SignalProcessor
    {
        private static double getGaussianCurve1D(double sigma, int x)
        {
            double ePC = -Math.Pow(x, 2);
            double ePP = 2 * Math.Pow(sigma, 2);
            double eP = ePC / ePP;
            double eC = Math.E;
            double e = Math.Pow(eC, eP);

            return 1 / (sigma * Math.Sqrt(2 * Math.PI)) * e;
        }

        private static double getGaussianCurve2D(double sigma, int x, int y)
        {
            double ePC = -(Math.Pow(x, 2) + Math.Pow(y, 2));
            double ePP = 2 * Math.Pow(sigma, 2);
            double eP = ePC / ePP;
            double eC = Math.E;
            double e = Math.Pow(eC, eP);

            return 1 / (2 * Math.PI * Math.Pow(sigma, 2)) * e;
        }

        public static double[] GetGaussianFilter1D(double sigma)
        {
            int length = (int)Math.Ceiling((sigma * 6));
            if (length % 2 == 0)
            {
                length++;
            }

            double[] result = new double[length];

            int half = length / 2;
            for (int i = -half; i <= half; i++)
            {
                result[i + half] = getGaussianCurve1D(sigma, i);
            }

            return result;
        }

        // Convolve1D = h(0) + h(1) ... h(signal.length)

        // h(0) = f(-1) * g(0) +  f(0) * g(1) + f(1) * g(2)
        // h(n) = f(n-1) * g(middleG-1) + f(n) * g(middleG) + f(n+1) * g(middleG+1) // Length = 3 // middle = 1
        // h(n) = f(n-2) * g(middleG-2) +  f(n-1) * g(middleG-1) + f(n) * g(middleG) + f(n+1) * g(middleG+1)  f(n+2) * g(middleG+2) // Length = 5 // middle 2

        /**
         *   
         * for(let i = -g.middle; i<g.middle; g++) {
         *       result += f(n + i) * g(middleG + i)
         * }
         *
         */

        private static double[] reverse1DArray(double[] arr)
        {
            double[] result = new double[arr.Length];
            result[arr.Length / 2] = arr[arr.Length / 2];
            for (int i = 0; i < arr.Length / 2; i++)
            {
                result[i] = arr[arr.Length - i - 1];
                result[arr.Length - i - 1] = arr[i];
            }
            return result;
        }


        public static double[] Convolve1D(double[] signal, double[] filter)
        {
            double[] result = new double[signal.Length];
            int filterLength = filter.Length / 2;
            double[] reversedFilter = reverse1DArray(filter);
            for (int i = 0; i < signal.Length; i++)
            {
                //h(n) = f(n - i) * g(middleG - i)
                double value = 0.0;
                for (int j = -filterLength; j <= filterLength; j++)
                {
                    if (i + j < 0 || i + j >= signal.Length)
                    {
                        continue;
                    }

                    value += signal[i + j] * reversedFilter[filterLength + j];
                }
                result[i] = value;
            }

            return result;
        }

        // SignalProcessor.GetGaussianFilter2D(0.5);
        /*
        0.0116600978601128  0.0861571172073945  0.0116600978601128
        0.0861571172073945  0.636619772367581   0.0861571172073945
        0.0116600978601128  0.0861571172073945  0.0116600978601128
        */

        public static double[,] GetGaussianFilter2D(double sigma)
        {
            int length = (int)Math.Ceiling((sigma * 6));
            if (length % 2 == 0)
            {
                length++;
            }

            double[,] result = new double[length, length];

            int half = length / 2;
            for (int i = -half; i <= half; i++)
            {
                for (int j = -half; j <= half; j++)
                {
                    result[i + half, j + half] = getGaussianCurve2D(sigma, i, j);
                }
            }
            return result;
        }

        public static Bitmap ConvolveImage(Bitmap bitmap, double[,] filter)
        {
            Bitmap origin = new Bitmap(bitmap);
            Bitmap result = new Bitmap(bitmap);
            double[,] reversedFilter = reverse2DimArray(filter);
            for (int x = 0; x < origin.Width; x++)
            {
                for (int y = 0; y < origin.Height; y++)
                {
                    // convolveValue(x, y, result, origin, reversedFilter);
                }
            }
            return result;
        }

        private static void convolveValue(int x, int y, Bitmap result, Bitmap bitmap, double[,] reversedFilter)
        {

            int xMiddle = reversedFilter.GetLength(0) / 2;
            int yMiddle = reversedFilter.GetLength(1) / 2;
            double resultR = 0;
            double resultG = 0;
            double resultB = 0;
            for (int i = -xMiddle; i <= xMiddle; i++)
            {
                if (x + i < 0 || x + i >= bitmap.Width)
                {
                    continue;
                }
                for (int j = -yMiddle; j <= yMiddle; j++)
                {

                    if (y + j < 0 || y + j >= bitmap.Height)
                    {
                        continue;
                    }

                    Color pixel = bitmap.GetPixel(x + i, y + j);

                    resultR += (pixel.R * reversedFilter[yMiddle + j, xMiddle + i]);
                    resultG += (pixel.G * reversedFilter[yMiddle + j, xMiddle + i]);
                    resultB += (pixel.B * reversedFilter[yMiddle + j, xMiddle + i]);
                }
            }

            byte bResultR = (byte)resultR;
            byte bResultG = (byte)resultG;
            byte bResultB = (byte)resultB;

            Color newColor = Color.FromArgb(bResultR, bResultG, bResultB);
            result.SetPixel(x, y, newColor);
        }

        private static double[,] reverse2DimArray(double[,] arr)
        {
            int xLength = arr.GetLength(0);
            int yLength = arr.GetLength(1);
            double[,] result = new double[xLength, yLength];
            for (int i = 0; i < xLength; i++)
            {
                for (int j = 0; j < yLength; j++)
                {
                    result[xLength - 1 - i, yLength - 1 - j] = arr[i, j];
                }
            }
            return result;
        }
    }
}
