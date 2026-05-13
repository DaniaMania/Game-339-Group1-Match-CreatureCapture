using System;
using Game339.Shared;
using Game339.Shared.DependencyInjection;
using Game339.Shared.DependencyInjection.Implementation;
using Game339.Shared.Diagnostics;
using Game339.Shared.Services;

namespace Game.Runtime
{
    public static class ServiceResolver
    {
        public static T Resolve<T>() => Container.Value.Resolve<T>();

        private static readonly Lazy<IMiniContainer> Container = new Lazy<IMiniContainer>(() =>
        {
            MiniContainer container = new MiniContainer();

            UnityGameLogger logger = new UnityGameLogger();
            container.RegisterSingletonInstance<IGameLog>(logger);
            
            DamageService damageService = new DamageService();
            container.RegisterSingletonInstance<IDamageService>(damageService);
            
            AttackService attackService = new AttackService();
            container.RegisterSingletonInstance(attackService);

            TurnEngine turnEngine = new TurnEngine();
            container.RegisterSingletonInstance(turnEngine);
            
            var audioService = new AudioService();
            container.RegisterSingletonInstance(audioService);

            return container;
        });
    }
}
