using Iostrade.Base;
using Iostrade.Base.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Estrategias
{
    abstract class EstrategiaSimpleBase
    {
        public EstrategiaSimpleBase()
        {
        }

        public virtual Task Configura(ICuenta cuenta, IInstrumento instrumento, IConexionBrokerAPI conexionBroker)
        {

            throw new NotImplementedException();
        }

    }
}
