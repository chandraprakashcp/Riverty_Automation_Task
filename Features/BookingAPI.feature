@API @Booking
Feature: Booking API Operations
    As a user of the booking system
    I want to manage bookings through the API
    So that I can perform CRUD operations on bookings

    StoryID: BOOKING-API-001
    JiraID: QA-247
    Date: 2025-01-15
    Created by: Automation Team
    Last Updated: 2025-01-15
    
    Description:
    Comprehensive acceptance tests for Restful-Booker Booking API
    covering all CRUD operations with positive and negative test scenarios

    API Coverage:
    - GET /booking - Retrieve all booking IDs
    - GET /booking/{id} - Fetch specific booking details
    - POST /booking - Create new bookings  
    - PUT /booking/{id} - Update existing bookings

Background:
    Given I set the base API URL to the restful-booker service

@GetBookingIds @Smoke
Scenario: Get all booking IDs
    When I send a GET request to the "booking" endpoint
    Then the response status code should be 200
    And the response should contain a list of booking IDs

@GetBooking @Reliable
Scenario: Get booking by creating it first
    Given I create a new booking with firstname "Test" and lastname "User"
    And I store the booking ID as "currentBookingId"
    When I send a GET request to the "booking/<bookingid>" endpoint
    Then the response status code should be 200
    And the response should contain the booking details for "Test" "User"
    And the created booking should have valid random data

@CreateBooking @Smoke
Scenario Outline: Create a new booking with valid data
    When I send a POST request to the "booking" endpoint with the following booking data:
        | firstname | lastname | totalprice | depositpaid | checkin     | checkout    | additionalneeds |
        | <firstname> | <lastname> | <totalprice> | <depositpaid> | <checkin> | <checkout> | <additionalneeds> |
    Then the response status code should be 200
    And the response should contain a booking ID
    And the response booking details should match the request data
    
    Examples:
    | firstname | lastname | totalprice | depositpaid | checkin     | checkout    | additionalneeds |
    | John      | Smith    | 200        | true         | 2024-01-01 | 2024-01-05 | Breakfast      |
    
@UpdateBooking @Authentication
Scenario Outline: Update an existing booking
    Given I create a new booking with firstname "Test" and lastname "User"
    And I store the booking ID as "createdBookingId"
    And I have obtained an authentication token
    When I send a PUT request to the "booking/<bookingid>" endpoint with the following updated data:
        | firstname | lastname | totalprice | depositpaid | checkin     | checkout    | additionalneeds |
        | <firstname> | <lastname> | <totalprice> | <depositpaid> | <checkin> | <checkout> | <additionalneeds> |
    Then the response status code should be 200
    And the response should contain the updated booking details
    
    Examples:
    | firstname    | lastname       | totalprice | depositpaid | checkin     | checkout    | additionalneeds |
    | UpdatedJohn  | UpdatedSmith   | 300        | false       | 2024-03-01 | 2024-03-05 | Lunch          |
    | UpdatedSarah | UpdatedJohnson | 450        | true        | 2024-04-15 | 2024-04-20 | Early Checkin  |

@Negative @GetBooking
Scenario Outline: Get booking with invalid ID should return appropriate error
    When I send a GET request to the "booking/<bookingid>" endpoint
    Then the response status code should be <expectedstatuscode>
    
    Examples:
    | bookingid | expectedstatuscode |
    | 999999    | 404                |
    | 0         | 404                |
    | -1        | 404                |

@Negative @CreateBooking
Scenario: Create booking with missing required fields should fail
    When I send a POST request to the "booking" endpoint with incomplete booking data:
        | firstname | lastname | totalprice |
        | John      |          | 200        |
    Then the response status code should be 500