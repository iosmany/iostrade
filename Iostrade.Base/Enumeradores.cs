using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade
{
    public enum Clasificacion { DayTrading, SwingTrading, TrendTrading }
    public enum TipoOperacion { BuyMarket, SellMarket, BuyLimit, SellLimit, BuyStop, SellStop }
    public enum EstadoOperacion { Abierta, Cerrada, Cancelada }
}
