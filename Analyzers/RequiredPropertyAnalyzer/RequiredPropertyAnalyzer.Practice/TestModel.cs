using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RequiredPropertyAnalyzer.Test
{
#pragma warning disable CS8618
    public class TestModel
    {
        public int Age { get; set; }
        public string? Name { get; set; }
        public TestEnum Status { get; set; }
        public DateTime? Date { get; set; }

        [Required]
        public TestModel Model { get; set; }
    }

    public enum TestEnum
    {
        A,
        B,
        C
    }
#pragma warning restore CS8618
}