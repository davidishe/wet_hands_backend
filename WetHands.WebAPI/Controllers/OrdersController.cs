using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using System;
using WetHands.Core.TonModels;
using Microsoft.Extensions.Logging;
using System.Linq;
using WetHands.Core;
using WetHands.Core.Models.Items;
using WetHands.Infrastructure.Specifications;
using Core.Models;
using WetHands.Identity.Extensions;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using WetHands.Core.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using WetHands.Infrastructure.Documents;


namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class OrdersController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<NftPlot> _nftorderRepository;
    private readonly IDbRepository<Order> _orderRepository;
    private readonly IGenericRepository<Core.Models.Items.File> _filesSpecRepository;
    private readonly IDbRepository<Core.Models.Items.File> _filesRepo;
    private readonly IDbRepository<TonLocalTransaction> _tonLocalTransactionRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<OrdersController> _logger;
    private readonly IDbRepository<OrderStatus> _orderStatusRepo;
    private readonly IGenericRepository<Order> _orderSpecRepo;
    private readonly IDbRepository<Company> _companyRepo;
    private readonly IDbRepository<OrderStatus> _plotStatusRepo;
    private readonly IGenericRepository<Favour> _favourRepo;
    private readonly IOrderDocumentService _orderDocumentService;


    public OrdersController(
      UserManager<AppUser> userManager,
      IGenericRepository<Favour> favourRepo,
      IGenericRepository<Core.Models.Items.File> filesSpecRepository,
      IGenericRepository<Order> orderSpecRepo,
      IDbRepository<Order> orderRepository,
      IDbRepository<Core.Models.Items.File> filesRepo,
      IDbRepository<Company> companyRepo,
      IDbRepository<NftPlot> nftorderRepository,
      IDbRepository<OrderStatus> orderStatusRepo,
      IDbRepository<OrderStatus> plotStatusRepo,
      IDbRepository<TonLocalTransaction> tonLocalTransactionRepo,
      ILogger<OrdersController> logger,
      IMapper mapper,
      IOrderDocumentService orderDocumentService)
    {
      // _projectsManager = projectsManager;
      _mapper = mapper;
      _logger = logger;
      _orderRepository = orderRepository;
      _nftorderRepository = nftorderRepository;
      _tonLocalTransactionRepo = tonLocalTransactionRepo;
      _userManager = userManager;
      _orderSpecRepo = orderSpecRepo;
      _orderStatusRepo = orderStatusRepo;
      _filesRepo = filesRepo;
      _filesSpecRepository = filesSpecRepository;
      _plotStatusRepo = plotStatusRepo;
      _companyRepo = companyRepo;
      _favourRepo = favourRepo;
      _orderDocumentService = orderDocumentService;
    }



    [AllowAnonymous]
    [HttpGet]
    [Route("all")]
    public async Task<ActionResult> GetAllPlots()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return Unauthorized();

      var orderSpec = new OrderSpecification(user.Id, true);
      var orders = await _orderSpecRepo.ListAsync(orderSpec);
      if (orders is null)
        return Ok(orders);
      var ordersToMap = orders.OrderByDescending(x => x.CreatedAt).ToList();

      var mappedPlots = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDto>>(ordersToMap);
      return Ok(mappedPlots);

    }

    [Authorize]
    [HttpGet]
    [Route("document/template")]
    public async Task<IActionResult> GetOrderDocumentTemplate(CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return Unauthorized();

      if (!user.IsAdmin)
        return Forbid();

      var bytes = await _orderDocumentService.GetTemplateAsync(cancellationToken);
      return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "order-summary-template.docx");
    }

    [Authorize]
    [HttpGet]
    [Route("{id}/document")]
    public async Task<IActionResult> GetOrderDocument([FromRoute] int id, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user is null)
        return Unauthorized();

      var order = await _orderRepository.GetByIdAsync(id);
      if (order is null)
        return NotFound();

      if (!user.IsAdmin && order.AuthorId != user.Id)
        return Forbid();

      var documentBytes = await _orderDocumentService.GenerateOrderSummaryAsync(id, cancellationToken);
      var fileName = $"order-{id}.docx";
      return File(documentBytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
    }

    [Authorize]
    [HttpGet]
    [Route("all/admin")]
    public async Task<ActionResult> GetAllPlotsForAdmin()
    {
      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user.IsAdmin == false)
        return Unauthorized();

      var orderSpec = new OrderSpecification();
      var plots = await _orderSpecRepo.ListAsync(orderSpec);
      if (plots is null)
        return Ok(plots);

      var ordersToMap = plots.OrderByDescending(x => x.CreatedAt).ToList();
      var mappedPlots = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDto>>(ordersToMap);
      return Ok(mappedPlots);

    }





    [Authorize]
    [HttpPost]
    [Route("create")]
    public async Task<ActionResult> CreateOrder()
    {

      var user = await _userManager.FindByClaimsCurrentUser(HttpContext.User);
      if (user.CompanyId is null)
        return BadRequest(new { Message = "Пользователь не привязан к отелю. Обратитесь к администратору приложения, чтобы привязать вас к отелю." });

      var newPlot = new Order()
      {
        AuthorId = user.Id,
        CompanyId = user.CompanyId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        OrderStatusId = 1,
      };

      var createdPlot = await _orderRepository.AddAsync(newPlot);
      return Ok(createdPlot);
    }





    [Authorize]
    [HttpPost]
    [Route("update/status")]
    public async Task<ActionResult> UdateStatus([FromBody] OrderDto orderDto)
    {

      var spec = new OrderSpecification((int)orderDto.Id);
      var orderForUpdate = await _orderSpecRepo.GetEntityWithSpec(spec);
      orderForUpdate.OrderStatusId = orderDto.OrderStatusId;
      orderForUpdate.DeliveryDate = orderDto.DeliveryDate;
      orderForUpdate.ContactNameText = orderDto.ContactNameText;
      orderForUpdate.ContactPhoneText = orderDto.ContactPhoneText;
      orderForUpdate.WeightKg = orderDto.WeightKg;
      orderForUpdate.BagsCount = orderDto.BagsCount;
      orderForUpdate.UpdatedAt = DateTime.UtcNow;
      await _orderRepository.UpdateAsync(orderForUpdate);
      return Ok(orderForUpdate);
    }



    [Authorize]
    [HttpGet]
    [Route("statuses/all")]
    public async Task<ActionResult> GetAllStatusForAdmin()
    {

      var statuses = _orderStatusRepo.GetAll().ToList();
      return Ok(statuses);
    }






    [AllowAnonymous]
    [HttpGet]
    [Route("get_by_id")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
      var spec = new OrderSpecification(id);
      var item = await _orderSpecRepo.GetEntityWithSpec(spec);
      var mappedItem = _mapper.Map<Order, OrderDto>(item);
      return Ok(mappedItem);
    }


    [Authorize]
    [HttpPost]
    [Route("update")]
    public async Task<ActionResult> Update([FromBody] OrderDto orderDto)
    {

      var plotForUpdate = await _orderRepository.GetByIdAsync((int)orderDto.Id);
      plotForUpdate.UpdatedAt = DateTime.UtcNow;
      plotForUpdate.OrderStatusId = orderDto.OrderStatusId;
      await _orderRepository.UpdateAsync(plotForUpdate);
      return Ok(200);
    }



    /// <summary>
    /// Get all plot lifecircle statuses
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("get_all_statuses")]
    public async Task<ActionResult> GetAllStatuses()
    {
      var statuses = _plotStatusRepo.GetAll();
      return Ok(statuses);
    }







    [Authorize]
    [HttpPost]
    [Route("update/invest_passport/{plotId}")]
    public async Task<ActionResult> UpdateInvestPassport([FromRoute] int plotId)
    {

      // var plotForUpdate = await _orderRepository.GetByIdAsync(plotId);
      Thread.Sleep(2000);
      var files = Request.Form.Files;
      foreach (var file in files)
      {
        if (file.Length > 0)
        {
          using (var memoryStream = new MemoryStream())
          {
            await file.CopyToAsync(memoryStream);
            var docByte = memoryStream.ToArray();
            var itemFile = new Core.Models.Items.File()
            {
              DocByte = docByte,
              FileType = file.ContentType,
              FileName = file.FileName,
              PlotId = plotId,
              Size = docByte.Length,
              EnrolledDate = DateTime.Now,
            };
            // plotForUpdate.InvestPassportDocByte = docByte;
            var createdFile = await _filesRepo.AddAsync(itemFile);
            return Ok(createdFile);

          }
        }
      }
      return Ok(200);


    }


    [AllowAnonymous]
    [HttpGet]
    [Route("files")]
    public async Task<ActionResult> GetItemFiles([FromQuery] int plotId)
    {
      var fileSpec = new FileSpecification(plotId);
      var files = await _filesSpecRepository.ListAsync(fileSpec);
      var mappedFiles = _mapper.Map<IReadOnlyList<Core.Models.Items.File>, IReadOnlyList<FileDto>>(files);
      return Ok(mappedFiles);

    }


    [AllowAnonymous]
    [HttpGet]
    [Route("file")]
    public async Task<ActionResult> GetFileById([FromQuery] int fileId)
    {
      Thread.Sleep(1500);
      // var fileSpec = new FileSpecification(fileId, true);
      var file = await _filesSpecRepository.GetByIdAsync(fileId);
      // var mappedFiles = _mapper.Map<IReadOnlyList<Core.Models.Items.File>, IReadOnlyList<FileDto>>(files);
      return Ok(file);
    }



    [Authorize]
    [HttpDelete]
    [Route("file/{fileId}")]
    public async Task<ActionResult> DeleteFileById([FromRoute] int fileId)
    {
      Thread.Sleep(1500);
      // var fileSpec = new FileSpecification(fileId, true);
      var file = await _filesRepo.GetByIdAsync(fileId);
      await _filesRepo.DeleteAsync(file);
      // var mappedFiles = _mapper.Map<IReadOnlyList<Core.Models.Items.File>, IReadOnlyList<FileDto>>(files);
      return Ok(file);
    }



  }
}
