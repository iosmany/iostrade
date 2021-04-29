using Iostrade.Base.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Base
{
    public interface ILeeInstrumentos
    {
        
    }

    public interface IConexionBrokerAPI : IDisposable, IAsyncDisposable
    {
        Task<ICuenta> DatosCuentaAsync();
        IAsyncEnumerable<ICotizacion> CotizacionesAsync(DateTimeOffset desde);
        IAsyncEnumerable<IOperacion> OperacionesAbiertasAsync(ICuenta cuenta);
        IAsyncEnumerable<IOperacion> OperacionesHistoricoAbiertasAsync(ICuenta cuenta);
        IAsyncEnumerable<IInstrumento> Get(ILeeInstrumentos datosLee);

        Task AbrirOpAsync(IOperacion op, ICuenta cuenta);
        Task CerrarOpAsync(IOperacion op, ICuenta cuenta);
    }
}
