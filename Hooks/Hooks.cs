using SpecFlowBookingAPI.Helpers;
using TechTalk.SpecFlow;

namespace SpecFlowBookingAPI.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private static int _scenarioCount = 0;
        private static readonly object _lockObject = new object();

        public Hooks(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Console.WriteLine("🚀 Starting Test Run...");
            ExtentReportHelper.InitializeReport();
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            int currentCount;
            lock (_lockObject)
            {
                _scenarioCount++;
                currentCount = _scenarioCount;
            }

            Console.WriteLine($"\n📝 Scenario #{currentCount}: {_scenarioContext.ScenarioInfo.Title}");
            ExtentReportHelper.CreateTest(_featureContext, _scenarioContext);
        }

        [AfterStep]
        public void AfterStep()
        {
            ExtentReportHelper.LogStep(_scenarioContext);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            var status = _scenarioContext.TestError == null ? "PASSED" : "FAILED";
            Console.WriteLine($"📊 Scenario Result: {status}");

            lock (_lockObject)
            {
                Console.WriteLine($"📈 Completed: {_scenarioCount} scenarios");
            }
        }


        [AfterTestRun]
        public static void AfterTestRun()
        {
            lock (_lockObject)
            {
                Console.WriteLine($"🏁 Test Run Completed. Total Scenarios: {_scenarioCount}");
            }

            ExtentReportHelper.FlushReport();
            Console.WriteLine("✅ Report generation process finished!");
        }
    }
}