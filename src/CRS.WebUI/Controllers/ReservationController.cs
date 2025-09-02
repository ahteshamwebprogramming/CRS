using CRS.Endpoints.Rooms;
using CRS.Infrastructure.Interface;
using CRS.Infrastructure.Models.Masters;
using CRS.Infrastructure.ViewModels.Summary;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CRS.WebUI.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ILogger<ReservationController> _logger;
        private readonly IEmailService _emailService;
        private readonly RoomsAPIController _roomsAPIController;

        public ReservationController(ILogger<ReservationController> logger, RoomsAPIController roomsAPIController, IEmailService emailService)
        {
            _logger = logger;
            _roomsAPIController = roomsAPIController;
            _emailService = emailService;
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
            dto.Booking = new BookingDTO { Address = "NA", City = "Dehradun", Country = "India", Guests = new List<GuestDetailsDTO> { new GuestDetailsDTO { FirstName = "Mohd", LastName = "Ahtesham" } } };

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
            dto.Booking = new BookingDTO { Address = "NA", City = "Dehradun", Country = "India", Guests = new List<GuestDetailsDTO> { new GuestDetailsDTO { FirstName = "Mohd", LastName = "Ahtesham" } } };

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
    }
}
