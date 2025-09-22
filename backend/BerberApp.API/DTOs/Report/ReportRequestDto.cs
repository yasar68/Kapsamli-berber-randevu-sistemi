using System;

namespace BerberApp.API.DTOs.Report
{
    public class ReportRequestDto
    {
        public int BarberId { get; set; }  // Hangi berbere ait rapor isteği

        public DateTime? StartDate { get; set; }  // Başlangıç tarihi (zorunlu yapabilirsin)

        public DateTime? EndDate { get; set; }  // Bitiş tarihi (zorunlu yapabilirsin)

        public string? Notes { get; set; }  // İstersen rapor için not ekleyebilirsin
    }
}
