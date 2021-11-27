using GraphQL.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MovieReviews.Database;
using MovieReviews.GraphQL;
using Microsoft.Extensions.Configuration;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using MovieReviews.AutofacModules;
using GraphQL.Server.Ui.Altair;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEntityFrameworkInMemoryDatabase()
               .AddDbContext<MovieContext>(context => { context.UseInMemoryDatabase("MovieDb"); });
builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services
    .AddGraphQL(
        (options, provider) =>
        {
            // Load GraphQL Server configurations
            var graphQLOptions = new GraphQLOptions();
            builder.Configuration.GetSection("GraphQL").Bind(graphQLOptions);
            options.ComplexityConfiguration = graphQLOptions.ComplexityConfiguration;
            options.EnableMetrics = graphQLOptions.EnableMetrics;
            // Log errors
            var logger = provider.GetRequiredService<ILogger<Program>>();
            options.UnhandledExceptionDelegate = ctx =>
                logger.LogError("{Error} occurred", ctx.OriginalException.Message);
        })
    // Adds all graph types in the current assembly with a singleton lifetime.
    .AddGraphTypes()
    // Add GraphQL data loader to reduce the number of calls to our repository. https://graphql-dotnet.github.io/docs/guides/dataloader/
    .AddDataLoader()
    .AddSystemTextJson();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => {
    containerBuilder.RegisterModule(new ApplicationModule()); });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseGraphQL<MovieReviewSchema>();
// Enables Altair UI at path /
app.UseGraphQLAltair(new AltairOptions(),"/");

app.UseHttpsRedirection();



app.Run();