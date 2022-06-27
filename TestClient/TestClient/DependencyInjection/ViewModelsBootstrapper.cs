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
        public static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, ClientConfiguration clientConfig)
        {
            RegisterServices(services, resolver, clientConfig);
            RegisterCommonViewModels(services, resolver);
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver, ClientConfiguration clientConfig)
        {
            services.RegisterLazySingleton(() => GetRefitService<ILobbyService>(new Uri(clientConfig.BaseUrl)));
            services.RegisterLazySingleton<ILobbyHubService>(() => new LobbyHubService(clientConfig));
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
                    //Converters = { new JsonStringEnumConverter() }
                });
        }

    }
}
