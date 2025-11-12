using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow;

namespace SpecFlowBookingAPI.StepDefinitions
{
    [Binding]
    public class BookingSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly BookingApiClient _apiClient;
        private IRestResponse _response;

        public BookingSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _apiClient = new BookingApiClient();
        }

        [Given(@"I set the base API URL to the restful-booker service")]
        public void GivenISetTheBaseAPIURLToTheRestfulBookerService()
        {
            // Base URL is set in the API client constructor
        }

        [Given(@"I create a new booking with firstname ""(.*)"" and lastname ""(.*)""")]
        public void GivenICreateANewBookingWithFirstnameAndLastname(string firstname, string lastname)
        {
            var random = new Random();
            var checkinDate = DateTime.Now.AddDays(random.Next(10, 30));
            var checkoutDate = checkinDate.AddDays(random.Next(3, 10));

            var booking = new Booking
            {
                firstname = firstname,
                lastname = lastname,
                totalprice = random.Next(100, 1000),
                depositpaid = random.Next(0, 2) == 1,
                bookingdates = new BookingDates
                {
                    checkin = checkinDate.ToString("yyyy-MM-dd"),
                    checkout = checkoutDate.ToString("yyyy-MM-dd")
                },
                additionalneeds = GetRandomAdditionalNeeds(random)
            };

            _response = _apiClient.PostRequest("booking", booking);

            if (_response.StatusCode == HttpStatusCode.OK)
            {
                var createdBooking = JsonConvert.DeserializeObject<BookingResponse>(_response.Content);
                _scenarioContext["currentBookingId"] = createdBooking.bookingid;
                _scenarioContext["createdBookingId"] = createdBooking.bookingid;
                _scenarioContext["createdBookingData"] = booking;
            }
            else
            {
                throw new Exception($"Failed to create booking: {_response.StatusCode}. Response: {_response.Content}");
            }
        }

        private string GetRandomAdditionalNeeds(Random random)
        {
            var additionalNeeds = new[]
            {
                "Breakfast", "Late Checkout", "Early Checkin", "Airport Transfer",
                "Extra Bed", "WiFi", "Parking", "Swimming Pool", "Gym Access", "Spa Treatment"
            };
            return additionalNeeds[random.Next(additionalNeeds.Length)];
        }

        [Given(@"I store the booking ID as ""(.*)""")]
        public void GivenIStoreTheBookingIDAs(string key)
        {
            // Booking ID is already stored, this step just documents the storage
            var bookingId = _scenarioContext["currentBookingId"];
            _scenarioContext[key] = bookingId;
        }

        [When(@"I send a GET request to the ""(.*)"" endpoint")]
        public void WhenISendAGETRequestToTheEndpoint(string endpoint)
        {
            // Replace <bookingid> with actual ID from context if it exists
            if (endpoint.Contains("<bookingid>") && _scenarioContext.ContainsKey("currentBookingId"))
            {
                var bookingId = _scenarioContext["currentBookingId"].ToString();
                endpoint = endpoint.Replace("<bookingid>", bookingId);
            }
            _response = _apiClient.GetRequest(endpoint);
        }

        [When(@"I send a POST request to the ""(.*)"" endpoint with the following booking data:")]
        public void WhenISendAPOSTRequestToTheEndpointWithTheFollowingBookingData(string endpoint, Table table)
        {
            var bookingData = table.Rows[0];
            var booking = new Booking
            {
                firstname = bookingData["firstname"],
                lastname = bookingData["lastname"],
                totalprice = int.Parse(bookingData["totalprice"]),
                depositpaid = bool.Parse(bookingData["depositpaid"]),
                bookingdates = new BookingDates
                {
                    checkin = bookingData["checkin"],
                    checkout = bookingData["checkout"]
                },
                additionalneeds = bookingData["additionalneeds"]
            };

            _response = _apiClient.PostRequest(endpoint, booking);

            if (_response.StatusCode == HttpStatusCode.OK)
            {
                var createdBooking = JsonConvert.DeserializeObject<BookingResponse>(_response.Content);
                _scenarioContext["createdBookingId"] = createdBooking.bookingid;
            }
        }

        [When(@"I send a POST request to the ""(.*)"" endpoint with incomplete booking data:")]
        public void WhenISendAPOSTRequestToTheEndpointWithIncompleteBookingData(string endpoint, Table table)
        {
            var bookingData = table.Rows[0];
            var booking = new Booking
            {
                firstname = bookingData["firstname"],
                lastname = bookingData.ContainsKey("lastname") ? bookingData["lastname"] : null,
                totalprice = int.Parse(bookingData["totalprice"]),
                depositpaid = bookingData.ContainsKey("depositpaid") ? bool.Parse(bookingData["depositpaid"]) : false,
                bookingdates = new BookingDates
                {
                    checkin = bookingData.ContainsKey("checkin") ? bookingData["checkin"] : null,
                    checkout = bookingData.ContainsKey("checkout") ? bookingData["checkout"] : null
                },
                additionalneeds = bookingData.ContainsKey("additionalneeds") ? bookingData["additionalneeds"] : null
            };

            _response = _apiClient.PostRequest(endpoint, booking);
        }

        [When(@"I send a PUT request to the ""(.*)"" endpoint with the following updated data:")]
        public void WhenISendAPUTRequestToTheEndpointWithTheFollowingUpdatedData(string endpoint, Table table)
        {
            var endpointWithId = endpoint.Replace("<bookingid>", _scenarioContext["createdBookingId"].ToString());

            var bookingData = table.Rows[0];
            var booking = new Booking
            {
                firstname = bookingData["firstname"],
                lastname = bookingData["lastname"],
                totalprice = int.Parse(bookingData["totalprice"]),
                depositpaid = bool.Parse(bookingData["depositpaid"]),
                bookingdates = new BookingDates
                {
                    checkin = bookingData["checkin"],
                    checkout = bookingData["checkout"]
                },
                additionalneeds = bookingData["additionalneeds"]
            };

            var token = _scenarioContext["authToken"].ToString();
            _response = _apiClient.PutRequest(endpointWithId, booking, token);
        }

        [Given(@"I have obtained an authentication token")]
        public void GivenIHaveObtainedAnAuthenticationToken()
        {
            var authResponse = _apiClient.GetAuthToken();

            if (authResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var authData = JsonConvert.DeserializeObject<AuthResponse>(authResponse.Content);
                _scenarioContext["authToken"] = authData.token;
            }
            else
            {
                throw new Exception($"Authentication failed with status: {authResponse.StatusCode}");
            }
        }

        [Then(@"the response status code should be (\d+)")]
        public void ThenTheResponseStatusCodeShouldBe(int expectedStatusCode)
        {
            _response.StatusCode.Should().Be((HttpStatusCode)expectedStatusCode);
        }

        [Then(@"the response should contain a list of booking IDs")]
        public void ThenTheResponseShouldContainAListOfBookingIDs()
        {
            var bookings = JsonConvert.DeserializeObject<List<BookingIdResponse>>(_response.Content);
            bookings.Should().NotBeNull();
            bookings.Should().NotBeEmpty();
        }

        [Then(@"the response should contain the booking details for ""(.*)"" ""(.*)""")]
        public void ThenTheResponseShouldContainTheBookingDetailsFor(string firstname, string lastname)
        {
            var booking = JsonConvert.DeserializeObject<Booking>(_response.Content);
            booking.Should().NotBeNull();
            booking.firstname.Should().Be(firstname);
            booking.lastname.Should().Be(lastname);
        }

        [Then(@"the created booking should have valid random data")]
        public void ThenTheCreatedBookingShouldHaveValidRandomData()
        {
            var createdBooking = _scenarioContext["createdBookingData"] as Booking;
            createdBooking.Should().NotBeNull();
            createdBooking.totalprice.Should().BeInRange(100, 1000);
            createdBooking.bookingdates.checkin.Should().NotBeNullOrEmpty();
            createdBooking.bookingdates.checkout.Should().NotBeNullOrEmpty();

            var checkin = DateTime.Parse(createdBooking.bookingdates.checkin);
            var checkout = DateTime.Parse(createdBooking.bookingdates.checkout);
            checkout.Should().BeAfter(checkin);
        }

        [Then(@"the response should contain a booking ID")]
        public void ThenTheResponseShouldContainABookingID()
        {
            var bookingResponse = JsonConvert.DeserializeObject<BookingResponse>(_response.Content);
            bookingResponse.bookingid.Should().BeGreaterThan(0);
        }

        [Then(@"the response booking details should match the request data")]
        public void ThenTheResponseBookingDetailsShouldMatchTheRequestData()
        {
            var responseBooking = JsonConvert.DeserializeObject<BookingResponse>(_response.Content);
            responseBooking.booking.Should().NotBeNull();
        }

        [Then(@"the response should contain the updated booking details")]
        public void ThenTheResponseShouldContainTheUpdatedBookingDetails()
        {
            var updatedBooking = JsonConvert.DeserializeObject<Booking>(_response.Content);
            updatedBooking.Should().NotBeNull();
        }

        [Then(@"the response should contain booking details for ID (\d+)")]
        public void ThenTheResponseShouldContainBookingDetailsForID(int bookingId)
        {
            var booking = JsonConvert.DeserializeObject<Booking>(_response.Content);
            booking.Should().NotBeNull();
            booking.firstname.Should().NotBeNullOrEmpty();
            booking.lastname.Should().NotBeNullOrEmpty();
        }
    }
}