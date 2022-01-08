
namespace CompiladorDART_RCTR
{

    public enum TipoToken
    {
        Identificador,
        OperadorAritmetico,
        OperadorRelacional,
        OperadorLogico,
        SimboloSimple,
        OperadorAsignacion,
        PalabraReservada,
        SimboloDoble,
        NumeroEntero,
        NumeroDecimal,
        Cadena,
        Caracter,
        Incremento,
        Asignacion,
        Comentario,
        ComentarioMultilinea,
        Desconocido,
        Lambda
    }

    public class Token
    {
        private TipoToken tipoToken;
        private int valorToken;
        private string lexema;
        private int linea;

        public int ValorToken
        {
            get
            {
                return valorToken;
            }
            set
            {
                valorToken = value;
            }
        }

        public string Lexema
        {
            get
            {
                return lexema;
            }
            set
            {
                lexema = value;
            }
        }

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

        public TipoToken TipoToken
        {
            get
            {
                return tipoToken;
            }
            set
            {
                tipoToken = value;
            }
        }

    }
}
