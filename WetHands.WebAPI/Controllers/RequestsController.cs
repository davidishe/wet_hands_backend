using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WetHands.Core.TonModels;
using Microsoft.Extensions.Logging;
using WetHands.Core;
using Microsoft.Extensions.Configuration;
using WetHands.Infrastructure.Specifications;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class RequestsController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<TonLocalTransaction> _tonLocalTransactionRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<RequestsController> _logger;
    private readonly IDbRepository<Order> _plotRepository;
    private readonly IDbRepository<NftPlot> _nftPlotRepo;
    private readonly IGenericRepository<PaymentRequest> _paymentRequestRepo;
    private readonly string _trxPreseedJettonMasterAddressText;
    private readonly IDbRepository<UsdtTransferRequest> _tokenRequestsRepo;



    public RequestsController(
      UserManager<AppUser> userManager,
      ILogger<RequestsController> logger,
      IDbRepository<TonLocalTransaction> tonLocalTransactionRepo,
      IDbRepository<Order> plotRepository,
      IDbRepository<NftPlot> nftPlotRepo,
      IDbRepository<UsdtTransferRequest> tokenRequestsRepo,
      IGenericRepository<PaymentRequest> paymentRequestRepo,
      IConfiguration config,
    IMapper mapper)
    {
      _trxPreseedJettonMasterAddressText = config["AppSettings:TrxPreseedJettonMasterAddressText"];
      _mapper = mapper;
      _logger = logger;
      _tonLocalTransactionRepo = tonLocalTransactionRepo;
      _userManager = userManager;
      _plotRepository = plotRepository;
      _nftPlotRepo = nftPlotRepo;
      _paymentRequestRepo = paymentRequestRepo;
      _tokenRequestsRepo = tokenRequestsRepo;
    }







    // [Authorize]
    [AllowAnonymous]
    [HttpGet]
    [Route("tokensell/all")]
    public async Task<ActionResult> GetTokenSellRequests()
    {
      var spec = new PaymentRequestSpecification();
      var requests = await _paymentRequestRepo.ListAsync(spec);
      var mappedRequests = _mapper.Map<IReadOnlyList<PaymentRequest>, IReadOnlyList<PaymentRequestDto>>(requests);
      return Ok(mappedRequests);
    }


    [AllowAnonymous]
    [HttpGet]
    [Route("tokens/all")]
    public async Task<ActionResult> GetTokenRequests()
    {
      var requests = _tokenRequestsRepo.GetAll().OrderByDescending(x => x.Id);
      return Ok(requests);
    }





    [AllowAnonymous]
    [HttpGet]
    [Route("tokensell/{requestId}")]
    public async Task<ActionResult> GetTokenSellRequest([FromRoute] int requestId)
    {
      Thread.Sleep(1000);
      var spec = new PaymentRequestSpecification(requestId);
      var request = await _paymentRequestRepo.GetEntityWithSpec(spec);
      var mappedRequest = _mapper.Map<PaymentRequest, PaymentRequestDto>(request);
      return Ok(mappedRequest);
    }



    // [Authorize]
    [AllowAnonymous]
    [HttpPut]
    [Route("tokensell/{requestId}/{statusId}")]
    public async Task<ActionResult> TransferJetokensWithRequest([FromRoute] int requestId, [FromRoute] int statusId)
    {
      var spec = new PaymentRequestSpecification(requestId);
      var request = await _paymentRequestRepo.GetEntityWithSpec(spec);
      request.PaymentRequestStatusId = statusId;
      await _paymentRequestRepo.UpdateAsync(request);
      var updatedRequst = await _paymentRequestRepo.GetEntityWithSpec(spec);
      var mappedRequest = _mapper.Map<PaymentRequest, PaymentRequestDto>(updatedRequst);
      return Ok(mappedRequest);
    }





  }
}