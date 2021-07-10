using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Saleos.DAO;
using Saleos.Entity.Services.CoreServices;

namespace Saleos.Model
{
    public class TagModel
    {
        public List<TagDAO> Data { get; set; }
    }
}
