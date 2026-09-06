using CommonLibrary.Core.Domain.Dto;
using CommonLibrary.Core.Domain.Interfaces;
using CommonLibrary.Core.Domain.RepositoryInterfaces;
using CommonLibrary.Core.Services.Interfaces;
using CommonLibrary.Infrastructure.JwtManagers;
using CommonLibrary.Infrastructure.Proxy;
using CommonLibrary.Infrastructure.Utils;
using CommonLibrary.Infrastructure.Utils.Caching;
using CommonLibrary.Infrastructure.Utils.MiddleWares;
using CommonLibrary.Infrastructure.Validators;
using LoanMicroService.Infrastructure.Persistence.Dappers;
using LoanMonitoringMicroService.Core.Domain.RepositoryInterfaces;
using LoanMonitoringMicroService.Core.DomainServices.Cartables;
using LoanMonitoringMicroService.Core.DomainServices.Interfaces;
using LoanMonitoringMicroService.Core.DomainServices.Loans;
using LoanMonitoringMicroService.Core.DomainServices.LoansActions;
using LoanMonitoringMicroService.Core.DomainServices.PlanNo;
using LoanMonitoringMicroService.Core.DomainServices.Wage;
using LoanMonitoringMicroService.Infrastructure.Persistence.EntityFramework;
using Microsoft.OpenApi.Models;
using System.Net.Mime;

namespace LoanMonitoringMicroService.Infrastructure.DependencyInjection
{
    public static partial class LoanMonitoringServicesInjection
    {
        public static IServiceCollection AddLoanMonitoringServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ISessionCacheService, SessionCacheService>();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<JwtAuthorize, JwtAuthorize>();
            services.AddScoped<IJwtManager, JwtManager>();
            services.AddScoped<IDapperHandler, LoanMonitoringDapperHandler>();
            services.AddSwaggerGen(o => o.CustomSchemaIds(c => c.ToString()));
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Loan Monitoring (Survey)",
                    Description = ""
                });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "-",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new ValidationFailedResult(context.ModelState);

                    // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);

                    return result;
                };
            });

            services.AddScoped<LoginUserDTO>(provider =>
            {
                return LoginUserMiddleware.SetLoginUser(provider.GetService<IHttpContextAccessor>().HttpContext);
            });
            services.AddScoped<ILoanMonitoringDbContext, LoanMonitoringDbContext>();
            services.AddScoped<ISurveyWageRepositoryRead, SurveyWageRepositoryRead>();
            services.AddScoped<ISurveyRepositoryRead, SurveyRepositoryRead>();
            services.AddScoped<ISurveyRepositoryWrite, SurveyRepositoryWrite>();
            services.AddScoped<ICartableRepositryWrite, CartableRepositryWrite>();
            services.AddScoped<ICartableRepositryRead, CartableRepositryRead>();
            services.AddScoped<ISurveyListRepositoryRead, SurveyListRepository>();
            services.AddScoped<IDateConvertor, DateConvertor>();
            services.AddScoped<IPlanNoRepositoryRead, PlanNoRepositoryRead>();
            services.AddScoped<IPlanNoRepositoryWrite, PlanNoRepositoryWrite>();
            services.AddScoped<IAccessManager, AccessManager>();
            services.AddScoped<IUserManagementProxy, UserManagementProxy>();
            services.AddScoped<IPwaRepositoryRead, PwaRepositoryRead>();
            services.AddScoped<IPwaRepositoryWrite, PwaRepositoryWrite>();

            return services;
        }

    }
}
