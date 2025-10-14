using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using UAE_Pass_Poc.DBContext;
using UAE_Pass_Poc.Entities;
using UAE_Pass_Poc.MappingProfile;
using UAE_Pass_Poc.Repositories;
using UAE_Pass_Poc.Services;
using UAE_Pass_Poc.Services.Interfaces;

namespace UAE_Pass_Poc.Extensions;

public static class Extension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServerDb(configuration);
        //Add Services in DI container
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<Mappers>();
        });

        //Register Services
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<ICadesVerificationService, CadesVerificationService>();
        services.AddScoped<IPresentationProcessingService, PresentationProcessingService>();
        services.AddScoped<IDidResolutionService, DidResolutionService>();
        services.AddScoped<HttpClient>();

        //Add Repositories in DI container
        services.AddScoped<IRequestPresentationRepository, RequestPresentationRepository>();
    }


    public static void AddSqlServerDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UaePassDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DbConnection")!;
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
            });
        });
    }
}