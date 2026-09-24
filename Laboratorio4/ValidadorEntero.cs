using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratorio4
{
    public class ValidadorEntero : IvalidadotorCampo
    {
        public string MensajeError { get; private set; } = string.Empty;

        public bool EsValido(string valor)
        {
            if(string.IsNullOrWhiteSpace(valor) || !int.TryParse(valor, out int resultado) | resultado < 0)
            {
                MensajeError = "Debe ingresarun numero entero valido";
                return false;
            }//fin de IsNullOrWhiteSpace

            return true;
        }//Fin del metodo EsValido
    }
}
