using System;

namespace BerberApp.API.Helpers
{
    public static class TimeHelper
    {
        /// <summary>
        /// Belirtilen tarih ile şu an arasındaki tam gün sayısını döner.
        /// </summary>
        public static int GetDaysDifference(DateTime date)
        {
            return (DateTime.Now.Date - date.Date).Days;
        }

        /// <summary>
        /// Tarihi "yyyy-MM-dd" formatında string olarak döner.
        /// </summary>
        public static string ToShortDateString(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Tarihi "dd MMM yyyy HH:mm" formatında string olarak döner.
        /// Örnek: 08 Tem 2025 14:30
        /// </summary>
        public static string ToLongDateTimeString(DateTime date)
        {
            return date.ToString("dd MMM yyyy HH:mm");
        }

        /// <summary>
        /// Verilen iki tarih arasında geçen zamanı saat:dakika:saniye formatında döner.
        /// </summary>
        public static string GetDurationString(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("Bitiş zamanı başlangıç zamanından önce olamaz.");

            TimeSpan duration = end - start;
            return $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
        }

        /// <summary>
        /// Verilen tarihin hafta içi mi hafta sonu mu olduğunu döner.
        /// </summary>
        public static bool IsWeekend(DateTime date)
        {
            return (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);
        }

        /// <summary>
        /// Belirtilen tarihe gün ekler.
        /// </summary>
        public static DateTime AddDays(DateTime date, int days)
        {
            return date.AddDays(days);
        }

        /// <summary>
        /// Belirtilen tarihe saat ekler.
        /// </summary>
        public static DateTime AddHours(DateTime date, int hours)
        {
            return date.AddHours(hours);
        }

        /// <summary>
        /// Verilen iki tarih arasındaki toplam iş günü sayısını döner (Hafta sonları hariç).
        /// </summary>
        public static int GetBusinessDays(DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("Bitiş tarihi başlangıç tarihinden küçük olamaz.");

            int totalDays = (end - start).Days + 1;
            int businessDays = 0;

            for (int i = 0; i < totalDays; i++)
            {
                var current = start.AddDays(i).DayOfWeek;
                if (current != DayOfWeek.Saturday && current != DayOfWeek.Sunday)
                    businessDays++;
            }

            return businessDays;
        }
    }
}
