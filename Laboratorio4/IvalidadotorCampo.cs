using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio4
{
    public interface IvalidadotorCampo
    {
        bool EsValido(string valor);
        string MensajeError { get;  }
    }
}
