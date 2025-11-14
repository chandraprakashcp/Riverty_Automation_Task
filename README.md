
# Booking API Automation Test Framework

A robust API testing framework for the Restful-Booker service, built with modern testing practices and tools.

##  Overview

This framework automates testing of the Restful-Booker API using Behavior-Driven Development (BDD) approach with SpecFlow. It provides comprehensive test coverage, detailed reporting, and CI/CD compatibility.

##  Features

- BDD Framework: Gherkin syntax with SpecFlow for readable test scenarios
- Comprehensive Reporting: HTML reports with Extent Reports
- REST API Testing: RestSharp for API interactions
- Clean Validation: FluentAssertions for expressive assertions
- Configuration Management: JSON-based configuration
- Parallel Execution: NUnit test runner support
- Tag-based Testing: Selective test execution (Smoke, Regression)

##  Tech Stack

- C# / .NET 6
- SpecFlow (BDD framework)
- NUnit (Test runner)
- RestSharp (HTTP client)
- FluentAssertions (Assertion library)
- Extent Reports (Reporting)

##  Project Structure


SpecFlowBookingAPI/
├── Dependencies/
├── Features/
│   ├── BookingAPI.feature          # Gherkin feature files
│   └── BookingAPI.feature.cs       # Auto-generated feature code
├── Hooks/
│   └── Hooks.cs                   # Test hooks and setup
├── StepDefinitions/
│   ├── BookingApiClient.cs        # API client wrapper
│   └── BookingSteps.cs            # Step definition implementations
├── Helpers/
│   └── ExtentReportHelper.cs      # Reporting utilities
├── .gitignore
├── .gitlab-ci.yml                 # GitLab CI/CD pipeline
├── appsettings.json               # Test configuration
├── Configurations.cs              # Configuration loader
├── Dockerfile                     # Container configuration
└── README.md


##  Prerequisites

- .NET 6 SDK
- Visual Studio 2022 or VS Code
- Git
- For GitLab Pipeline: Docker Desktop must be running

##  Getting Started

1. Clone the repository
   
   git clone <repository-url>
   cd SpecFlowBookingAPI
   

2. Restore dependencies
   
   dotnet restore
 

3. Run tests
   
   # Run all tests
   dotnet test
   
   # Run specific test categories
   dotnet test --filter TestCategory=Smoke
   dotnet test --filter TestCategory=Regression
   

##  Test Coverage

The framework tests key Restful-Booker API endpoints:

- `GET /booking` - Retrieve booking IDs
- `GET /booking/{id}` - Get booking details
- `POST /booking` - Create new booking
- `PUT /booking/{id}` - Update existing booking

##  Configuration

Update `appsettings.json` for your environment:

  json
{
  "BaseUrl": "https://restful-booker.herokuapp.com",
  "Timeout": 30
}


##  Reporting

HTML test reports are automatically generated with detailed execution results including feature/scenario status, step details, and failure analysis.
Note: can see it in \booking-api-automation\bin\Debug\net6.0\TestReports  - If run it locally
If run it via GitLab pipeline, can see it in the pipeline job artifacts.

##  Docker Support

   bash
# Build image
docker build -t bookingapi-tests .

# Run tests
docker run bookingapi-tests


##  CI/CD Integration

### GitLab Pipeline
The framework includes `.gitlab-ci.yml` for automated testing. Before running the pipeline:

Important: Ensure Docker Desktop is running on your system.

The pipeline will:
- Build Docker image
- Execute tests in containerized environment
- Generate test reports and artifacts

##  Framework Components

- BookingApiClient.cs: REST API client wrapper using RestSharp
- BookingSteps.cs: Step definitions for BDD scenarios
- Hooks.cs: Test lifecycle management
- ExtentReportHelper.cs: HTML report generation
- Configurations.cs: Configuration management

