using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using TechTalk.SpecFlow;
using System.Text;

namespace SpecFlowBookingAPI.Helpers
{
    public class ExtentReportHelper
    {
        private static AventStack.ExtentReports.ExtentReports _extent;
        private static ExtentTest _feature;
        private static ExtentTest _scenario;
        private static string _reportPath;

        public static void InitializeReport()
        {
            try
            {
                var reportsDir = Path.Combine(Directory.GetCurrentDirectory(), "TestReports");
                Directory.CreateDirectory(reportsDir);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                _reportPath = Path.Combine(reportsDir, $"BookingAPI_TestReport_{timestamp}.html");

                var htmlReporter = new ExtentHtmlReporter(_reportPath);
                htmlReporter.Config.DocumentTitle = "Booking API Test Report";
                htmlReporter.Config.ReportName = "Restful-booker API Automation Report";

                _extent = new AventStack.ExtentReports.ExtentReports();
                _extent.AttachReporter(htmlReporter);

                // System information
                _extent.AddSystemInfo("Environment", "Test");
                _extent.AddSystemInfo("OS", Environment.OSVersion.VersionString);
                _extent.AddSystemInfo("Machine", Environment.MachineName);
                _extent.AddSystemInfo("User", Environment.UserName);
                _extent.AddSystemInfo("Runtime", Environment.Version.ToString());
                _extent.AddSystemInfo("Report Generated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                Console.WriteLine($"Extent Report Initialized: {_reportPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing report: {ex.Message}");
            }
        }

        public static void CreateTest(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            try
            {
                // Create feature once
                if (_feature == null || _feature.Model.Name != featureContext.FeatureInfo.Title)
                {
                    _feature = _extent.CreateTest<AventStack.ExtentReports.Gherkin.Model.Feature>(
                        featureContext.FeatureInfo.Title
                    );
                }

                // Create scenario node for each scenario
                _scenario = _feature.CreateNode<AventStack.ExtentReports.Gherkin.Model.Scenario>(
                    scenarioContext.ScenarioInfo.Title
                );

                // Add tags as categories
                foreach (var tag in scenarioContext.ScenarioInfo.Tags)
                {
                    _scenario.AssignCategory(tag);
                }

                Console.WriteLine($"Scenario Started: {scenarioContext.ScenarioInfo.Title}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating test in report: {ex.Message}");
            }
        }

        public static void LogStep(ScenarioContext scenarioContext)
        {
            try
            {
                var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
                var stepName = scenarioContext.StepContext.StepInfo.Text;

                if (scenarioContext.TestError == null)
                {
                    // Passed step
                    switch (stepType.ToLower())
                    {
                        case "given":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.Given>(stepName);
                            break;
                        case "when":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.When>(stepName);
                            break;
                        case "then":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.Then>(stepName);
                            break;
                        case "and":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.And>(stepName);
                            break;
                    }

                    Console.WriteLine($"{stepType}: {stepName}");
                }
                else
                {
                    // Failed step with details
                    var errorMessage = $"{stepType}: {stepName}";
                    errorMessage += $"\nError: {scenarioContext.TestError.Message}";

                    if (!string.IsNullOrEmpty(scenarioContext.TestError.StackTrace))
                    {
                        errorMessage += $"\nStack Trace: {scenarioContext.TestError.StackTrace}";
                    }

                    switch (stepType.ToLower())
                    {
                        case "given":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.Given>(stepName)
                                .Fail(errorMessage);
                            break;
                        case "when":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.When>(stepName)
                                .Fail(errorMessage);
                            break;
                        case "then":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.Then>(stepName)
                                .Fail(errorMessage);
                            break;
                        case "and":
                            _scenario.CreateNode<AventStack.ExtentReports.Gherkin.Model.And>(stepName)
                                .Fail(errorMessage);
                            break;
                    }

                    Console.WriteLine($"{stepType}: {stepName} - FAILED");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging step: {ex.Message}");
            }
        }

        public static void FlushReport()
        {
            try
            {
                _extent.Flush();
                Console.WriteLine($"HTML Report Generated Successfully: {_reportPath}");

                TryOpenReportInBrowser();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error flushing report: {ex.Message}");
            }
        }

        private static void TryOpenReportInBrowser()
        {
            try
            {
                if (File.Exists(_reportPath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = _reportPath,
                        UseShellExecute = true
                    });

                    Console.WriteLine("Opening report in browser...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to open report in browser: {ex.Message}");
            }
        }

        public static string GetReportPath()
        {
            return _reportPath;
        }
    }
}
