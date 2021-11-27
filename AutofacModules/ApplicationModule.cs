using System.Reflection;
using Autofac;
using GraphQL.NewtonsoftJson;
using Microsoft.AspNetCore.Http;
using MovieReviews.Database;
using MovieReviews.GraphQL;

namespace MovieReviews.AutofacModules;

public class ApplicationModule
    : Autofac.Module
{

    protected override void Load(ContainerBuilder builder)
    {

        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        builder.RegisterType<MovieRepository>().As<IMovieRepository>().InstancePerLifetimeScope();

        builder.RegisterType<DocumentWriter>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<QueryObject>().AsSelf().SingleInstance();
        builder.RegisterType<MovieReviewSchema>().AsSelf().SingleInstance();

    }
}
