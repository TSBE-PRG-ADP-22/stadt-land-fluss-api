﻿using Splat;

namespace TestClient.DependencyInjection
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            ViewModelsBootstrapper.RegisterViewModels(services, resolver);
        }
    }
}
