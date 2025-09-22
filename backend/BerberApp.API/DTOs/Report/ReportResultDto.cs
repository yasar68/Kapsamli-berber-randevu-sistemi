using System;

namespace BerberApp.API.DTOs.Report
{
    public class ReportResultDto
    {
        public int TotalAppointments { get; set; }          // Toplam randevu sayısı
        public int TotalDurationMinutes { get; set; }       // Toplam randevu süresi (dakika)
        public DateTime StartDate { get; set; }              // Raporun başlangıç tarihi
        public DateTime EndDate { get; set; }                // Raporun bitiş tarihi
    }
}
