using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Core.Models.Identity;
using System;
using System.Linq;
using WetHands.Core.Models;

namespace Core.Identity
{
  public class AppUser : IdentityUser<int>
  {
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? PictureUrl { get; set; }
    public byte[]? PictureByte { get; set; }
    public string? PictureType { get; set; }
    public string? UserDescription { get; set; }
    public string CurrentLanguage { get; set; }
    public long? TelegramId { get; set; }
    public string? TelegramUserName { get; set; }
    public string? InstagramUserName { get; set; }
    public string? FacebookUserName { get; set; }
    public string? PnoneNumber { get; set; }
    public int? CompanyId { get; set; }
    public virtual Address? Address { get; set; }
    public virtual ICollection<UserRole> UserRoles { get; set; }
    public bool? IsAgency { get; set; }
    public DateTime? WasOnline { get; set; }
    public Currency Currency { get; set; }
    public bool IsAdmin { get; set; }
    public string JoinCode { get; set; } = GetPersonalCode();
    public string? Nickname { get; set; }
    public string? FakeAvatar { get; set; }
    public bool? IsVerified { get; set; }



    private static string GetPersonalCode()
    {
      Random rnd = new Random();
      const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
      return new string(Enumerable.Repeat(chars, 5)
        .Select(s => s[rnd.Next(s.Length)]).ToArray());
    }


    //TODO: male , female or company make avatar in https://jitter.video/


  }
}