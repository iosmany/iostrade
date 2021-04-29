using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iostrade.Core.Indicadores
{
    class EMA
    {
        public EMA()
        {
        }

        public void Calcula()
        { 

        }
    }
}

/*
1. Calcular SMA.

2. Calcular el multiplicador para ponderar la EMA: Este factor está determinado por 
el número de períodos de la EMA. Por lo que para su cálculo, se divide 2 entre la 
cantidad de periodos tomados más 1. Por lo tanto, la fórmula quedará de la siguiente manera:

Multiplicador = [2 ÷ (cantidad de períodos seleccionados + 1)] 

3. EMA actual = [Precio de cierre – EMA (día anterior)] x multiplicador + EMA (día anterior)

 
 */
