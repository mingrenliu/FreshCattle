﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbHandlerTest.Entity
{
    [Table("MapTable1")]
    [DbContext(typeof(TestDbContext))]
    public class MapTable1
    {
        [StringLength(50)]
        public string Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
    }
}
