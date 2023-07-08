using InheritCore;
namespace InheritTestLib;

[Inherit("UserConfig")]
public partial class TestClass<T> where T : class
{

}