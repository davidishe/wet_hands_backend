using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Core.Models.Identity
{
  public class Role : IdentityRole<int>
  {
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public string RoleName { get; set; }


  }
}