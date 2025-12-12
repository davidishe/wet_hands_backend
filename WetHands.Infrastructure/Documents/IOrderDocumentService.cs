using System.Threading;
using System.Threading.Tasks;

namespace WetHands.Infrastructure.Documents
{
  public interface IOrderDocumentService
  {
    Task<byte[]> GenerateOrderSummaryAsync(int orderId, CancellationToken cancellationToken = default);
    Task<byte[]> GetTemplateAsync(CancellationToken cancellationToken = default);
  }
}
