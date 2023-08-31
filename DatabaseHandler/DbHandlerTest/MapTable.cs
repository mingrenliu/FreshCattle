using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbHandlerTest
{
    [Table("MapTable")]
    [DbContext(typeof(DbHandlerTest.TestDbContext))]
    public class MapTable
    {
        private string name { get; set; }

        [StringLength(50)]
        public string Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
