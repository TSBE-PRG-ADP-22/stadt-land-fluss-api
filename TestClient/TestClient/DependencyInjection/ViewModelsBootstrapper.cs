using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;
using Splat;
using TestClient.Services;
using TestClient.ViewModels;

namespace TestClient.DependencyInjection
{
    public static class ViewModelsBootstrapper
    {
        private const string APIURL = "http://localhost:8080";

        public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterServices(services, resolver);
            RegisterCommonViewModels(services, resolver);
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton(() => GetRefitService<ILobbyService>(new Uri(APIURL)));
            services.RegisterLazySingleton<ILobbyHubService>(() => new LobbyHubService());

        }

        private static void RegisterCommonViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IMainWindowViewModel>(() => new MainWindowViewModel(
                resolver.GetRequiredService<ILobbyService>(),
                resolver.GetRequiredService<ILobbyHubService>()
            ));
        }

        private static T GetRefitService<T>(Uri uri)
        {
            var client = new HttpClient()
            {
                BaseAddress = uri
            };

            return RestService.For<T>(
            client: client,
            settings: new RefitSettings
            {
                ContentSerializer = GetContentSerializer()
            });
        }

        private static IHttpContentSerializer GetContentSerializer()
        {
            return new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter() }
                });
        }

    }
}
