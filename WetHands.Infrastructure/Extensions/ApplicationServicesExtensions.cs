using System.Linq;
using WetHands.Core.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using WetHands.Infrastructure.Database.UnitOfWork;
using WetHands.Infrastructure.Database;

namespace WetHands.Infrastructure
{
  public static class ApplicationServicesExtensions
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
      services.AddScoped<IUnitOfWork, UnitOfWork>();


      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.InvalidModelStateResponseFactory = actionContext =>
        {
          var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage).ToArray();

          var errorResponse = new ApiValidationErrorResponse()
          {
            Errors = errors
          };

          return new BadRequestObjectResult(errorResponse);

        };

      });

      return services;

    }
  }
}