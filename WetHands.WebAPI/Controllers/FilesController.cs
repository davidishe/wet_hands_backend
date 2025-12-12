using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Core.Identity;

namespace WebAPI.Controllers
{



  [AllowAnonymous]
  public class FilesController : BaseApiController
  {


    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;


    public FilesController(
      UserManager<AppUser> userManager,
      IMapper mapper)
    {
      _mapper = mapper;
      _userManager = userManager;
    }




    private Task<string> SaveFileToServer(IFormFile file, string path, string subPath)
    {
      var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", path, subPath, fileName);

      using (var stream = new FileStream(fullPath, FileMode.Create))
      {
        file.CopyTo(stream);
      }

      return Task.FromResult(fileName);

    }

    private Task<bool> DeleteFileFromServer(string fileName, string path, string subPath)
    {

      if (fileName is null)
        return Task.FromResult(true);

      var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", path, subPath, fileName);
      if (System.IO.File.Exists(fullPath))
      {
        System.IO.File.Delete(fullPath);
      }

      return Task.FromResult(true);

    }



  }
}