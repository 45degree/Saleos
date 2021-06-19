using System.Collections.Generic;
using Saleos.DAO;

namespace Saleos.Models
{
    public class AdminArticleViewModel
    {
        public List<ArticleInfoDAO> articleInfos { get;set; }
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
    }
}
