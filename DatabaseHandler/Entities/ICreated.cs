using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseHandler.Entities
{
    internal interface ICreated
    {
        public DateTime CreatedTime { get; set; }
        public string UserId { get; set; }
    }
}
