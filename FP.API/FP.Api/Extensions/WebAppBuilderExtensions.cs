using FP.Application.Interfaces;
using FP.Domain.Base;
using FP.Infrastructure.Services;
using System.Reflection;

namespace FP.Api.Extensions
{
    public static class WebAppBuilderExtensions
    {
        public static void RegisterRepositories(this WebApplicationBuilder builder)
        {

            var domainAssembly = Assembly.Load("FP.Domain");
            var baseEntityType = typeof(BaseEntity);

            foreach (var type in domainAssembly.GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseEntityType))
                {
                    var repositoryInterface = typeof(IRepository<>).MakeGenericType(type);
                    var repositoryImplementation = typeof(Repository<>).MakeGenericType(type);

                    builder.Services.AddTransient(repositoryInterface, repositoryImplementation);
                }
            }
        }

        public static void RegisterServices(this WebApplicationBuilder builder)
        {
            var applicationAssembly = Assembly.Load("FP.Application");
            var baseServiceType = typeof(IBaseService);

            var serviceTypes = applicationAssembly.GetTypes()
                .Where(t => baseServiceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

            foreach (var serviceType in serviceTypes)
            {
                var serviceInterface = serviceType.GetInterfaces()
                    .FirstOrDefault(i => i != baseServiceType && baseServiceType.IsAssignableFrom(i));

                if (serviceInterface != null)
                {
                    builder.Services.AddTransient(serviceInterface, serviceType);
                }
            }
        }
    }
}
