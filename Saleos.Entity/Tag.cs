using System.Collections.Generic;

namespace Saleos.Entity
{
    public class Tag
    {
        public Tag()
        {
            ArticleTag = new List<ArticleTag>();
        }
        public int Id { get; set; }
        public string Content { get; set; }
        public List<ArticleTag> ArticleTag {get; set;}
    }
}
