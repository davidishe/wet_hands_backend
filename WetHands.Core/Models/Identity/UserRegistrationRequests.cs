using System.ComponentModel.DataAnnotations;
namespace Core.Models.Identity
{
  public class UserRegistrationRequests
  {
    public class UserRegistrationRequest
    {
      [EmailAddress]
      public string Email { get; set; }
      public string Password { get; set; }
      public bool? IsAgency { get; set; }
    }
  }
}