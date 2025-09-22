namespace BerberApp.API.DTOs.Common
{
    public class PagedResultDto<T>
    {
        public int CurrentPage { get; set; }    // Şu anki sayfa numarası
        public int PageSize { get; set; }       // Sayfadaki öğe sayısı
        public int TotalCount { get; set; }     // Toplam kayıt sayısı
        public int TotalPages { get; set; }     // Toplam sayfa sayısı

        public List<T> Items { get; set; } = new List<T>();   // Sayfadaki veri listesi

        // Sayfa sonunda mi? 
        public bool HasNextPage => CurrentPage < TotalPages;

        // Sayfa başında mı?
        public bool HasPreviousPage => CurrentPage > 1;

        // Parametresiz constructor
        public PagedResultDto() { }

        // Tüm bilgileri constructor ile atayabilirsin
        public PagedResultDto(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
