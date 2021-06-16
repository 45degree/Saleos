using System.Collections.Generic;
using Saleos.DAO;

namespace Saleos.Models
{
    public class HomeIndexViewModel
    {
        public List<ArticleInfoDAO> articleInfos { get; set; }
        public int currentPage { get; set; }
    }
}
