using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Indicadores
{
    class SMA
    {
        public SMA()
        {
        }

        public void Calcula()
        {

        }
    }
}

/*
 Calcular la SMA: Este cálculo es muy sencillo. El valor de la media móvil se calcula tomando 
los precios de cierre de un periodo determinado. Luego se suman cada uno de ellos, y el 
resultado de dicha suma se divide entre la cantidad de periodos tomados. Es decir, la suma 
de n períodos, dividido entre n. Por tanto, la operación sería la siguiente:

SMA = suma de los períodos seleccionados ÷ el número de períodos seleccionados
 */
