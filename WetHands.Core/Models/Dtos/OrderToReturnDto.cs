using System;
using Core.Dtos;

namespace Core.Models
{
  public class OrderToReturnDto : BaseEntity
  {
    public int? ProposalId { get; set; }
    public DateTime EnrolledDate { get; set; } = DateTime.Now;
    public double ShareValue { get; set; }
    public UserToReturnDto? Investor { get; set; }
    public string Status { get; set; }
    public int StatusId { get; set; }
    public int PartnerId { get; set; }


  }
}