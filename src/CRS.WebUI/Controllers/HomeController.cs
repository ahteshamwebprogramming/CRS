using CRS.Endpoints.Rooms;
using CRS.Infrastructure.Models.Masters;
using CRS.Infrastructure.Models.Rooms;
using CRS.Infrastructure.ViewModels.Rooms;
using CRS.Infrastructure.ViewModels.Summary;
using CRS.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CRS.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RoomsAPIController _roomsAPIController;

        public HomeController(ILogger<HomeController> logger, RoomsAPIController roomsAPIController)
        {
            _logger = logger;
            _roomsAPIController = roomsAPIController;
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
            SummaryViewModel dto = new SummaryViewModel();
            dto.SummaryInputDTO = inputDTO;

            var res = await _roomsAPIController.GetSummary(inputDTO);
            if (res != null && ((Microsoft.AspNetCore.Mvc.ObjectResult)res).StatusCode == 200)
            {
                dto.RoomTypeDTO = (RoomTypeDTO?)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;
            }
            return PartialView("_index/_summary", dto);
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
    }
}
