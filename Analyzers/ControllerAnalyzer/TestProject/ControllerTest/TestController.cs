using ServiceTest;

namespace ControllerTest
{
    public class TestController
    {
        private readonly ITestService _testService;

        /// <summary>
        /// 
        /// </summary>
        /// <paramtestService="testService"></param>
        public TestController(ITestService testService)
        {
            _testService = testService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <paramid="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task Delete(string id)
        {
            await _testService.DeleteAsync(id);
        }
        
    }
}