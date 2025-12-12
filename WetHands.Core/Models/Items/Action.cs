using System;
using Core.Models;

namespace WetHands.Core.Models.Items
{
  public class Action : BaseEntity
  {
    public string ActionName { get; set; }
    public string ActionDescription { get; set; }
    public DateTime EnrolledDate { get; set; }
    public int? ProposalId { get; set; }
  }
}