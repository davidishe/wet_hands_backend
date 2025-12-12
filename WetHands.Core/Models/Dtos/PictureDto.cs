using System;
using Core.Models;

namespace WetHands.Core.Models.Items
{
  public class PictureDto : BaseEntity
  {
    public bool IsMain { get; set; }
    public DateTime EnrolledDate { get; set; }

    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public byte[]? DocByte { get; set; }
    public int? Size { get; set; }

  }
}