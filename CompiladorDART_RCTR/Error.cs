
namespace CompiladorDART_RCTR
{
    public enum tipoError
    {
        Lexico,
        Sintactico,
        Semantico,
        CodigoIntermedio,
        Ejecucion
    }
    public class Error
    {
        private int linea;
        private int codigo;
        private tipoError tipoError;
        private string mensajeError;

        public int Linea
        {
            get
            {
                return linea;
            }
            set
            {
                linea = value;
            }
        }

        public int Codigo
        {
            get
            {
                return codigo;
            }
            set
            {
                codigo = value;
            }
        }

        public tipoError TipoError
        {
            get
            {
                return tipoError;
            }
            set
            {
                tipoError = value;
            }
        }

        public string MensajeError
        {
            get
            {
                return mensajeError;
            }
            set
            {
                mensajeError = value;
            }
        }

    }
}
