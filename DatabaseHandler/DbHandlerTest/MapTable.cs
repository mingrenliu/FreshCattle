using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbHandlerTest
{
    [Table("MapTable")]
    [DbContext(typeof(TestDbContext))]
    public class MapTable
    {
        [NotMapped]
        private string? name { get; set; }

        public int MyProperty { get; set; }
        public long MyProperty1 { get; set; }
        public DateTime? Date { get; set; }
        public bool? Date1 { get; set; }
        public DateOnly? dfewf { get; set; }

        [StringLength(50)]
        public string Id { get; set; }

        [StringLength(50)]
        public MapTable Name { get; set; }

        public IEnumerable<string> Name1 { get; set; }
        public IEnumerable<MapTable> Name2 { get; set; }
    }
}