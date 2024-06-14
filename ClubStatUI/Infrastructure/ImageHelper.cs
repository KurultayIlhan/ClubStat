// ***********************************************************************
// Assembly         : ClubStatUI
// Author           : Ilhan Kurultay
// Created          : Wed 05-Jun-2024
//
// Last Modified By : Ilhan Kurultay
// Last Modified On : Mon 10-Jun-2024
// ***********************************************************************
// <copyright file="ImageHelper.cs" company="Ilhan Kurultay">
//     Copyright (c) Ilhan Kurultay. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Walter;

namespace ClubStatUI.Infrastructure
{
    internal static class ImageHelper
    {
        //image cashing
        private static readonly ConcurrentDictionary<int, ImageSource> _imageCache = new ConcurrentDictionary<int, ImageSource>();

        /// <summary>
        /// Converts the bytes to an image in away that we do not need to be afraid that we keep making images from scratch.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns>ImageSource</returns>
        public static ImageSource ToImage(this byte[] bytes)
        {
            //will throw and log if empty
            Guard.EnsureNotNullOrEmpty(bytes,nameof(bytes));

            int hashCode = GetByteArrayHashCode(bytes);

            // Use GetOrAdd to ensure atomic operation and get the image for the player
            return _imageCache.GetOrAdd(hashCode, _ => ImageSource.FromStream(() => new MemoryStream(bytes)));

        }

        internal static void ReplaceImage(byte[] bytes,ImageSource imageSource)
        {
            Guard.EnsureNotNullOrEmpty(bytes, nameof(bytes));

            int hashCode = GetByteArrayHashCode(bytes);
            _imageCache[hashCode] = imageSource;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetByteArrayHashCode(byte[] bytes)
        {
            unchecked
            {
                int hash = 17;
                foreach (var b in bytes)
                {
                    hash = (hash * 31) + b;
                }
                return hash;
            }
        }
    }

}
