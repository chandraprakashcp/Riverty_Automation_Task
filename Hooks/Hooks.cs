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
            _scenarioCount++;
            Console.WriteLine($"\n📝 Scenario #{_scenarioCount}: {_scenarioContext.ScenarioInfo.Title}");
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
            Console.WriteLine($"📈 Completed: {_scenarioCount} scenarios");
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Console.WriteLine($"🏁 Test Run Completed. Total Scenarios: {_scenarioCount}");
            ExtentReportHelper.FlushReport();
            Console.WriteLine("✅ Report generation process finished!");
        }
    }
}