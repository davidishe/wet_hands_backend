using System.Threading.Tasks;
using AutoMapper;
using Core.Identity;
using WetHands.Infrastructure.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models.Items;
using WetHands.Core.Models;
using System.Threading;
using System.IO;

namespace WetHands.WebAPI.Controllers
{


    [Authorize]
    public class CompanyController : BaseApiController
    {

        private readonly IDbRepository<Company> _companyRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<CompanyController> _logger;
        private readonly IDbRepository<Picture> _pictureRepo;


        public CompanyController(
          UserManager<AppUser> userManager,
          IDbRepository<Company> companyRepo,
          ILogger<CompanyController> logger,
          IDbRepository<Picture> pictureRepo,
          IMapper mapper)
        {
            _logger = logger;
            _companyRepo = companyRepo;
            _userManager = userManager;
            _pictureRepo = pictureRepo;
        }



        [AllowAnonymous]
        [HttpGet]
        [Route("all")]
        public ActionResult GetAll()
        {
            var result = _companyRepo.GetAll();
            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create()
        {
            var company = new Company();
            var createdCompany = await _companyRepo.AddAsync(company);
            return Ok(createdCompany);
        }


        [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetById([FromRoute] int id)
        {
            var company = await _companyRepo.GetByIdAsync(id);
            if (company == null) return NotFound();
            return Ok(company);
        }





        [Authorize]
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult> Update([FromBody] CompanyDto dto)
        {

            var plotForUpdate = await _companyRepo.GetByIdAsync(dto.Id);

            plotForUpdate.Name = dto.Name;
            plotForUpdate.Description = dto.Description;
            plotForUpdate.Phone = dto.Phone;
            plotForUpdate.Website = dto.Website;
            plotForUpdate.Address = dto.Address;
            plotForUpdate.Mail = dto.Mail;
            plotForUpdate.FacebookUserName = dto.FacebookUserName;
            plotForUpdate.TelegramCompanyName = dto.TelegramCompanyName;
            plotForUpdate.LaundryPricePerKg = dto.LaundryPricePerKg;
            plotForUpdate.DryCleaningPricePerKg = dto.DryCleaningPricePerKg;
            plotForUpdate.RestaurantPricePerKg = dto.RestaurantPricePerKg;

            await _companyRepo.UpdateAsync(plotForUpdate);
            return Ok(200);
        }


        [Authorize]
        [HttpPost]
        [Route("photo/{itemId}")]
        public async Task<ActionResult> UpdatePhoto([FromRoute] int itemId)
        {

            var itemForUpdate = await _companyRepo.GetByIdAsync(itemId);
            Thread.Sleep(1200);
            var files = Request.Form.Files;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        var docByte = memoryStream.ToArray();
                        itemForUpdate.CompanyAvatar = docByte;
                        await _companyRepo.UpdateAsync(itemForUpdate);
                    }
                }
            }

            return Ok(itemForUpdate);
        }






    }
}
