using Bogus;

namespace ExcelUtileTest.Mocks
{
    internal class RecordMock
    {
        public static IEnumerable<Record> Records()
        {
            var faker = new Faker<Record>("en");
            faker.RuleFor(x => x.IsEnable, faker => faker.PickRandomParam(true, false))
                .RuleFor(x => x.Name, faker => faker.Name.FullName())
                .RuleFor(x => x.Order, faker => faker.Random.Int(10, 45).OrNull(faker))
                .RuleFor(x => x.CreatedTime, faker => faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now))
                .RuleFor(x => x.Mass, faker => faker.Random.Double(1000, 450000))
                .RuleFor(x => x.Volume, faker => faker.Random.Decimal(100, 450000).OrNull(faker))
                .RuleFor(x => x.Spans, faker =>faker.Date.Timespan(new TimeSpan(50,15,25)).OrNull(faker))
                .RuleFor(x=>x.Date,faker=>faker.Date.BetweenDateOnly(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now.AddYears(1))))
                .RuleFor(x=>x.Time,faker=>faker.Date.BetweenTimeOnly(TimeOnly.FromDateTime(DateTime.Now.Date),TimeOnly.FromDateTime(DateTime.Now.Date.AddSeconds(-1))))
                .RuleFor(x => x.TimeOffset, faker => faker.Date.FutureOffset());
            return faker.GenerateBetween(1000, 2000);
        }
    }
}