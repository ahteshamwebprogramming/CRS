using CRS.Endpoints.Rooms;
using CRS.Infrastructure.Interface;
using CRS.Infrastructure.Models.Masters;
using CRS.Infrastructure.ViewModels.Summary;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace CRS.WebUI.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IEmailService _emailService;
        private readonly RoomsAPIController _roomsAPIController;

        private readonly IHttpClientFactory _httpClientFactory;
        public ReservationController(ILogger<ReservationController> logger, RoomsAPIController roomsAPIController, IEmailService emailService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _roomsAPIController = roomsAPIController;
            _emailService = emailService;
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Confirmation()
        {
            if (TempData["TotalPax"] is int totalPax)
            {
                ViewBag.TotalPax = totalPax;

                TempData.Keep("TotalPax");
            }

            BookingReviewViewModel dto = new BookingReviewViewModel();

            dto.Summary = new SummaryInputDTO
            {
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(2),
                NoOfNights = 2,
                NoOfRooms = 1,
                PaxPerRoom = new List<PaxInfo> { new PaxInfo { Adults = 2 } }
            };
            //dto.Booking = new BookingDTO { Address = "NA", City = "Dehradun", Country = "India", Guests = new List<GuestDetailsDTO> { new GuestDetailsDTO { FirstName = "Mohd", LastName = "Ahtesham" } } };
            dto.Booking = new BookingDTO { Address = "NA", City = "Dehradun", Country = "India", CountryId = 1, CityId = 1, Guests = new List<GuestDetailsDTO> { new GuestDetailsDTO { FirstName = "Mohd", LastName = "Ahtesham" } } };
            return View(dto);
        }
        public IActionResult Confirmation1()
        {
            if (TempData["TotalPax"] is int totalPax)
            {
                ViewBag.TotalPax = totalPax;

                TempData.Keep("TotalPax");
            }

            BookingReviewViewModel dto = new BookingReviewViewModel();

            dto.Summary = new SummaryInputDTO
            {
                CheckInDate = DateTime.Now,
                CheckOutDate = DateTime.Now.AddDays(2),
                NoOfNights = 2,
                NoOfRooms = 1,
                PaxPerRoom = new List<PaxInfo> { new PaxInfo { Adults = 2 } }
            };
            dto.Booking = new BookingDTO { Address = "NA", City = "Dehradun", Country = "India", CountryId = 1, CityId = 1, Guests = new List<GuestDetailsDTO> { new GuestDetailsDTO { FirstName = "Mohd", LastName = "Ahtesham" } } };

            return View(dto);
        }

        public IActionResult ConfirmGuest([FromBody] BookingDTO model)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadSummaryFromSession()
        {
            var summaryJson = HttpContext.Session.GetString("Summary");
            if (string.IsNullOrEmpty(summaryJson))
                return BadRequest("Summary not found in session.");

            var inputDTO = JsonConvert.DeserializeObject<SummaryInputDTO>(summaryJson);
            SummaryViewModel dto = new SummaryViewModel();
            dto.SummaryInputDTO = inputDTO;
            if (inputDTO.CheckInDate.HasValue && inputDTO.CheckOutDate.HasValue)
            {
                inputDTO.NoOfNights = (inputDTO.CheckOutDate.Value - inputDTO.CheckInDate.Value).Days;
                if (inputDTO.NoOfNights <= 0)
                    inputDTO.NoOfNights = 1;
            }
            var res = await _roomsAPIController.GetSummary(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomTypeDTO = (RoomTypeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return PartialView("_reservation/_summary", dto);
        }



        [HttpPost]
        public async Task<IActionResult> PostBookingData()
        {
            var bookingJson = HttpContext.Session.GetString("BookingData");
            var summaryJson = HttpContext.Session.GetString("Summary1");

            if (string.IsNullOrEmpty(bookingJson) || string.IsNullOrEmpty(summaryJson))
            {
                return BadRequest(new { success = false, message = "Session data missing" });
            }

            var booking = JsonConvert.DeserializeObject<BookingDTO>(bookingJson);
            var summary = JsonConvert.DeserializeObject<SummaryViewModelNew>(summaryJson);

            var payload = new { booking, summary };

            try
            {
                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                // TODO: replace placeholder URL with actual endpoint of external tool
                var response = await client.PostAsync("https://example.com/api/booking", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("External API returned status {StatusCode}", response.StatusCode);
                    return StatusCode((int)response.StatusCode, new { success = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to post booking data");
                return StatusCode(500, new { success = false });
            }

            return Ok(new { success = true, redirectUrl = Url.Action("PaymentSuccess", "Home") });
        }



    }
}
