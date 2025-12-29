using System;
using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace WetHands.Core.Models
{
  public class MassagePlaceImage : BaseEntity
  {
    public int MassagePlaceId { get; set; }

    public bool IsMain { get; set; }

    public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;

    [MaxLength(256)]
    public string? FileName { get; set; }

    [MaxLength(128)]
    public string? FileType { get; set; }

    public byte[]? DocByte { get; set; }

    public int? Size { get; set; }
  }
}


