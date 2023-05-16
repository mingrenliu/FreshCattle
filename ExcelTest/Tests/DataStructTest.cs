using ExcelUtile.ExcelCore;

namespace ExcelUtileTest.Tests
{
    [TestFixture]
    internal class DataStructTest
    {
        [Test]
        public void SortedWrapper_Count_list_Test()
        {
            var fwefwe = typeof(string).Name;
            var a = DateTime.Now.TimeOfDay.Add(new TimeSpan(35, 0, 0));
            var fewfwe = DateTime.Now.ToLongDateString();
            var fweefowe= DateTime.Now.ToLongTimeString();
            var dateonly = DateOnly.FromDateTime(DateTime.Now);
             var longstr=dateonly   .ToLongDateString();
             var shortstr=dateonly   .ToShortDateString();
            var timeonly=TimeOnly.FromDateTime(DateTime.Now);
            var timestr=timeonly.ToLongTimeString();
            var timeshortstr=timeonly.ToShortTimeString();
            var timefewf = DateTimeOffset.Now.DateTime.ToLongDateString();
            var str = a.ToString(@"hh\:mm\:ss");
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