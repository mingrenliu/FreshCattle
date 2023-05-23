using ExcelUtile.ExcelCore;

namespace ExcelTest
{
    [TestFixture]
    internal class DataStructTest
    {
        [Test]
        public void SortedWrapper_Count_list_Test()
        {
            var type = typeof(ushort).Name;
            var list = GetHeaders();
            Assert.That(list.Count(), Is.EqualTo(4));
        }

        [Test]
        public void SortedWrapper_Print_list_Test()
        {
            var list = GetHeaders();
            Assert.That(list.Skip(1).First().Order, Is.EqualTo(2));
            Assert.That(list.Skip(2).First().Order, Is.EqualTo(3));
        }

        private SortedWrapper<HeaderInfo> GetHeaders()
        {
            var list = new SortedWrapper<HeaderInfo>();
            list.Add(1, new HeaderInfo("liu", 1));
            list.Add(3, new HeaderInfo("liu", 3));
            list.Add(2, new HeaderInfo("liu", 2));
            list.Add(4, new HeaderInfo("liu", 4));
            list.Add(3, new HeaderInfo("liu", 5));
            list.Add(2, new HeaderInfo("liu", 6));
            list.Add(1, new HeaderInfo("liu", 7));
            return list;
        }
    }
}