using CRS.Endpoints.Rooms;
using CRS.Infrastructure.Interface;
using CRS.Infrastructure.Models.Masters;
using CRS.Infrastructure.Models.Rooms;
using CRS.Infrastructure.ViewModels.Rooms;
using CRS.Infrastructure.ViewModels.Summary;
using CRS.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;


namespace CRS.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoomsAPIController _roomsAPIController;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, RoomsAPIController roomsAPIController,IEmailService emailService)
        {
            _logger = logger;
            _roomsAPIController = roomsAPIController;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> RoomTypeGridPartialView([FromBody] RoomAllocationDTO inputDTO)
        {
            RoomsViewModel dto = new RoomsViewModel();
            var res = await _roomsAPIController.RoomTypes(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomTypesWithAttr = (List<RoomTypeWithChild>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            //var resService = await _roomsAPIController.Services();
            //if (resService != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resService).StatusCode == 200)
            //{
            //    dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resService).Value;
            //}
            return PartialView("_index/_roomTypeGridView", dto);
        }
        public async Task<IActionResult> PackagesPartialView()
        {
            RoomsViewModel dto = new RoomsViewModel();

            var resService = await _roomsAPIController.Services();
            if (resService != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resService).StatusCode == 200)
            {
                dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resService).Value;
            }
            return PartialView("_index/_packages", dto);
        }


        public async Task<IActionResult> SummaryPartialView([FromBody] SummaryInputDTO inputDTO)
        {
            SummaryInputDTO sessionInputDTO = new();
            var sessionData = HttpContext.Session.GetString("Summary");
            if (!string.IsNullOrEmpty(sessionData))
            {
                sessionInputDTO = JsonConvert.DeserializeObject<SummaryInputDTO>(sessionData) ?? new SummaryInputDTO();
            }

            if (inputDTO.CheckInDate.HasValue)
                sessionInputDTO.CheckInDate = inputDTO.CheckInDate;
            if (inputDTO.CheckOutDate.HasValue)
                sessionInputDTO.CheckOutDate = inputDTO.CheckOutDate;

            if (sessionInputDTO.CheckInDate.HasValue && sessionInputDTO.CheckOutDate.HasValue)
                sessionInputDTO.NoOfNights = (sessionInputDTO.CheckOutDate.Value - sessionInputDTO.CheckInDate.Value).Days;

            if (inputDTO.RoomSelections != null && inputDTO.RoomSelections.Any())
            {
                sessionInputDTO.SelectedRooms.Clear(); 

                foreach (var key in inputDTO.RoomSelections.Keys)
                {
                    if (int.TryParse(key, out int roomId))
                    {
                        var res = await _roomsAPIController.GetSummary(new SummaryInputDTO { RoomType = roomId });
                        var room = (res as ObjectResult)?.Value as RoomTypeDTO;
                        if (room != null)
                        {
                            var selection = inputDTO.RoomSelections[key];
                            room.NoOfRooms = selection.Count;
                            room.PaxCount = selection.Count;
                            room.Price = selection.Price;

                           
                            if (inputDTO.PaxPerRoom != null)
                            {
                                
                                var pax = inputDTO.PaxPerRoom.FirstOrDefault(p =>p.RoomNumber.ToString() == key ||  p.RoomNumber == roomId);
                                if (pax != null)
                                {
                                    room.Adults = pax.Adults;
                                    room.Children = pax.Children;
                                }
                                else
                                {
                                    
                                    room.Adults = 1;
                                    room.Children = 0;
                                }
                            }

                            sessionInputDTO.SelectedRooms.Add(room);
                        }
                    }
                }

                sessionInputDTO.RoomSelections = inputDTO.RoomSelections;
            }

            sessionInputDTO.PaxPerRoom = inputDTO.PaxPerRoom;
            HttpContext.Session.SetString("Summary", JsonConvert.SerializeObject(sessionInputDTO));

            var viewModel = new SummaryViewModel
            {
                SummaryInputDTO = sessionInputDTO
            };

            return PartialView("_index/_summary", viewModel);
        }

        public async Task<IActionResult> PackagesDateWisePartialView([FromBody] RoomAllocationDTO inputDTO)
        {
            RoomsViewModel dto = new RoomsViewModel();

            var resService = await _roomsAPIController.Services();
            if (resService != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resService).StatusCode == 200)
            {
                dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resService).Value;
            }

            return PartialView("_index/_packagesDateWise", dto);
        }
        public async Task<IActionResult> AvailabilityCalenderPartialView([FromBody] RoomAllocationDTO inputDTO)
        {
            RoomsViewModel dto = new RoomsViewModel();
            var res = await _roomsAPIController.GetRoomCalenderPrice(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomCalenderPrices = (List<RoomCalenderPrice>?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            //var resService = await _roomsAPIController.Services();
            //if (resService != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resService).StatusCode == 200)
            //{
            //    dto.Services = (List<ServicesDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resService).Value;
            //}
            return PartialView("_index/_availabilityCalender", dto);
        }
        public IActionResult Index1()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Pax([FromBody] List<PaxInfo> paxData)
        {
            var summaryJson = HttpContext.Session.GetString("Summary");
            int totalPax = paxData.Sum(p => p.Adults + p.Children);
            TempData["TotalPax"] = totalPax;
            return Json(new { redirectUrl = Url.Action("Booking") });
        }

       
        public IActionResult Booking()
        {
            if (TempData["TotalPax"] is int totalPax)
            {
                ViewBag.TotalPax = totalPax;
                
                TempData.Keep("TotalPax");
            }
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
            }
            var res = await _roomsAPIController.GetSummary(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomTypeDTO = (RoomTypeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return PartialView("_index/_summary", dto);
        }
        [HttpPost]
        public IActionResult SubmitBooking(BookingDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });
            HttpContext.Session.SetString("BookingData", JsonConvert.SerializeObject(model));
            return Json(new { success = true, redirectUrl = Url.Action("ReviewPayment", "Home") });
        }
        public IActionResult ReviewPayment()
        {
            var bookingJson = HttpContext.Session.GetString("BookingData");
            var summaryJson = HttpContext.Session.GetString("Summary");
            var summary = JsonConvert.DeserializeObject<SummaryInputDTO>(summaryJson);
            var booking = JsonConvert.DeserializeObject<BookingDTO>(bookingJson);
            var viewModel = new BookingReviewViewModel
            {
                Booking = booking,
                Summary = summary
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            var code = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("VerificationCode", code);
            HttpContext.Session.SetString("VerificationEmail", email);
            var subject = "Your Email Verification Code";
            var body = $"Your verification code is: {code}";
            try
            {
                await _emailService.SendAsync(email, subject, body);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult VerifyEmailCode(string email, string code)
        {
            var storedCode = HttpContext.Session.GetString("VerificationCode");
            var storedEmail = HttpContext.Session.GetString("VerificationEmail");

            if (!string.IsNullOrEmpty(storedCode) && storedCode == code && storedEmail == email)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        
    }
}
