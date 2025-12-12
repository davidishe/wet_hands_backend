using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WetHands.Core.Models;
using WetHands.Infrastructure.Database;
using WetHands.Infrastructure.Specifications;

namespace WetHands.Infrastructure.Documents
{
  public class OrderDocumentService : IOrderDocumentService
  {
    private readonly IGenericRepository<Order> _orderSpecRepository;
    private readonly ILogger<OrderDocumentService> _logger;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly string _templatePath;

    public OrderDocumentService(
      IGenericRepository<Order> orderSpecRepository,
      IHostEnvironment hostEnvironment,
      ILogger<OrderDocumentService> logger)
    {
      _orderSpecRepository = orderSpecRepository;
      _hostEnvironment = hostEnvironment;
      _logger = logger;
      _templatePath = ResolveTemplatePath();
    }

    public async Task<byte[]> GenerateOrderSummaryAsync(int orderId, CancellationToken cancellationToken = default)
    {
      var spec = new OrderSpecification(orderId);
      var order = await _orderSpecRepository.GetEntityWithSpec(spec);

      if (order is null)
      {
        throw new InvalidOperationException($"Order with id {orderId} not found.");
      }

      if (!File.Exists(_templatePath))
      {
        _logger.LogWarning("Docx template not found at '{TemplatePath}'. Falling back to generated layout.", _templatePath);
        return GenerateDocumentFromScratch(order);
      }

      using var templateStream = File.OpenRead(_templatePath);
      using var ms = new MemoryStream();
      templateStream.CopyTo(ms);

      using (var wordDoc = WordprocessingDocument.Open(ms, true))
      {
        var mainPart = wordDoc.MainDocumentPart ?? throw new InvalidOperationException("Template does not contain a main document part.");
        ReplaceText(mainPart, "{{OrderNumber}}", order.Id.ToString());
        ReplaceText(mainPart, "{{CustomerName}}", order.Company?.Name ?? "—");
        ReplaceText(mainPart, "{{OrderDate}}", order.CreatedAt.ToString("dd.MM.yyyy HH:mm"));
        ReplaceText(mainPart, "{{ContactName}}", order.ContactNameText ?? "—");
        ReplaceText(mainPart, "{{ContactPhone}}", order.ContactPhoneText ?? "—");
        ReplaceText(mainPart, "{{TotalWeight}}", (order.WeightKg ?? 0m).ToString("N2"));
        ReplaceText(mainPart, "{{GeneratedAt}}", DateTime.UtcNow.ToLocalTime().ToString("dd.MM.yyyy HH:mm"));

        PopulateItemsTable(mainPart, order.Items ?? Array.Empty<OrderItem>());

        mainPart.Document.Save();
      }

      return ms.ToArray();
    }

    public Task<byte[]> GetTemplateAsync(CancellationToken cancellationToken = default)
    {
      using var stream = new MemoryStream();
      using (var wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
      {
        var mainPart = wordDoc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body ?? mainPart.Document.AppendChild(new Body());

        body.AppendChild(CreateParagraph("Отчёт по заказу {{OrderNumber}}", bold: true, fontSize: "32"));
        body.AppendChild(CreateParagraph(string.Empty));

        body.AppendChild(CreateParagraph("Заказчик: {{CustomerName}}"));
        body.AppendChild(CreateParagraph("Дата заказа: {{OrderDate}}"));
        body.AppendChild(CreateParagraph("Контактное лицо: {{ContactName}}"));
        body.AppendChild(CreateParagraph("Контактный телефон: {{ContactPhone}}"));
        body.AppendChild(CreateParagraph(string.Empty));

        body.AppendChild(CreateParagraph("Состав заказа", bold: true));
        body.AppendChild(CreateParagraph(string.Empty));
        body.AppendChild(BuildTemplateTable());
        body.AppendChild(CreateParagraph(string.Empty));

        body.AppendChild(CreateParagraph("Итого вес, кг: {{TotalWeight}}"));
        body.AppendChild(CreateParagraph(string.Empty));
        body.AppendChild(CreateParagraph("Документ сформирован: {{GeneratedAt}}"));

        mainPart.Document.Save();
      }

      return Task.FromResult(stream.ToArray());
    }

    private string ResolveTemplatePath()
    {
      var root = _hostEnvironment.ContentRootPath;
      var path = Path.Combine(root, "..", "WetHands.Infrastructure", "Documents", "Templates", "order-summary-template.docx");
      return Path.GetFullPath(path);
    }

    private static void ReplaceText(MainDocumentPart mainPart, string placeholder, string value)
    {
      foreach (var text in mainPart.Document.Descendants<Text>())
      {
        if (text.Text.Contains(placeholder))
        {
          text.Text = text.Text.Replace(placeholder, value);
        }
      }
    }

    private static void PopulateItemsTable(MainDocumentPart mainPart, ICollection<OrderItem> items)
    {
      var table = mainPart.Document.Body?.Descendants<Table>()
        .FirstOrDefault(t => t.InnerText.Contains("{{ItemIndex}}"));

      if (table is null) return;

      var templateRow = table.Descendants<TableRow>()
        .FirstOrDefault(r => r.InnerText.Contains("{{ItemIndex}}"));

      if (templateRow is null) return;

      var orderedItems = items?.OrderBy(i => i.Id).ToList() ?? new List<OrderItem>();
      if (orderedItems.Count == 0)
      {
        var emptyRow = CloneRow(templateRow);
        ReplaceRowPlaceholders(emptyRow, new Dictionary<string, string>
        {
          ["{{ItemIndex}}"] = "1",
          ["{{ItemName}}"] = "—",
          ["{{QtyDirty}}"] = "0",
          ["{{QtyClean}}"] = "0",
          ["{{ItemNote}}"] = "Позиции отсутствуют"
        });
        table.AppendChild(emptyRow);
      }
      else
      {
        var index = 1;
        foreach (var item in orderedItems)
        {
          var row = CloneRow(templateRow);
          ReplaceRowPlaceholders(row, new Dictionary<string, string>
          {
            ["{{ItemIndex}}"] = index++.ToString(),
            ["{{ItemName}}"] = item.Name ?? "—",
            ["{{QtyDirty}}"] = (item.QtyDirty ?? 0).ToString(),
            ["{{QtyClean}}"] = (item.QtyClean ?? 0).ToString(),
            ["{{ItemNote}}"] = item.CommentText ?? string.Empty
          });
          table.AppendChild(row);
        }
      }

      table.RemoveChild(templateRow);
    }

    private static TableRow CloneRow(TableRow templateRow)
    {
      return (TableRow)templateRow.CloneNode(true);
    }

    private static void ReplaceRowPlaceholders(TableRow row, IDictionary<string, string> values)
    {
      foreach (var text in row.Descendants<Text>())
      {
        foreach (var kvp in values)
        {
          if (text.Text.Contains(kvp.Key))
          {
            text.Text = text.Text.Replace(kvp.Key, kvp.Value);
          }
        }
      }
    }

    private static Paragraph CreateParagraph(string text, bool bold = false, string? fontSize = null)
    {
      var runProperties = new RunProperties();
      if (bold) runProperties.AppendChild(new Bold());
      if (!string.IsNullOrEmpty(fontSize))
      {
        runProperties.AppendChild(new FontSize { Val = fontSize });
      }

      var run = new Run();
      if (runProperties.ChildElements.Count > 0) run.Append(runProperties);
      run.AppendChild(new Text(text) { Space = SpaceProcessingModeValues.Preserve });

      return new Paragraph(run);
    }

    private static Table BuildTemplateTable()
    {
      var table = new Table();
      var tableProperties = new TableProperties(
        new TableBorders(
          new TopBorder { Val = BorderValues.Single, Size = 6 },
          new BottomBorder { Val = BorderValues.Single, Size = 6 },
          new LeftBorder { Val = BorderValues.Single, Size = 6 },
          new RightBorder { Val = BorderValues.Single, Size = 6 },
          new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
          new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
        ));
      table.AppendChild(tableProperties);

      table.AppendChild(new TableRow(
        CreateHeaderCell("№"),
        CreateHeaderCell("Наименование"),
        CreateHeaderCell("Грязные, шт"),
        CreateHeaderCell("Чистые, шт"),
        CreateHeaderCell("Примечание")
      ));

      var templateRow = new TableRow(
        CreateCell("{{ItemIndex}}"),
        CreateCell("{{ItemName}}"),
        CreateCell("{{QtyDirty}}"),
        CreateCell("{{QtyClean}}"),
        CreateCell("{{ItemNote}}")
      );
      table.AppendChild(templateRow);

      return table;
    }

    private static TableCell CreateHeaderCell(string text)
    {
      return new TableCell(CreateParagraph(text, bold: true));
    }

    private static TableCell CreateCell(string text)
    {
      return new TableCell(CreateParagraph(text));
    }

    private byte[] GenerateDocumentFromScratch(Order order)
    {
      using var stream = new MemoryStream();
      using (var wordDoc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
      {
        var mainPart = wordDoc.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var body = mainPart.Document.Body ?? mainPart.Document.AppendChild(new Body());

        body.AppendChild(CreateParagraph($"Отчёт по заказу №{order.Id}", bold: true, fontSize: "32"));
        body.AppendChild(CreateParagraph(string.Empty));

        body.AppendChild(CreateParagraph($"Заказчик: {order.Company?.Name ?? "—"}"));
        body.AppendChild(CreateParagraph($"Дата заказа: {order.CreatedAt:dd.MM.yyyy HH:mm}"));
        body.AppendChild(CreateParagraph($"Контактное лицо: {order.ContactNameText ?? "—"}"));
        body.AppendChild(CreateParagraph($"Контактный телефон: {order.ContactPhoneText ?? "—"}"));
        body.AppendChild(CreateParagraph(string.Empty));

        body.AppendChild(CreateParagraph("Состав заказа", bold: true));
        body.AppendChild(CreateParagraph(string.Empty));
        body.AppendChild(BuildItemsTable(order));
        body.AppendChild(CreateParagraph(string.Empty));

        body.AppendChild(CreateParagraph($"Итого вес, кг: {(order.WeightKg ?? 0m):N2}"));
        body.AppendChild(CreateParagraph(string.Empty));
        body.AppendChild(CreateParagraph($"Документ сформирован: {DateTime.UtcNow.ToLocalTime():dd.MM.yyyy HH:mm}"));

        mainPart.Document.Save();
      }

      return stream.ToArray();
    }

    private static Table BuildItemsTable(Order order)
    {
      var table = new Table();
      var tableProperties = new TableProperties(
        new TableBorders(
          new TopBorder { Val = BorderValues.Single, Size = 6 },
          new BottomBorder { Val = BorderValues.Single, Size = 6 },
          new LeftBorder { Val = BorderValues.Single, Size = 6 },
          new RightBorder { Val = BorderValues.Single, Size = 6 },
          new InsideHorizontalBorder { Val = BorderValues.Single, Size = 6 },
          new InsideVerticalBorder { Val = BorderValues.Single, Size = 6 }
        ));
      table.AppendChild(tableProperties);

      table.AppendChild(new TableRow(
        CreateHeaderCell("№"),
        CreateHeaderCell("Наименование"),
        CreateHeaderCell("Грязные, шт"),
        CreateHeaderCell("Чистые, шт"),
        CreateHeaderCell("Примечание")));

      var orderedItems = order.Items?.OrderBy(i => i.Id).ToList() ?? new List<OrderItem>();
      if (orderedItems.Count == 0)
      {
        table.AppendChild(new TableRow(
          CreateCell("1"),
          CreateCell("—"),
          CreateCell("0"),
          CreateCell("0"),
          CreateCell("Позиции отсутствуют")));
      }
      else
      {
        var index = 1;
        foreach (var item in orderedItems)
        {
          table.AppendChild(new TableRow(
            CreateCell(index++.ToString()),
            CreateCell(item.Name ?? "—"),
            CreateCell((item.QtyDirty ?? 0).ToString()),
            CreateCell((item.QtyClean ?? 0).ToString()),
            CreateCell(item.CommentText ?? string.Empty)));
        }
      }

      return table;
    }
  }
}
