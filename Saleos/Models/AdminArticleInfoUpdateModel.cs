namespace Saleos.Models
{
    public class AdminArticleInfoUpdateModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public bool IsReprint { get; set; }
        public string ReprintUrl { get; set; }
        public bool Visible { get; set; }
        public string ImageUrl { get; set; }
    }
}
