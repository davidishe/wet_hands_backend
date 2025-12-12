using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Core.Identity;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;


namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class OrderItemTypesController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<Order> _orderRepository;
    private readonly IDbRepository<OrderItem> _orderItemRepo;
    private readonly IDbRepository<OrderItemType> _orderItemTypeRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderItemTypesController> _logger;
    private readonly IGenericRepository<OrderItem> _orderItemSpecRepo;


    public OrderItemTypesController(
      UserManager<AppUser> userManager,
      IGenericRepository<OrderItem> orderItemSpecRepo,
      IDbRepository<Order> orderRepository,
      IDbRepository<OrderItemType> orderItemTypeRepo,
      IDbRepository<OrderItem> orderItemRepo,
      ILogger<OrderItemTypesController> logger,
      IMapper mapper)
    {
      _mapper = mapper;
      _logger = logger;
      _orderRepository = orderRepository;
      _orderItemRepo = orderItemRepo;
      _orderItemTypeRepo = orderItemTypeRepo;
      _userManager = userManager;
      _orderItemSpecRepo = orderItemSpecRepo;
    }



    [AllowAnonymous]
    [HttpGet]
    [Route("get_all")]
    public async Task<ActionResult> GetAll()
    {
      var types = _orderItemTypeRepo.GetAll().ToList();
      var mapped = _mapper.Map<IReadOnlyList<OrderItemType>, IReadOnlyList<OrderItemTypeDto>>(types);
      return Ok(mapped);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById([FromRoute] int id)
    {
      var type = await _orderItemTypeRepo.GetByIdAsync(id);
      if (type == null) return NotFound();

      var mapped = _mapper.Map<OrderItemType, OrderItemTypeDto>(type);
      return Ok(mapped);
    }

    [Authorize]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] OrderItemTypeDto dto)
    {
      if (dto == null) return BadRequest("Order item type payload is required.");

      var type = new OrderItemType
      {
        Weight = dto.Weight,
        Name = dto.Name
      };

      var created = await _orderItemTypeRepo.AddAsync(type);
      var mapped = _mapper.Map<OrderItemType, OrderItemTypeDto>(created);
      return Ok(mapped);
    }

    [Authorize]
    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] OrderItemTypeDto dto)
    {
      if (dto?.Id == null) return BadRequest("Order item type id is required.");

      var type = await _orderItemTypeRepo.GetByIdAsync(dto.Id.Value);
      if (type == null) return NotFound();

      type.Weight = dto.Weight;
      type.Name = dto.Name;

      await _orderItemTypeRepo.UpdateAsync(type);

      var mapped = _mapper.Map<OrderItemType, OrderItemTypeDto>(type);
      return Ok(mapped);
    }

    [Authorize]
    [HttpDelete]
    [Route("{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
      var type = await _orderItemTypeRepo.GetByIdAsync(id);
      if (type == null) return NotFound();

      await _orderItemTypeRepo.DeleteAsync(type);
      return Ok(200);
    }








  }
}
