using Iostrade.Base;
using Iostrade.Base.Entidades;
using Iostrade.Core.Indicadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Estrategias.Swingtrading
{
    /// <summary>
    /// Estrategia que se centra en ejecutarse y abrir operación 
    /// al abrir la bolsa y cerrar operacion antes de cerrar la bolsa
    /// basándose en los indicadores MACD y Volumen
    /// OJO: no sirve con crypto
    /// </summary>
    sealed class DayByDay : EstrategiaSimpleBase
    {
        readonly MACD mACD;
        readonly Volumen volumen;

        public DayByDay(MACD mACD, Volumen volumen)
        {
            this.mACD = mACD;
            this.volumen = volumen;
        }

        public override Task Configura(ICuenta cuenta, IInstrumento instrumento, IConexionBrokerAPI conexionBroker)
        {
            //configurar las ejecuciones
            return Task.CompletedTask;
        }

        public Task Aplica()
        {
            //obtener previsión ajustada de MACD
            mACD.Calcula();
            volumen.Calcula();

            return Task.CompletedTask;
        }
    }
}
