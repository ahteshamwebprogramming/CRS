using AutoMapper;
using CRS.Core.Repository;
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
}
