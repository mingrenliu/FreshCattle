using InheritCore;

using Taskfewf = System.Collections.Generic;
namespace InheritTestLib;
[Inherit("UserConfig", 1 == 1)]
public partial class TestClass<T> where T : class
{

}