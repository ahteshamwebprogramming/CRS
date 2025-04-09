using AutoMapper;
using CRS.Core.Entities;
using CRS.Core.Repository;
using CRS.Infrastructure.Models.Masters;
using CRS.Infrastructure.Models.Rooms;
using CRS.Infrastructure.ViewModels.Rooms;
using CRS.Infrastructure.ViewModels.Summary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRS.Endpoints.Rooms;

[Route("api/[controller]")]
[ApiController]
public class RoomsAPIController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RoomsAPIController> _logger;
    private readonly IMapper _mapper;

    public RoomsAPIController(IUnitOfWork unitOfWork, ILogger<RoomsAPIController> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IActionResult> RoomTypes(RoomAllocationDTO inputDTO)
    {
        try
        {

            //string query = @"Select 
            //                *
            //                ,(Select top 1 r.Remarks from Rooms r where r.RTypeID=rt.ID and r.Status=1) Remarks
            //                ,(Select count(r.RNumber) from Rooms r where r.RTypeID=rt.ID and r.Status=1) NoOfRooms
            //                from RoomType rt where Status=1";
            string query = @"declare @StartDate datetime=@StartDate1,@EndDate datetime=@EndDate1;

                            Select 
                            *
                            ,rt.Remarks Remarks
                            ,(Select count(1) from Rooms r where r.RTypeID=rt.ID and Status=1 and r.RNumber not in (SELECT ra.RNumber FROM RoomAllocation ra WHERE ra.IsActive = 1
                                    AND (
                                      (ra.CheckInDate IS NOT NULL AND ra.CheckOutDate IS NOT NULL AND (ra.CheckInDate <= @EndDate AND ra.CheckOutDate >= @StartDate) )
                                      OR
                                      (ra.CheckInDate IS NOT NULL AND ra.CheckOutDate IS NULL AND (ra.CheckInDate <= @EndDate) )
                                      OR
                                      ( ra.CheckInDate IS NULL AND ra.CheckOutDate IS NULL AND (ra.FD <= @EndDate AND ra.TD >= @StartDate))
                                    ))) NoOfRooms
                            ,(SELECT TOP 1 r.Price FROM Rates r WHERE r.RoomTypeId =rt.ID  AND r.[Date] <= @StartDate ORDER BY r.[Date] DESC) OfferedPrice
                            ,(SELECT TOP 1 r.MaxRate FROM Rates r WHERE r.RoomTypeId = rt.ID AND r.[Date] <= @StartDate ORDER BY r.[Date] DESC) ActualPrice
                            ,CASE WHEN (SELECT TOP 1 r.MaxRate FROM Rates r WHERE r.RoomTypeId = rt.ID AND r.[Date] <= @StartDate ORDER BY r.[Date] DESC) > 0 THEN ROUND(100.0 * ((SELECT TOP 1 r.MaxRate FROM Rates r WHERE r.RoomTypeId = rt.ID AND r.[Date] <= @StartDate ORDER BY r.[Date] DESC) - (SELECT TOP 1 r.Price FROM Rates r WHERE r.RoomTypeId = rt.ID AND r.[Date] <= @StartDate ORDER BY r.[Date] DESC)) / (SELECT TOP 1 r.MaxRate FROM Rates r WHERE r.RoomTypeId = rt.ID AND r.[Date] <= @StartDate ORDER BY r.[Date] DESC), 2 ) ELSE 0 END AS OfferPercentage
                            from RoomType rt where Status=1  order by rt.RoomRank desc";
            var param = new { @StartDate1 = inputDTO.CheckInDate?.ToString("yyyy-MM-dd"), @EndDate1 = inputDTO.CheckOutDate?.ToString("yyyy-MM-dd") };
            var res = await _unitOfWork.RoomType.GetTableData<RoomTypeWithChild>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RoomTypes)}");
            throw;
        }
    }
    public async Task<IActionResult> GetRoomCalenderPrice(RoomAllocationDTO inputDTO)
    {
        try
        {
            if (inputDTO == null)
            {
                return BadRequest("Invalid dates");
            }
            string query = @"Declare @StartDate datetime=@StartDate1,@EndDate datetime=@EndDate1,@RoomTypeId int=@RoomTypeId1;

                            WITH DateRange AS (
                                SELECT 
                                    DATEADD(DAY, number, @StartDate) AS [Date]
                                FROM 
                                    master..spt_values
                                WHERE 
                                    type = 'P'
                                    AND DATEADD(DAY, number, @StartDate) <= @EndDate
                            ),
                            EarliestPriceForDate AS (
                                SELECT
                                    dr.[Date],
                                    (SELECT TOP 1 r.Price FROM Rates r WHERE r.RoomTypeId = @RoomTypeId AND r.[Date] <= dr.[Date] ORDER BY r.[Date] DESC) AS EarliestPrice
                                FROM 
                                    DateRange dr
                            )

                            SELECT dr.[Date],@RoomTypeId AS RoomTypeId, Convert(decimal(10,2),ep.EarliestPrice) AS Price 
                            ,(Select count(1) from Rooms r where r.RTypeID=@RoomTypeId and Status=1 and r.RNumber not in (SELECT ra.RNumber FROM RoomAllocation ra WHERE ra.IsActive = 1
                                                                AND (
                                                                  (ra.CheckInDate IS NOT NULL AND ra.CheckOutDate IS NOT NULL AND (ra.CheckInDate <= dr.[Date] AND ra.CheckOutDate >= dr.[Date]) )
                                                                  OR
                                                                  (ra.CheckInDate IS NOT NULL AND ra.CheckOutDate IS NULL AND (ra.CheckInDate <= dr.[Date]) )
                                                                  OR
                                                                  ( ra.CheckInDate IS NULL AND ra.CheckOutDate IS NULL AND (ra.FD <= dr.[Date] AND ra.TD >= dr.[Date]))
                                                                )))RoomsAvailable
                            FROM  DateRange dr LEFT JOIN EarliestPriceForDate ep ON dr.[Date] = ep.[Date] ORDER BY dr.[Date] ASC;";

            inputDTO.CheckOutDate = inputDTO?.CheckInDate?.AddDays(15);
            var param = new { @StartDate1 = inputDTO?.CheckInDate?.ToString("yyyy-MM-dd"), @EndDate1 = inputDTO?.CheckOutDate?.ToString("yyyy-MM-dd"), @RoomTypeId1 = inputDTO?.Rtype };
            var res = await _unitOfWork.RoomType.GetTableData<RoomCalenderPrice>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RoomTypes)}");
            throw;
        }
    }
    public async Task<IActionResult> Services()
    {
        try
        {

            string query = @"Select * from Services where Status=1";
            var res = await _unitOfWork.RoomType.GetTableData<ServicesDTO>(query);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RoomTypes)}");
            throw;
        }
    }
    public async Task<IActionResult> GetSummary(SummaryInputDTO inputDTO)
    {
        try
        {

            string query = @"Select * from roomtype where id=@Id";
            var param = new { @Id = inputDTO.RoomType };
            var res = await _unitOfWork.RoomType.GetEntityData<RoomTypeDTO>(query, param);
            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in retriving Attendance {nameof(RoomTypes)}");
            throw;
        }
    }
}
