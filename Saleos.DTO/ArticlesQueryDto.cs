namespace Saleos.DTO
{
    public class ArticlesQueryDto
    {
        public string Title { get; set; }

        /**
         * Paging Query Information
         */

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        private int _pageSize = MaxPageSize;
        private int _pageNumber = 1;
        private const int MaxPageSize = 10; 
    }
}