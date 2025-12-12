using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Checker.Infrastructure.Worker;

public class Worker : BackgroundService
{
  private readonly ILogger<Worker> _logger;
  // private readonly ITelegramService _telegramService;
  public Worker(
    ILogger<Worker> logger
    // ITelegramService telegramService
    )
  {
    _logger = logger;
    // _telegramService = telegramService;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      await ExecuteRequestGetTonPayments();
      await ExecuteSendJettons();
      await ExecuteRequestGetJetokenPayments();
      await ExecuteRequestGetTrxPayments();
      var second = 1000; // 1000 = 1sec; 
      await Task.Delay(second * 30, stoppingToken);

    }
  }


  private static async Task ExecuteRequestGetTonPayments()
  {

    Console.WriteLine("EXECUTING GET TRANSACTIONS");

    var options = new RestClientOptions();
    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

    // var client = new RestClient($"https://strapi.propertybook.space/api/companies/{account}");
    var client = new RestClient(options);
    var restRequest = new RestRequest("http://localhost:6014/api/ton/get_transactions");
    // restRequest.AddHeader("Authorization", "bearer " + _config.GetSection("AppSettings:StrapiApiToken").Value);
    restRequest.Method = Method.Get;
    var response = await client.ExecuteAsync(restRequest);

  }


  /// <summary>
  ///  высылам продуктовые жетокены через таблицу TonLocalTransactions
  /// </summary>
  /// <returns></returns>
  private static async Task ExecuteSendJettons()
  {

    Console.WriteLine("EXECUTING SEND PRODUCT JETTONS");

    var options = new RestClientOptions();
    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

    var client = new RestClient(options);
    var restRequest = new RestRequest("http://localhost:6014/api/ton/send_jettons");
    restRequest.Method = Method.Post;
    var response = await client.ExecuteAsync(restRequest);

  }


  private static async Task ExecuteRequestGetJetokenPayments()
  {

    Console.WriteLine("EXECUTING GET JETOKEN PAYMENTS");
    var options = new RestClientOptions();
    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    var client = new RestClient(options);
    var restRequest = new RestRequest("http://localhost:6014/api/ton/get_jetoken_payments");
    restRequest.Method = Method.Get;
    var response = await client.ExecuteAsync(restRequest);

  }



  private static async Task ExecuteRequestGetTrxPayments()
  {
    Console.WriteLine("EXECUTING GET TRX PAYMENTS");
    var options = new RestClientOptions();
    options.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
    var client = new RestClient(options);
    var restRequest = new RestRequest("http://localhost:6014/api/trx/get_trx_payments");
    restRequest.Method = Method.Get;
    var response = await client.ExecuteAsync(restRequest);
  }






}

