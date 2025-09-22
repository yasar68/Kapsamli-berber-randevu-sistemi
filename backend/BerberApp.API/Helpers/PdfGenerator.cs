using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SkiaSharp;

namespace BerberApp.API.Helpers
{
    public class PdfGenerator
    {
        public void GenerateReport(
            string filePath,
            List<Customer> customers,
            List<Service> services,
            List<Appointment> appointments)
        {
            try
            {
                // Klasör yoksa oluştur
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                var doc = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(30);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        page.Header()
                            .Text("Berber Raporu")
                            .SemiBold()
                            .FontSize(20)
                            .FontColor(Colors.Blue.Medium);

                        page.Content()
                            .PaddingVertical(10)
                            .Column(col =>
                            {
                                col.Spacing(15);

                                col.Item().Text("Müşteri Tablosu").Bold().FontSize(16);
                                col.Item().Element(c => DrawCustomerTable(c, customers));

                                col.Item().Text("Hizmet Dağılımı (Pasta Grafik)").Bold().FontSize(16);
                                col.Item().Element(c => DrawServicePieChart(c, services, appointments));

                                col.Item().Text("En Çok Randevu Alınan Tarihler").Bold().FontSize(16);
                                col.Item().Element(c => DrawTopAppointmentDates(c, appointments));
                            });

                        page.Footer()
                            .AlignCenter()
                            .Text(x =>
                            {
                                x.CurrentPageNumber();
                                x.Span(" / ");
                                x.TotalPages();
                            });
                    });
                });

                using var fs = new FileStream(filePath, FileMode.Create);
                doc.GeneratePdf(fs);
            }
            catch (Exception ex)
            {
                // Hataları uygun şekilde logla veya yönet
                throw new Exception("PDF oluşturulurken hata oluştu: " + ex.Message, ex);
            }
        }

        private void DrawCustomerTable(IContainer container, List<Customer> customers)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("ID");
                    header.Cell().Element(CellStyle).Text("Ad Soyad");
                    header.Cell().Element(CellStyle).Text("E-posta");
                    header.Cell().Element(CellStyle).Text("Telefon");
                });

                foreach (var cust in customers)
                {
                    table.Cell().Element(CellStyle).Text(cust.Id.ToString());
                    table.Cell().Element(CellStyle).Text(cust.FullName);
                    table.Cell().Element(CellStyle).Text(cust.Email);
                    table.Cell().Element(CellStyle).Text(cust.Phone);
                }

                static IContainer CellStyle(IContainer container)
                {
                    return container
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Padding(5);
                }
            });
        }

        private void DrawServicePieChart(IContainer container, List<Service> services, List<Appointment> appointments)
        {
            var serviceCounts = services
                .Select(s => new
                {
                    Service = s,
                    Count = appointments.Count(a => a.ServiceId == s.Id)
                })
                .Where(sc => sc.Count > 0)
                .ToList();

            int total = serviceCounts.Sum(sc => sc.Count);

            if (total == 0)
            {
                container.Text("Henüz randevu yok.");
                return;
            }

            container.Canvas((canvas, size) =>
            {
                float centerX = size.Width / 2;
                float centerY = 100;
                float radius = 80;
                float startAngle = 0;
                Random rnd = new();

                foreach (var sc in serviceCounts)
                {
                    float sweepAngle = (float)(sc.Count / (double)total) * 360f;

                    var color = new SKColor(
                        (byte)rnd.Next(100, 255),
                        (byte)rnd.Next(100, 255),
                        (byte)rnd.Next(100, 255));

                    using var paint = new SKPaint
                    {
                        Color = color,
                        IsAntialias = true,
                        Style = SKPaintStyle.Fill
                    };

                    using var path = new SKPath();
                    path.MoveTo(centerX, centerY);
                    path.ArcTo(new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius),
                             startAngle, sweepAngle, false);
                    path.LineTo(centerX, centerY);
                    path.Close();
                    canvas.DrawPath(path, paint);

                    double percent = (sc.Count / (double)total) * 100;
                    float midAngle = startAngle + sweepAngle / 2;
                    float labelX = centerX + (float)(radius * 0.6 * Math.Cos(midAngle * Math.PI / 180));
                    float labelY = centerY + (float)(radius * 0.6 * Math.Sin(midAngle * Math.PI / 180));

                    using var textPaint = new SKPaint
                    {
                        Color = SKColors.Black,
                        IsAntialias = true,
                        TextSize = 9,
                        TextAlign = SKTextAlign.Center
                    };

                    canvas.DrawText($"{sc.Service.Name} ({percent:0.0}%)", labelX, labelY, textPaint);

                    startAngle += sweepAngle;
                }
            });
        }

        private void DrawTopAppointmentDates(IContainer container, List<Appointment> appointments)
        {
            var groupedDates = appointments
                .GroupBy(a => a.Date.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(5)
                .ToList();

            if (groupedDates.Count == 0)
            {
                container.Text("Henüz randevu yok.");
                return;
            }

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Text("Tarih").SemiBold();
                    header.Cell().Text("Randevu Sayısı").SemiBold();
                });

                foreach (var item in groupedDates)
                {
                    table.Cell().Text(item.Date.ToString("dd MMM yyyy"));
                    table.Cell().Text(item.Count.ToString());
                }
            });
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
    }

    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
    }
}
