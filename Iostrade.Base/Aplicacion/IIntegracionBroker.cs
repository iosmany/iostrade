using Iostrade.Base.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Base
{
    public interface IIntegracionBroker
    {
        Task Configura();
        IConexionBrokerAPI AbreConexion(ICuenta cuenta);
    }
}
