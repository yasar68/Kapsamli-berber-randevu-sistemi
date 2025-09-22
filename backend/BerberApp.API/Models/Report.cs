namespace BerberApp.API.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        // Randevu sayıları
        public int TotalAppointments { get; set; }
        public int PendingAppointments { get; set; }
        public int ConfirmedAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        
        // Gelir bilgileri
        public decimal TotalRevenue { get; set; }
        public decimal CompletedRevenue { get; set; }
        public decimal ConfirmedRevenue { get; set; }
        
        public string? Notes { get; set; }
    }
}