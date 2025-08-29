using CRS.Endpoints.Rooms;
using CRS.Infrastructure.Interface;
using CRS.Infrastructure.Models.Masters;
using CRS.Infrastructure.Models.Rooms;
using CRS.Infrastructure.ViewModels.Rooms;
using CRS.Infrastructure.ViewModels.Summary;
using CRS.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Newtonsoft.Json;
using RestSharp;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;


namespace CRS.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoomsAPIController _roomsAPIController;
        private readonly IEmailService _emailService;

        public HomeController(ILogger<HomeController> logger, RoomsAPIController roomsAPIController, IEmailService emailService)
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
            if (inputDTO == null)
            {
                return PartialView("_index/_summary", new SummaryViewModel());
            }


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
                                var pax = inputDTO.PaxPerRoom.FirstOrDefault(p => p.RoomNumber.ToString() == key || p.RoomNumber == roomId);
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

            // Update Selected Services
            if (inputDTO.SelectedServices != null && inputDTO.SelectedServices.Any())
            {
                sessionInputDTO.SelectedServices = inputDTO.SelectedServices;
            }

            sessionInputDTO.PaxPerRoom = inputDTO.PaxPerRoom;
            HttpContext.Session.SetString("Summary", JsonConvert.SerializeObject(sessionInputDTO));

            var viewModel = new SummaryViewModel
            {
                SummaryInputDTO = sessionInputDTO
            };

            return PartialView("_index/_summary", viewModel);


        }

        public async Task<IActionResult> SummaryPartialView1([FromBody] SummaryViewModelNew inputDTO)
        {
            if (inputDTO == null)
            {
                return PartialView("_index/_summary", new SummaryViewModel());
            }

            SummaryViewModelNew sessionInputDTO = new();
            var sessionDataNew = HttpContext.Session.GetString("Summary1");
            if (!string.IsNullOrEmpty(sessionDataNew))
            {
                sessionInputDTO = JsonConvert.DeserializeObject<SummaryViewModelNew>(sessionDataNew) ?? new SummaryViewModelNew();
            }

            if (inputDTO.CheckInDate.HasValue)
                sessionInputDTO.CheckInDate = inputDTO.CheckInDate;
            if (inputDTO.CheckOutDate.HasValue)
                sessionInputDTO.CheckOutDate = inputDTO.CheckOutDate;

            if (sessionInputDTO.CheckInDate.HasValue && sessionInputDTO.CheckOutDate.HasValue)
                sessionInputDTO.NoOfNights = (sessionInputDTO.CheckOutDate.Value - sessionInputDTO.CheckInDate.Value).Days;

            if (inputDTO.RoomSelectionList != null && inputDTO.RoomSelectionList.Any())
            {
                foreach (var item in inputDTO.RoomSelectionList)
                {
                    RoomDetails? roomDetails = item.RoomDetails;
                    if (roomDetails != null)
                    {
                        var res = await _roomsAPIController.GetSummary(new SummaryInputDTO { RoomType = roomDetails.RoomTypeId });
                        var room = (res as ObjectResult)?.Value as RoomTypeDTO;
                        roomDetails.RoomTypeName = room?.Rtype;
                    }
                    item.RoomDetails = roomDetails;
                    // Update Selected Services
                    if (inputDTO.SelectedServices != null && inputDTO.SelectedServices.Any())
                    {
                        item.SelectedServices = inputDTO.SelectedServices;
                    }
                }
            }
            //sessionInputDTO.RoomSelectionList = inputDTO.RoomSelectionList;


            HttpContext.Session.SetString("Summary1", JsonConvert.SerializeObject(sessionInputDTO));


            //var viewModel = new SummaryViewModelNew
            //{
            //    CheckInDate = inputDTO.CheckInDate,
            //    CheckOutDate = inputDTO.CheckOutDate,
            //    RoomSelectionList = inputDTO.RoomSelectionList
            //};
            var viewModel = sessionInputDTO;
            return PartialView("_index/_summary1", viewModel);
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
        public async Task<IActionResult> AvailabilityCalenderPartialView([FromBody] AvailabilityCalender inputDTO)
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

            //var inputDTO = JsonConvert.DeserializeObject<SummaryInputDTO>(summaryJson);
            //SummaryViewModel dto = new SummaryViewModel();
            //dto.SummaryInputDTO = inputDTO;
            //if (inputDTO.CheckInDate.HasValue && inputDTO.CheckOutDate.HasValue)
            //{
            //    inputDTO.NoOfNights = (inputDTO.CheckOutDate.Value - inputDTO.CheckInDate.Value).Days;
            //}
            //var res = await _roomsAPIController.GetSummary(inputDTO);
            //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            //{
            //    dto.RoomTypeDTO = (RoomTypeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            //}

            SummaryViewModel dto = await GetDataForLoadSummaryFromSession();

            return PartialView("_index/_summary", dto);
        }

        [HttpGet]
        public async Task<IActionResult> LoadSummaryFromSessionViewOnly()
        {
            var summaryJson = HttpContext.Session.GetString("Summary");
            if (string.IsNullOrEmpty(summaryJson))
                return BadRequest("Summary not found in session.");

            SummaryViewModel dto = await GetDataForLoadSummaryFromSession();

            return PartialView("_index/_summaryViewOnly", dto);
        }

        [HttpGet]
        public async Task<IActionResult> LoadSummaryFromSessionViewOnly1()
        {
            var summaryJson = HttpContext.Session.GetString("Summary");
            if (string.IsNullOrEmpty(summaryJson))
                return BadRequest("Summary not found in session.");

            SummaryViewModelNew dto = await GetDataForLoadSummaryFromSession1();

            return PartialView("_index/_summaryViewOnly1", dto);
        }

        [HttpGet]
        public async Task<SummaryViewModelNew> GetDataForLoadSummaryFromSession1()
        {
            SummaryViewModelNew? dto = new SummaryViewModelNew();
            var summaryJson = HttpContext.Session.GetString("Summary1");
            if (!String.IsNullOrEmpty(summaryJson))
            {
                var inputDTO = JsonConvert.DeserializeObject<SummaryViewModelNew>(summaryJson);
                dto = inputDTO;
                if (inputDTO != null)
                {
                    if (inputDTO.CheckInDate.HasValue && inputDTO.CheckOutDate.HasValue)
                    {
                        inputDTO.NoOfNights = (inputDTO.CheckOutDate.Value - inputDTO.CheckInDate.Value).Days;
                    }
                    //var res = await _roomsAPIController.GetSummary(inputDTO);
                    //if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
                    //{
                    //    dto.RoomTypeDTO = (RoomTypeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
                    //}
                }
            }
            return dto;
        }
        [HttpGet]
        public async Task<SummaryViewModel> GetDataForLoadSummaryFromSession()
        {
            var summaryJson = HttpContext.Session.GetString("Summary");

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
            return dto;
        }
        [HttpPost]
        public IActionResult SubmitBooking([FromBody] BookingDTO model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });
            HttpContext.Session.SetString("BookingData", JsonConvert.SerializeObject(model));
            return Json(new { success = true, redirectUrl = Url.Action("ReviewPayment", "Home") });
        }
        public IActionResult ReviewPayment1()
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
        public IActionResult ReviewPayment()
        {
            var bookingJson = HttpContext.Session.GetString("BookingData");
            var summaryJson = HttpContext.Session.GetString("Summary");
            var summary = JsonConvert.DeserializeObject<SummaryInputDTO>(summaryJson);
            var booking = JsonConvert.DeserializeObject<BookingDTO>(bookingJson);
            var booking1 = summary.BookingDetails;
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

        public async Task<IActionResult> GetTaskPackage()
        {
            RoomsViewModel dto = new RoomsViewModel();

            var resService = await _roomsAPIController.GetTask();
            if (resService != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)resService).StatusCode == 200)
            {
                dto.TaskPackage = (List<TaskDTO>?)((Microsoft.AspNetCore.Mvc.ObjectResult)resService).Value;
            }
            return PartialView("_index/_TaskPackage", dto);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPaymentAndSave([FromBody] BookingDTO model)
        {

            return Ok();
            SummaryInputDTO sessionInputDTO = new();
            var sessionData = HttpContext.Session.GetString("Summary");

            if (!string.IsNullOrEmpty(sessionData))
            {
                sessionInputDTO = JsonConvert.DeserializeObject<SummaryInputDTO>(sessionData) ?? new SummaryInputDTO();
            }

            sessionInputDTO.BookingDetails = model;
            HttpContext.Session.SetString("Summary", JsonConvert.SerializeObject(sessionInputDTO));




            return Ok(new
            {
                success = true,
                redirectUrl = Url.Action("Confirmation", "Reservation")
            });


            return Ok("");
            // decimal totalPrice = model.TotalPrice;
            //string invoiceNumber = "INV" + DateTime.UtcNow.Ticks; 

            //var (isSuccess, responseContent) = await _valorPaymentService.ProcessPaymentAsync(totalPrice, invoiceNumber);

            //if (!isSuccess)
            //{
            //    return BadRequest(new { success = false, message = "Payment failed", details = responseContent });
            //}

            // Save the booking data
            //  await _reservationService.SaveBookingAsync(model);

            //return Ok(new { success = true, message = "Payment successful", valorResponse = responseContent });

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(new { success = false, message = "Invalid input." });
            //}

            // Step 1: Prepare the payment request payload
            var txnId = Guid.NewGuid().ToString(); // e.g., "INV001"

            //var requestPayload = new ValorRequest
            //{
            //    appid = "h6tSvf0Cz7fSB86B44bkLEplS15AsWYo",
            //    appkey = "zOKj0LUOiW4L9jFRrMbMiJQ5KxQEpSMm",
            //    epi = "2412333540",
            //    txn_type = "vc_publish",
            //    channel_id = "b16ccf618612e56da187c47bb39fe1a9",
            //    version = "1",
            //    payload = new ValorPayload
            //    {
            //        TRAN_MODE = "1",
            //        TRAN_CODE = "1",
            //        AMOUNT = model.TotalPrice.ToString("F2"),
            //        REQ_TXN_ID = txnId
            //    }
            //};


            //var jsonPayload = System.Text.Json.JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = null
            //});

            try
            {
                var client = new RestClient("https://securelink-staging.valorpaytech.com:4430/?status=");
                var request = new RestRequest("", RestSharp.Method.Post);
                request.AddHeader("accept", "application/json");

                // request.AddStringBody(jsonPayload, DataFormat.Json);

                var response = await client.ExecuteAsync(request);



                //if (!response.IsSuccessful)
                if (false)
                {
                    return BadRequest(new { success = false, message = "Payment API failed." });
                }

                var content = response.Content;
                //if (string.IsNullOrEmpty(content) || !content.Contains("approved", StringComparison.OrdinalIgnoreCase))
                if (false)
                {
                    return BadRequest(new { success = false, message = "Payment not approved." });
                }

                return Ok(new
                {
                    success = true,
                    redirectUrl = Url.Action("ConfirmGuest", "Reservation")
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Server error", error = ex.Message });
            }
        }



        public class ValorPayload
        {
            public string TRAN_MODE { get; set; }
            public string TRAN_CODE { get; set; }
            public string AMOUNT { get; set; }
            public string REQ_TXN_ID { get; set; }
        }

        public class ValorRequest
        {
            public string appid { get; set; }
            public string appkey { get; set; }
            public string epi { get; set; }
            public string txn_type { get; set; }
            public string channel_id { get; set; }
            public string version { get; set; }
            public ValorPayload payload { get; set; }
        }

    }
}
