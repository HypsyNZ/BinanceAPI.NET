/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using System;
using System.Collections.Generic;

namespace BinanceAPI.Objects
{
    /// <summary>
    /// Comparer for byte order
    /// </summary>
    public class ByteOrderComparer : IComparer<byte[]>
    {
        /// <summary>
        /// Compare function
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(byte[] x, byte[] y)
        {
            // Shortcuts: If both are null, they are the same.
            if (x == null && y == null) return 0;

            // If one is null and the other isn't, then the
            // one that is null is "lesser".
            if (x == null) return -1;
            if (y == null) return 1;

            // Both arrays are non-null.  Find the shorter
            // of the two lengths.
            var bytesToCompare = Math.Min(x.Length, y.Length);

            // Compare the bytes.
            for (var index = 0; index < bytesToCompare; ++index)
            {
                // The x and y bytes.
                var xByte = x[index];
                var yByte = y[index];

                // Compare result.
                var compareResult = Comparer<byte>.Default.Compare(xByte, yByte);

                // If not the same, then return the result of the
                // comparison of the bytes, as they were the same
                // up until now.
                if (compareResult != 0) return compareResult;

                // They are the same, continue.
            }

            // The first n bytes are the same.  Compare lengths.
            // If the lengths are the same, the arrays
            // are the same.
            if (x.Length == y.Length) return 0;

            // Compare lengths.
            return x.Length < y.Length ? -1 : 1;
        }
    }
}
