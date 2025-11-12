using RestSharp;
using Newtonsoft.Json;

namespace SpecFlowBookingAPI.StepDefinitions
{
    public class BookingApiClient
    {
        private readonly RestClient _client;

        public BookingApiClient()
        {
            _client = new RestClient(Configuration.BaseUrl);
        }

        public IRestResponse GetRequest(string endpoint)
        {
            var request = new RestRequest(endpoint, Method.GET);
            request.AddHeader("Accept", "application/json");
            return _client.Execute(request);
        }

        public IRestResponse PostRequest(string endpoint, object body)
        {
            var request = new RestRequest(endpoint, Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(body);
            return _client.Execute(request);
        }

        public IRestResponse PutRequest(string endpoint, object body, string token)
        {
            var request = new RestRequest(endpoint, Method.PUT);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Cookie", $"token={token}");
            request.AddJsonBody(body);
            return _client.Execute(request);
        }

        public IRestResponse DeleteRequest(string endpoint, string token)
        {
            var request = new RestRequest(endpoint, Method.DELETE);
            request.AddHeader("Cookie", $"token={token}");
            return _client.Execute(request);
        }

        public IRestResponse GetAuthToken()
        {
            var authRequest = new
            {
                username = Configuration.Username,
                password = Configuration.Password
            };
            return PostRequest("auth", authRequest);
        }
    }

    public class Booking
    {
        public string? firstname { get; set; }
        public string? lastname { get; set; }
        public int totalprice { get; set; }
        public bool depositpaid { get; set; }
        public BookingDates? bookingdates { get; set; }
        public string? additionalneeds { get; set; }
    }

    public class BookingDates
    {
        public string? checkin { get; set; }
        public string? checkout { get; set; }
    }

    public class BookingResponse
    {
        public int bookingid { get; set; }
        public Booking? booking { get; set; }
    }

    public class BookingIdResponse
    {
        public int bookingid { get; set; }
    }

    public class AuthResponse
    {
        public string? token { get; set; }
    }
}