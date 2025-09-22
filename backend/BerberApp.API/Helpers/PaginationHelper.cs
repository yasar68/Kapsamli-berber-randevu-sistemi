using System;
using System.Collections.Generic;
using System.Linq;

namespace BerberApp.API.Helpers
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Verilen listeyi sayfalar ve istenilen sayfa verisini döner.
        /// </summary>
        /// <typeparam name="T">Liste eleman tipi</typeparam>
        /// <param name="source">Tam liste</param>
        /// <param name="pageNumber">Sayfa numarası (1’den başlar)</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
        /// <returns>Sayfalanmış liste (alt liste)</returns>
        public static IEnumerable<T> Paginate<T>(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            if (pageNumber < 1)
                throw new ArgumentException("Sayfa numarası 1 veya daha büyük olmalıdır.", nameof(pageNumber));
            if (pageSize < 1)
                throw new ArgumentException("Sayfa başına kayıt sayısı 1 veya daha büyük olmalıdır.", nameof(pageSize));

            return source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        /// <summary>
        /// Toplam sayfa sayısını hesaplar.
        /// </summary>
        /// <param name="totalItemCount">Toplam kayıt sayısı</param>
        /// <param name="pageSize">Sayfa başına kayıt sayısı</param>
        /// <returns>Toplam sayfa sayısı</returns>
        public static int GetTotalPages(int totalItemCount, int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentException("Sayfa başına kayıt sayısı 1 veya daha büyük olmalıdır.", nameof(pageSize));

            return (int)Math.Ceiling(totalItemCount / (double)pageSize);
        }
    }
}
