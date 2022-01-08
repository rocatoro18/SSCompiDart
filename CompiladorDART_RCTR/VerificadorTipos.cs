using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorDART_RCTR
{
    public class VerificadorTipos
    {

        //copia del arbol original
        public NodoArbol arbolSintactico;

        /// <summary>
        /// constructor donde recibe el arbol original
        /// </summary>
        /// <param name="arbolSintactico"></param>
        public VerificadorTipos(NodoArbol arbolSintactico)
        {
            this.arbolSintactico = arbolSintactico;
            EjecutarVerificacionTipos(arbolSintactico);
        }

        public void EjecutarVerificacionTipos(NodoArbol arbolSintactico)
        {
            obtenerSiguienteVerificacion(arbolSintactico);
            if (arbolSintactico.hermano != null)
            {
                EjecutarVerificacionTipos(arbolSintactico.hermano);
            }
        }

        private void obtenerSiguienteVerificacion(NodoArbol arbolSintactico)
        {
            if (arbolSintactico.soyDeTipoNodo == TipoNodoArbol.Sentencia
               && arbolSintactico.soySentenciaDeTipo == TipoSentencia.ASIGNACION)
            {
                RecorridoAsignacion(arbolSintactico);
            }
            if (arbolSintactico.soyDeTipoNodo == TipoNodoArbol.Sentencia
               && arbolSintactico.soySentenciaDeTipo == TipoSentencia.FOR)
            {
                // RecorridoFor(arbolSintactico);
            }
            if (arbolSintactico.soyDeTipoNodo == TipoNodoArbol.Sentencia
               && arbolSintactico.soySentenciaDeTipo == TipoSentencia.IF)
            {
                RecorridoIF(arbolSintactico);
            }
            if (arbolSintactico.soyDeTipoNodo == TipoNodoArbol.Sentencia
                 && arbolSintactico.soySentenciaDeTipo == TipoSentencia.ESCRIBIR)
            {
                //RecorridoESCRIBIR(arbolSintactico);
            }

            if (arbolSintactico.soyDeTipoNodo == TipoNodoArbol.Sentencia
                && arbolSintactico.soySentenciaDeTipo == TipoSentencia.LEER)
            {
                //RecorridoLEER(arbolSintactico);
            }
        }

        public void RecorridoIF(NodoArbol miArbol)
        {
            RecorridoCondicional(miArbol.hijoIzquierdo);
            EjecutarVerificacionTipos(miArbol.hijoCentro);
            EjecutarVerificacionTipos(miArbol.hijoIzquierdo);

        }
        public void RecorridoCondicional(NodoArbol miArbol)
        {
            var tipoHijoIzquierdo = RecorridoPostOrdenExpresiones(miArbol.hijoIzquierdo);
            var tipoHijoDerecho = RecorridoPostOrdenExpresiones(miArbol.hijoDerecho);

            try
            {
                var tipoCondicional = FuncionEquivalenciaCondcional(tipoHijoIzquierdo,
                 tipoHijoDerecho, miArbol.soyOperacionCondicionaDeTipo);
            }
            catch (Exception)
            {

                var error = new Error()
                {
                    Codigo = 700,
                    Linea = miArbol.linea,
                    MensajeError = string.Format("el operador {0} no se puede aplicar a operandos del tipo {1}, {2}",
                miArbol.soyOperacionCondicionaDeTipo, tipoHijoDerecho, tipoHijoIzquierdo),
                    TipoError = tipoError.Semantico
                };
                TablaSimbolos.listaErroresSemantico.Add(error);

            }


        }
        private void RecorridoAsignacion(NodoArbol miArbol)
        {
            miArbol.tipoValorNodoHijoIzquierdo =
                RecorridoPostOrdenExpresiones(miArbol.hijoIzquierdo);

            if (miArbol.SoyDeTipoDato != miArbol.tipoValorNodoHijoIzquierdo)
            {
                var error = new Error()
                {
                    Codigo = 700,
                    Linea = miArbol.linea,
                    MensajeError = string.Format("no se puede asinar un tipo {1} a un tipo {0}",
                    miArbol.SoyDeTipoDato, miArbol.tipoValorNodoHijoIzquierdo),
                    TipoError = tipoError.Semantico
                };

                TablaSimbolos.listaErroresSemantico.Add(error);
            }
        }

        private TipoDato RecorridoPostOrdenExpresiones(NodoArbol miArbol)
        {
            if (miArbol.hijoIzquierdo != null)
                miArbol.tipoValorNodoHijoIzquierdo =
                     RecorridoPostOrdenExpresiones(miArbol.hijoIzquierdo); 

            if (miArbol.hijoDerecho != null)
                miArbol.tipoValorNodoHijoDerecho =
                    RecorridoPostOrdenExpresiones(miArbol.hijoDerecho);

            if (miArbol.SoyDeTipoExpresion == tipoExpresion.Operador)
            {
                try
                {
                    return FuncionEquivalencia(miArbol.tipoValorNodoHijoIzquierdo,
                     miArbol.tipoValorNodoHijoDerecho, miArbol.soyDeTipoOperacion);
                }
                catch (Exception)
                {

                    var error = new Error()
                    {
                        Codigo = 700,
                        Linea = miArbol.linea,
                        MensajeError = string.Format("no se puede hacer la operacion" + miArbol.soyDeTipoOperacion + " con un tipo {1} y un tipo {0}",
                    miArbol.tipoValorNodoHijoDerecho, miArbol.tipoValorNodoHijoIzquierdo),
                        TipoError = tipoError.Semantico
                    };
                    TablaSimbolos.listaErroresSemantico.Add(error);

                }

            }

            else if (miArbol.SoyDeTipoExpresion == tipoExpresion.Constante)
            {
                return miArbol.SoyDeTipoDato;
            }
            else if (miArbol.SoyDeTipoExpresion == tipoExpresion.Identificador)
            {
                return miArbol.SoyDeTipoDato;
            }
            return TipoDato.NADA;
        }

        private TipoDato FuncionEquivalencia(TipoDato tipoValorNodoHijoIzquierdo, TipoDato tipoValorNodoHijoDerecho,
            tipoOperador soyDeTipoOperacion)
        {

            if (tipoValorNodoHijoIzquierdo == TipoDato.STRING
               && tipoValorNodoHijoDerecho == TipoDato.CHAR
               && soyDeTipoOperacion == tipoOperador.Suma)
            {
                return TipoDato.STRING;
            }



            if (tipoValorNodoHijoIzquierdo == TipoDato.STRING
                && tipoValorNodoHijoDerecho == TipoDato.STRING
                && soyDeTipoOperacion == tipoOperador.Suma)
            {
                return TipoDato.STRING;
            }

            if (tipoValorNodoHijoIzquierdo == TipoDato.INT
                && tipoValorNodoHijoDerecho == TipoDato.INT
                && soyDeTipoOperacion == tipoOperador.Suma)
            {
                return TipoDato.INT;
            }

            if (tipoValorNodoHijoIzquierdo == TipoDato.INT
                && tipoValorNodoHijoDerecho == TipoDato.INT
                && soyDeTipoOperacion == tipoOperador.Resta)
            {
                return TipoDato.INT;
            }

            if (tipoValorNodoHijoIzquierdo == TipoDato.INT
               && tipoValorNodoHijoDerecho == TipoDato.INT
               && soyDeTipoOperacion == tipoOperador.Multiplicacion)
            {
                return TipoDato.INT;
            }
            if (tipoValorNodoHijoIzquierdo == TipoDato.INT
               && tipoValorNodoHijoDerecho == TipoDato.INT
               && soyDeTipoOperacion == tipoOperador.Division)
            {
                return TipoDato.DOBLE;
            }

            if (tipoValorNodoHijoIzquierdo == TipoDato.DOBLE
           && tipoValorNodoHijoDerecho == TipoDato.DOBLE
           && (soyDeTipoOperacion == tipoOperador.Suma
               || soyDeTipoOperacion == tipoOperador.Resta
               || soyDeTipoOperacion == tipoOperador.Multiplicacion
               || soyDeTipoOperacion == tipoOperador.Division))
            {
                return TipoDato.DOBLE;
            }

            if (tipoValorNodoHijoIzquierdo == TipoDato.DOBLE
          && tipoValorNodoHijoDerecho == TipoDato.INT
          && (soyDeTipoOperacion == tipoOperador.Suma
              || soyDeTipoOperacion == tipoOperador.Resta
              || soyDeTipoOperacion == tipoOperador.Multiplicacion
              || soyDeTipoOperacion == tipoOperador.Division))
            {
                return TipoDato.DOBLE;
            }


            if (tipoValorNodoHijoIzquierdo == TipoDato.INT
         && tipoValorNodoHijoDerecho == TipoDato.DOBLE
         && (soyDeTipoOperacion == tipoOperador.Suma
             || soyDeTipoOperacion == tipoOperador.Resta
             || soyDeTipoOperacion == tipoOperador.Multiplicacion
             || soyDeTipoOperacion == tipoOperador.Division))
            {
                return TipoDato.DOBLE;
            }


            throw new Exception();
        }

        private TipoDato FuncionEquivalenciaCondcional(TipoDato tipoValorNodoHijoIzquierdo, TipoDato tipoValorNodoHijoDerecho,
            OperacionCondicional soyDeTipoOperacion)
        {
            if (tipoValorNodoHijoIzquierdo == TipoDato.INT
                && tipoValorNodoHijoDerecho == TipoDato.INT
                && (soyDeTipoOperacion == OperacionCondicional.MayorQue
                || soyDeTipoOperacion == OperacionCondicional.MenorQue
                || soyDeTipoOperacion == OperacionCondicional.Diferente
                || soyDeTipoOperacion == OperacionCondicional.IgualIgual
                || soyDeTipoOperacion == OperacionCondicional.MenorIgualQue
                || soyDeTipoOperacion == OperacionCondicional.MayorIgualQue))
            {
                return TipoDato.BOOL;
            }



            throw new Exception();
        }

    }


}
