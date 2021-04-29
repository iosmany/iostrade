using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Iostrade.Core.Tareas
{
    /// <summary>
    /// Permite utilizar el Container para crear las tareas.
    /// </summary>
    public class TareaContainer : ITarea
    {
        Container container;
        Type type;

        public TareaContainer(Container container, Type type)
        {
            if (!(type.IsClass && type.GetInterfaces().Any(i => i == typeof(ITarea))))
                throw new Exception($"El tipo {type} no implementa ITarea");
            this.container = container;
            this.type = type;
        }

        public async Task Ejecutar(CancellationToken cancellationToken)
        {
            using (var scope = AsyncScopedLifestyle.BeginScope(container))
            {
                var ejecutor = container.GetInstance(type) as ITarea;
                try
                {
                    await ejecutor.Ejecutar(cancellationToken);
                }
                finally
                {
                    var reg = container.GetRegistration(type);
                    if (ejecutor is IDisposable dsp && reg.Lifestyle == Lifestyle.Transient)
                        dsp.Dispose();
                }
            }
        }
    }
}
