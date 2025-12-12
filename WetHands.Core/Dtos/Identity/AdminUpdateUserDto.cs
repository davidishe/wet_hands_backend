namespace Core.Dtos.Identity
{
  public class AdminUpdateUserDto
  {
    public int UserId { get; set; }
    public int? CompanyId { get; set; }
    public bool IsAdmin { get; set; }
  }
}
