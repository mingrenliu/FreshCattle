using Bogus;

namespace ExcelTest.Mocks
{
    internal class PersonMock
    {
        public static IEnumerable<Entities.Person> Persons(int rows = 0)
        {
            var faker = new Faker<Entities.Person>("en");
            faker.RuleFor(x => x.IsEnable, faker => faker.PickRandomParam(true, false))
                .RuleFor(x => x.Name, faker => faker.Name.FullName())
                .RuleFor(x => x.Sex, faker => faker.PickRandomParam("male", "female"))
                .RuleFor(x => x.Age, faker => faker.Random.Int(10, 45))
                .RuleFor(x => x.Birthday, faker => faker.Date.Between(DateTime.Now.AddYears(1), DateTime.Now))
                .RuleFor(x => x.Remark, faker => faker.Lorem.Letter(25))
                .RuleFor(x => x.FeatherName, faker => faker.Name.FullName())
                .RuleFor(x => x.Money, faker => faker.Random.Double(10, 25642));
            if (rows > 0)
            {
                return faker.Generate(rows);
            }
            return faker.GenerateBetween(1000, 2000);
        }
    }
}