using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using WetHands.Core;
using WetHands.Core.Models.Items;
using WetHands.Infrastructure.Specifications;
using WetHands.Identity.Extensions;
using System.Collections.Generic;


namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class OrderItemsController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<NftPlot> _nftPlotRepository;
    private readonly IDbRepository<Order> _plotRepository;
    private readonly IDbRepository<OrderItem> _orderItemRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderItemsController> _logger;
    private readonly IGenericRepository<OrderItem> _orderItemSpecRepo;


    public OrderItemsController(
      UserManager<AppUser> userManager,
      IGenericRepository<OrderItem> orderItemSpecRepo,
      IDbRepository<Order> plotRepository,
      IDbRepository<Picture> pictureRepo,
      IDbRepository<OrderItem> orderItemRepo,
      ILogger<OrderItemsController> logger,
      IMapper mapper)
    {
      _mapper = mapper;
      _logger = logger;
      _plotRepository = plotRepository;
      _orderItemRepo = orderItemRepo;
      _userManager = userManager;
      _orderItemSpecRepo = orderItemSpecRepo;
    }



    [AllowAnonymous]
    [HttpGet]
    [Route("get_by_order_id/{orderId}")]
    public async Task<ActionResult> GetByOrderId([FromRoute] int orderId)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      var spec = new OrderItemSpecification(orderId, true);
      var plots = await _orderItemSpecRepo.ListAsync(spec);
      if (plots is null)
        return Ok(plots);

      // var res = plots.ToReadOnlyList();
      var mappedPlots = _mapper.Map<IReadOnlyList<OrderItem>, IReadOnlyList<OrderItem>>(plots);
      return Ok(mappedPlots);

    }




    /// <summary>
    /// Get all nfts for specifice plot
    /// </summary>
    /// <param name="plotId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> Create([FromBody] OrderItem orderItem)
    {
      var createdOrderItem = await _orderItemRepo.AddAsync(orderItem);
      return Ok(createdOrderItem);
    }


    /// <summary>
    /// Get all nfts for specifice plot
    /// </summary>
    /// <param name="plotId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] OrderItem orderItem)
    {
      var oi = await _orderItemRepo.GetByIdAsync(orderItem.Id);
      oi.Bags = orderItem.Bags;
      oi.QtyDirty = orderItem.QtyDirty;
      oi.BagsClean = orderItem.BagsClean;
      oi.QtyClean = orderItem.QtyClean;
      oi.OrderItemTypeId = orderItem.OrderItemTypeId;
      oi.CommentText = orderItem.CommentText;
      oi.Name = orderItem.Name;
      await _orderItemRepo.UpdateAsync(oi);
      return Ok(250);
    }





  }
}