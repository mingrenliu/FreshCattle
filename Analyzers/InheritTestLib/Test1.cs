using System;
using System.Reflection.Metadata.Ecma335;
using System.Collections.Generic;

namespace InheritTestLib
{

    public class Test1
    {
        public string Name { get; set; }
        public IEnumerable<string> MyProperty { get; set; }
        public Test<Test1> MyProperty1 { get; set; }
    }

    public partial class Test1<T> where T : IEnumerable<Test1>
    {

    }
    public partial class Test<T> where T : class
    {

    }
}

