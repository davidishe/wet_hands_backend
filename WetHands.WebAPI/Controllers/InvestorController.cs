using AutoMapper;
using Core.Identity;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebAPI.Controllers;
using WetHands.Core.TonModels;
using Microsoft.Extensions.Logging;
using WetHands.Core;
using Microsoft.Extensions.Configuration;

namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class InvestorController : BaseApiController
  {

    private readonly UserManager<AppUser> _userManager;
    private readonly IDbRepository<TonLocalTransaction> _tonLocalTransactionRepo;
    private readonly IMapper _mapper;
    private readonly ILogger<InvestorController> _logger;
    private readonly IDbRepository<Order> _plotRepository;
    private readonly IDbRepository<NftPlot> _nftPlotRepo;
    private readonly IDbRepository<NftSellRequest> _jettonSellRequestRepo;

    // _config.GetSection("AppSettings:MainHolderAccount").Value!
    private readonly string _mainHolderAccount;
    // private readonly IConfiguration _config;



    public InvestorController(
      UserManager<AppUser> userManager,
      ILogger<InvestorController> logger,
      IDbRepository<TonLocalTransaction> tonLocalTransactionRepo,
      IDbRepository<Order> plotRepository,
      IDbRepository<NftPlot> nftPlotRepo,
      IDbRepository<NftSellRequest> jettonSellRequestRepo,
      IConfiguration config,
      IMapper mapper)
    {
      _mainHolderAccount = config["AppSettings:MainHolderAccount"];
      _mapper = mapper;
      _logger = logger;
      _tonLocalTransactionRepo = tonLocalTransactionRepo;
      _userManager = userManager;
      _plotRepository = plotRepository;
      _nftPlotRepo = nftPlotRepo;
      _jettonSellRequestRepo = jettonSellRequestRepo;
    }






  }
}