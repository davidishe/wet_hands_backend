using Core.Models;

namespace WetHands.Core.Models.Items
{
  public class ProposalDescriptionTranaltaion : BaseEntity
  {
    public string? English { get; set; }
    public string? Russian { get; set; }
    public string? Georgian { get; set; }
    public string InitialLanguage { get; set; }
  }
}

// en
// ru
// ka