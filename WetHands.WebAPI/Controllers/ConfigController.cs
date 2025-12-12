using AutoMapper;
using Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WetHands.WebAPI.Controllers
{


  [Authorize]
  public class ConfigController : BaseApiController
  {


    private readonly IMapper _mapper;
    private readonly ILogger<ConfigController> _logger;
    private readonly string _mainHolderAccount;
    private readonly bool _isProductiveMode;

    public ConfigController(
      UserManager<AppUser> userManager,
      ILogger<ConfigController> logger,
      IConfiguration config,
      IMapper mapper)
    {
      _mainHolderAccount = config["AppSettings:MainHolderAccount"];
      _isProductiveMode = config.GetValue<bool>("AppSettings:IsProductiveMode");
      _mapper = mapper;
      _logger = logger;
    }



    public enum CHAIN
    {
      MAINNET = -239,
      TESTNET = -3
    }


    [AllowAnonymous]
    [HttpGet]
    [Route("mode")]
    public ActionResult GetTestmodeStatus()
    {

      if (_isProductiveMode)
      {
        return Ok(new
        {
          isProd = true,
          chain = CHAIN.MAINNET,
          nonBouncileTerraPreseedWalletAddress = "UQAU0WM_b0FCWepmFuvcNAgXb30GBgGVBl_poM0AYZGJxu-F",
          nonBouncileTerraPreseedWalletAddressUrl = "https://tonviewer.com/UQAU0WM_b0FCWepmFuvcNAgXb30GBgGVBl_poM0AYZGJxu-F",
          productTokenByeChain = CHAIN.TESTNET
        });
      }

      else
      {
        return Ok(new
        {
          isProd = false,
          chain = CHAIN.TESTNET,
          nonBouncileTerraPreseedWalletAddress = "0QAlIwXHc0p_FVzKs3NP2RrED3sGj5WC_0r-xxuTEU_Do-qk",
          nonBouncileTerraPreseedWalletAddressUrl = "https://testnet.tonviewer.com/0QAlIwXHc0p_FVzKs3NP2RrED3sGj5WC_0r-xxuTEU_Do-qk",
          productTokenByeChain = CHAIN.TESTNET
        });
      }





    }










  }
}