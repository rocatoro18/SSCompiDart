using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorDART_RCTR
{
    public enum TipoNodoArbol
    {
        Expresion, Sentencia, Condiciona, NADA
    }
    public enum TipoSentencia
    {
        IF,
        FOR,
        ASIGNACION,
        LEER,
        ESCRIBIR,
        NADA
    }
    public enum tipoExpresion
    {
        Operador,
        Constante,
        Identificador,
        Cadena,
        NADA
    }
    public enum tipoOperador
    {
        Suma,
        Resta,
        Multiplicacion,
        Division,
        NADA
    }
    public enum OperacionCondicional
    {
        IgualIgual,
        MenorQue,
        MayorQue,
        MenorIgualQue,
        MayorIgualQue,
        Diferente,
        NADA
    }

    public class NodoArbol
    {                                      //    IF             EXP             ASIG       for
        public NodoArbol hijoIzquierdo;  //  condicional      operando izq    arbol exp   sentencias
        public NodoArbol hijoDerecho;     // if false          operando der                condicional
        public NodoArbol hijoCentro;      // if true                                       cuerpo del for
        public NodoArbol hermano;     // apunta a la siguiente instruccion (arbol)

        public TipoNodoArbol soyDeTipoNodo;
        public TipoSentencia soySentenciaDeTipo;
        public tipoExpresion SoyDeTipoExpresion;
        public tipoOperador soyDeTipoOperacion;
        public OperacionCondicional soyOperacionCondicionaDeTipo;

        public string lexema;

        //reglas semanticas // atributos
        //comprobacion de tipos y generacion de codigo intermedio
        public TipoDato SoyDeTipoDato;
        public TipoDato tipoValorNodoHijoIzquierdo; // valores sintetizados
        public TipoDato tipoValorNodoHijoDerecho;   // valores sintetizados

        public string memoriaAsignada; // conocer la memoria asignada
        public string valor;   //para hacer calculos codigo intermedio
        public string pCode;  // generar el codigo intermedio
        public string pCode1;  // generar el codigo intermedio
        public string pCode2;  // generar el codigo intermedio
        public string pCode3;  // generar el codigo intermedio
        public int linea;
    }

    public class Arbol
    {
        public NodoClase nombreClaseActiva;
        public string nombreMetodoActivo;

        int contadorIF = 0;
        int contadoELSE = 1;

        NodoArbol SiguienteArbol = new NodoArbol();
        NodoArbol nodoraiz = new NodoArbol();

        public int puntero;
        public List<Token> miListaTokenTemporal;

        public Arbol(List<Token> miListaTokenLexico, NodoClase ClaseActiva, string MetodoActivo)
        {
            puntero = 0;
            miListaTokenTemporal = miListaTokenLexico;
            nombreClaseActiva = ClaseActiva;
            nombreMetodoActivo = MetodoActivo;
        }

        public NodoArbol CrearArbolSintacticoAbstracto()
        {
            //NodoArbol nodoraiz = new NodoArbol();
            nodoraiz = ObtenerSiguienteArbol();
            puntero++;
            if (nodoraiz != null)
            {
                if (miListaTokenTemporal[puntero].Lexema != "$")
                {
                    nodoraiz.hermano = NodoHermano();
                }
            }
            else
            {
                CrearArbolSintacticoAbstracto();
            }
            
            return nodoraiz;
        }

        private NodoArbol NodoHermano()
        {
            NodoArbol t;
            do
            {
                t = ObtenerSiguienteArbol();
                puntero++;
                if (t == null && miListaTokenTemporal.Count == puntero)
                {
                    return t;
                }
            } while (t == null);

            if(miListaTokenTemporal.Count > puntero)
            {
                t.hermano = NodoHermano();
            }

            return t;

            /*
            NodoArbol t = ObtenerSiguienteArbol();
            puntero++;

            if (miListaTokenTemporal[puntero].Lexema != "$")
            {
                t.hermano = NodoHermano();
            }
            return t;
            */
        }

        private NodoArbol ObtenerSiguienteArbol()
        {
            switch (miListaTokenTemporal[puntero].ValorToken)
            {
                case -82: //if  *si*
                    SiguienteArbol = CrearArbolIF();
                    break;
                case -1: //asignacion
                    SiguienteArbol = CrearArbolAsignacion();
                    break;
                case -78: //for *por*
                    SiguienteArbol = CrearArbolFor();
                    break;         // por
                                                   // 
                case -106: // escritura*
                    SiguienteArbol = CrearNodoEscrituraLectura();
                    puntero++;
                    break;    // inout print   *leer* 

                case -113: // lectura*
                    SiguienteArbol = CrearNodoEscrituraLectura();
                    puntero++;
                    break;
                    //return CrearNodoLectura();      // input read    *impresion*

                default:
                    SiguienteArbol = null;
                    break;
            }
            return SiguienteArbol;
        }


        #region Crear ARBOL Expresion
        public NodoArbol CrearArbolExpresion()
        {
            NodoArbol nodoRaiz = Termino();
            while (miListaTokenTemporal[puntero].ValorToken == -6 || miListaTokenTemporal[puntero].ValorToken == -7)
            {
                NodoArbol nodoTemp = NuevoNodoExpresion(tipoExpresion.Operador);
                nodoTemp.hijoIzquierdo = nodoRaiz;
                //nodoTemp.linea = miListaTokenTemporal[puntero].Token;
                /*nodoTemp.soyDeTipoOperacion =
                    miListaTokenTemporal[puntero].Lexema.Equals("+")
                    ? tipoOperador.Suma
                    : tipoOperador.Resta;*/
                switch (miListaTokenTemporal[puntero].ValorToken)
                {
                    case -6: nodoTemp.soyDeTipoOperacion = tipoOperador.Suma; break;
                    case -7: nodoTemp.soyDeTipoOperacion = tipoOperador.Resta; break;
                }
                if (miListaTokenTemporal[puntero].ValorToken == -6)
                {
                    nodoTemp.pCode = "adi;";
                    nodoTemp.lexema = "+";
                }
                else if (miListaTokenTemporal[puntero].ValorToken == -7)
                {
                    nodoTemp.pCode = "sbi;";
                    nodoTemp.lexema = "-";
                }
                else
                {
                    //nodoTemp.lexema = miListaTokenTemporal[puntero].Lexema;
                    nodoRaiz = nodoTemp;
                    puntero++;
                    nodoRaiz.hijoDerecho = Termino();
                }
                
            }

            return nodoRaiz;
        }

        private NodoArbol Termino()
        {
            NodoArbol t = Factor();
            while (miListaTokenTemporal[puntero].ValorToken == -8 || miListaTokenTemporal[puntero].ValorToken == -9 ) // cambiar por su token
            {
                NodoArbol p = NuevoNodoExpresion(tipoExpresion.Operador);
                p.hijoIzquierdo = t;
                p.soyDeTipoOperacion = miListaTokenTemporal[puntero].ValorToken == -8
                    ? tipoOperador.Multiplicacion
                    : tipoOperador.Division;
                if (miListaTokenTemporal[puntero].ValorToken == -8)
                {
                    t.pCode = "mpi;";
                }
                else
                {
                    t.pCode = "div;";
                }

                t.lexema = miListaTokenTemporal[puntero].Lexema;
                t = p;
                puntero++;
                t.hijoDerecho = Factor();
            }
            return t;
        }

        public NodoArbol Factor()
        {
            NodoArbol t = new NodoArbol();

            if (miListaTokenTemporal[puntero].ValorToken == -2) //ENTERO
            {
                t = NuevoNodoExpresion(tipoExpresion.Constante);
                t.pCode = " ldc " + miListaTokenTemporal[puntero].Lexema + ";";
                t.SoyDeTipoDato = TipoDato.INT;
                t.lexema = miListaTokenTemporal[puntero].Lexema;
                t.linea = miListaTokenTemporal[puntero].ValorToken;///puede fallar
                puntero++;
            }
            if (miListaTokenTemporal[puntero].ValorToken == -3)  //decimal
            {
                t = NuevoNodoExpresion(tipoExpresion.Constante);
                t.pCode = " ldc " + miListaTokenTemporal[puntero].Lexema + ";";
                t.lexema = miListaTokenTemporal[puntero].Lexema;
                t.SoyDeTipoDato = TipoDato.DOBLE;
                puntero++;
            }
            else if (miListaTokenTemporal[puntero].ValorToken == -4) //cadenas
            {
                t = NuevoNodoExpresion(tipoExpresion.Cadena);
                t.lexema = miListaTokenTemporal[puntero].Lexema;
                t.SoyDeTipoDato = TipoDato.STRING;
                t.pCode = "" + miListaTokenTemporal[puntero].Lexema ;
                t.linea = miListaTokenTemporal[puntero].ValorToken;
                puntero++;
            }
            else if (miListaTokenTemporal[puntero].ValorToken == -1) // identificador
            {
                t = NuevoNodoExpresion(tipoExpresion.Identificador);
                t.lexema = miListaTokenTemporal[puntero].Lexema;

                t.pCode = " lod " + miListaTokenTemporal[puntero].Lexema + ";"; // gramatica con atributos
                t.SoyDeTipoDato =
                    TablaSimbolos.ObtenerTipoDato(
                        miListaTokenTemporal[puntero].Lexema,
                        nombreClaseActiva, nombreMetodoActivo); // gramatica con atributos
                t.linea = miListaTokenTemporal[puntero].ValorToken;
                puntero++;
            }
            else if (miListaTokenTemporal[puntero].ValorToken == -20)
            {
                puntero++;
                t = CrearArbolExpresion();
                puntero++;
            }
            return t;
        }
        //
        #endregion      
        #region Crear ARBOL Asignacion
        public NodoArbol CrearArbolAsignacion()
        {
            //var sentenciaAsignacion = NuevoNodoSentencia(TipoSentencia.ASIGNACION);
            //if (!(miListaTokenTemporal[puntero+2].ValorToken == -113) && !(miListaTokenTemporal[puntero+1].ValorToken == -20))
            //{
                if (miListaTokenTemporal[puntero].ValorToken == -99 || miListaTokenTemporal[puntero].ValorToken == -100 || miListaTokenTemporal[puntero].ValorToken == -102
                    || miListaTokenTemporal[puntero].ValorToken == -103)
                {
                    puntero++;
                }
                    var sentenciaAsignacion = NuevoNodoSentencia(TipoSentencia.ASIGNACION);
                    sentenciaAsignacion.lexema = miListaTokenTemporal[puntero].Lexema;
                    sentenciaAsignacion.pCode = "lda " + miListaTokenTemporal[puntero].Lexema;
                    sentenciaAsignacion.pCode1 = "sto;";
                    //puntero += 2;
                    sentenciaAsignacion.SoyDeTipoDato = TablaSimbolos.ObtenerTipoDato(miListaTokenTemporal[puntero].Lexema, nombreClaseActiva, nombreMetodoActivo);
                    puntero += 2;
                    sentenciaAsignacion.hijoIzquierdo = CrearArbolExpresion();


            //} 
            //else
            //{
            // puntero ++;
            //ObtenerSiguienteArbol();
            //puntero += 2;
            //}


            return sentenciaAsignacion;

        }


        #endregion       
        #region Crear Arbol If
        public NodoArbol CrearArbolIF()
        {
            var nodoArbolIF = NuevoNodoSentencia(TipoSentencia.IF);
            puntero += 2;
            contadorIF++;
            //nodoArbolIF.hijoIzquierdo = CrearArbolCondicional();
            //puntero += 2;
            //nodoArbolIF.pCode = "fjp" + " " + "Ln";
            nodoArbolIF.pCode = "fjp" + " " + "Ln"+contadorIF;
            nodoArbolIF.hijoIzquierdo = CrearArbolCondicional();
            puntero += 2;
            //error cuando no hay comandos cuando la condicional es verdadera
            // validar que exista codigo en el TRUE
            //nodoArbolIF.hijoCentro = ObtenerSiguienteArbol();
            //puntero += 2;
            nodoArbolIF.pCode2 = "ujp" + " " + "L" + (contadorIF + 1).ToString();
            nodoArbolIF.pCode1 = "lab" + " " + "L" + contadorIF;
            nodoArbolIF.pCode3 = "lab" + " " + "L" + (contadorIF + 1).ToString();
            //codigo cuando sea falso

            if (miListaTokenTemporal[puntero].ValorToken != -23)
            {
                if (miListaTokenTemporal[puntero].ValorToken == -99 || miListaTokenTemporal[puntero].ValorToken == -100 || miListaTokenTemporal[puntero].ValorToken == -102) // Tipos de datos int doble string
                {
                    puntero++;
                    while (miListaTokenTemporal[puntero].ValorToken != -23)
                    {
                        nodoArbolIF.hijoCentro = HermanoSentencia();
                    }
                    if (puntero > miListaTokenTemporal.Count - 1)
                    {
                        puntero--;
                    }
                }

                else
                {
                    while (miListaTokenTemporal[puntero].ValorToken != -23)
                    {
                        contadorIF++;
                        nodoArbolIF.hijoCentro = HermanoSentencia();
                    }

                    if (puntero > miListaTokenTemporal.Count - 1)
                    {
                        puntero--;
                    }
                }
            }

            puntero++;

            if(puntero == miListaTokenTemporal.Count)
            {
                return nodoArbolIF;
            }
            else
            {
                if(miListaTokenTemporal[puntero].ValorToken == -47)//demas
                {
                    puntero += 2;
                    if(miListaTokenTemporal[puntero].ValorToken != -23) // }
                    {
                        if(miListaTokenTemporal[puntero].ValorToken == -99 || miListaTokenTemporal[puntero].ValorToken == -100 || miListaTokenTemporal[puntero].ValorToken == -102) // tipos de datos int doble string
                        {
                            puntero++;
                            nodoArbolIF.hijoDerecho = HermanoSentencia();
                            if (puntero > miListaTokenTemporal.Count -1)
                            {
                                puntero++;
                            }
                        }
                        else
                        {
                            contadorIF++;
                            nodoArbolIF.hijoDerecho = HermanoSentencia();
                            if(puntero > miListaTokenTemporal.Count - 1)
                            {
                                puntero++;
                            }
                        }
                    }
                }
                return nodoArbolIF;
            }

        }

        public NodoArbol HermanoSentencia()
        {
            NodoArbol arbolSentencia;

            do
            {
                if (miListaTokenTemporal[puntero].ValorToken == -99 || miListaTokenTemporal[puntero].ValorToken == -100 || miListaTokenTemporal[puntero].ValorToken == -103 || miListaTokenTemporal[puntero].ValorToken == -102) { puntero++; }
                arbolSentencia = ObtenerSiguienteArbol();
                puntero++;
            } while (arbolSentencia == null);



            if (miListaTokenTemporal.Count > puntero)
            {
                if (miListaTokenTemporal[puntero].ValorToken != -23 && miListaTokenTemporal[puntero].ValorToken != -47)
                {
                    arbolSentencia.hermano = HermanoSentencia();
                }

            }

            return arbolSentencia;
        }

        #endregion
        #region Crear Arbol Condicional 
        public NodoArbol CrearArbolCondicional()
        {
            NodoArbol nodoRaiz = CrearArbolExpresion();

            if (miListaTokenTemporal[puntero].ValorToken == -15
                || miListaTokenTemporal[puntero].ValorToken == -13
                || miListaTokenTemporal[puntero].ValorToken == -14
                || miListaTokenTemporal[puntero].ValorToken == -11
                || miListaTokenTemporal[puntero].ValorToken ==  -12)
            {
                NodoArbol nodoTemp = NuevoNodoCondicional();

                switch (miListaTokenTemporal[puntero].ValorToken)
                {
                    case -15:
                        nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.IgualIgual;
                        nodoTemp.pCode = "equ";
                        break;
                    case -13:
                        nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MenorIgualQue;
                        nodoTemp.pCode = "leq";
                        break;
                    case -14:
                        nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MayorIgualQue;
                        nodoTemp.pCode = "geq";
                        break;
                    case -11:
                        nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MayorQue;
                        nodoTemp.pCode = "grt";
                        break;
                    case -12:
                        nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MenorQue;
                        nodoTemp.pCode = "lss";
                        break;
                    default:
                        break;
                }
                nodoTemp.hijoIzquierdo = nodoRaiz;
                nodoRaiz = nodoTemp;
                puntero++;
                nodoRaiz.hijoDerecho = CrearArbolExpresion();
            }

            return nodoRaiz;
        }
        #endregion        
        #region Crear Arbol For

        private NodoArbol CrearArbolFor()
        {
            var nodoArbolfor = NuevoNodoSentencia(TipoSentencia.FOR);
            puntero += 2;
            nodoArbolfor.hijoIzquierdo = CrearArbolAsignacion();    // sentencias
            nodoArbolfor.pCode = "loop;";
            ////////////
            puntero++;
            nodoArbolfor.hijoDerecho = CrearArbolCondicional();    // condicional
            ///////////////
            puntero ++;
            nodoArbolfor.hijoCentro = HermanoSentencia();

            nodoArbolfor.pCode3 = "L end;";

            return nodoArbolfor;
        }

        #endregion
        #region Crear Arbol de Escritura
        private NodoArbol CrearNodoEscritura()
        {
            //throw new NotImplementedException();
            var nodoArbolWrite = NuevoNodoSentencia(TipoSentencia.ESCRIBIR);

            puntero += 2;
            nodoArbolWrite.hijoCentro = CrearArbolExpresion();

            return nodoArbolWrite;
        }
        #endregion
        #region Crear Arbol Lectura
        private NodoArbol CrearNodoLectura()
        {
            //throw new NotImplementedException();
            var nodoArbolRead = NuevoNodoSentencia(TipoSentencia.LEER);

            puntero -=2;
            nodoArbolRead.hijoCentro = CrearArbolExpresion();
            puntero +=2; //El método del árbol lo mueve otro espacio, despues del leer
            return nodoArbolRead;
        }

        private NodoArbol CrearNodoEscrituraLectura()
        {
            NodoArbol CrearNodoEscrituraLectura;

            if ((miListaTokenTemporal[puntero + 3].ValorToken == -106))
            {
                CrearNodoEscrituraLectura = NuevoNodoSentencia(TipoSentencia.ESCRIBIR);
                if (miListaTokenTemporal[puntero + 5].ValorToken == -1)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TablaSimbolos.ObtenerTipoDato(miListaTokenTemporal[puntero+5].Lexema, nombreClaseActiva, nombreMetodoActivo);
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                    CrearNodoEscrituraLectura.lexema = miListaTokenTemporal[puntero + 5].ValorToken.ToString();
                }
                else if (miListaTokenTemporal[puntero + 5].ValorToken == -2)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TipoDato.INT;
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                    CrearNodoEscrituraLectura.lexema = miListaTokenTemporal[puntero + 5].ValorToken.ToString();
                }
                else if (miListaTokenTemporal[puntero + 5].ValorToken == -3)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TipoDato.DOBLE;
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                    CrearNodoEscrituraLectura.lexema = miListaTokenTemporal[puntero + 5].ValorToken.ToString();
                }
                else if (miListaTokenTemporal[puntero + 5].ValorToken == -4)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TipoDato.STRING;
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                    CrearNodoEscrituraLectura.lexema = miListaTokenTemporal[puntero + 5].ValorToken.ToString();
                }
                puntero += 5;
                CrearNodoEscrituraLectura.hijoIzquierdo = CrearArbolExpresion();
                CrearNodoEscrituraLectura.pCode = "wr";
                puntero--;
            }
            else
            {
                CrearNodoEscrituraLectura = NuevoNodoSentencia(TipoSentencia.LEER);
                if (miListaTokenTemporal[puntero + 5].ValorToken == -1)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TablaSimbolos.ObtenerTipoDato(miListaTokenTemporal[puntero + 5].Lexema, nombreClaseActiva, nombreMetodoActivo);
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                }
                else if (miListaTokenTemporal[puntero + 5].ValorToken == -2)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TipoDato.INT;
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                }
                else if (miListaTokenTemporal[puntero + 5].ValorToken == -3)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TipoDato.DOBLE;
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                }
                else if (miListaTokenTemporal[puntero + 5].ValorToken == -4)
                {
                    CrearNodoEscrituraLectura.SoyDeTipoDato = TipoDato.STRING;
                    CrearNodoEscrituraLectura.linea = miListaTokenTemporal[puntero + 5].Linea;
                }
                puntero += 5;
                CrearNodoEscrituraLectura.hijoIzquierdo = CrearArbolExpresion();
                CrearNodoEscrituraLectura.pCode = "rd";
                puntero--;
            }

            return CrearNodoEscrituraLectura;
        }

        #endregion

        #region Crear diferentes tipos de NodoArbol
        public NodoArbol NuevoNodoExpresion(tipoExpresion tipoExpresion)
        {
            NodoArbol nodo = new NodoArbol();

            nodo.soyDeTipoNodo = TipoNodoArbol.Expresion;

            nodo.SoyDeTipoExpresion = tipoExpresion;
            nodo.soyDeTipoOperacion = tipoOperador.NADA;
            nodo.SoyDeTipoDato = TipoDato.NADA;

            nodo.soySentenciaDeTipo = TipoSentencia.NADA;
            nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
            nodo.tipoValorNodoHijoDerecho = TipoDato.NADA;
            nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
            nodo.tipoValorNodoHijoIzquierdo = TipoDato.NADA;
            return nodo;
        }
        public NodoArbol NuevoNodoSentencia(TipoSentencia tipoSentencia)
        {
            NodoArbol nodo = new NodoArbol();
            nodo.soyDeTipoNodo = TipoNodoArbol.Sentencia;
            nodo.soySentenciaDeTipo = tipoSentencia;

            nodo.SoyDeTipoExpresion = tipoExpresion.NADA;
            nodo.soyDeTipoOperacion = tipoOperador.NADA;
            nodo.SoyDeTipoDato = TipoDato.NADA;


            nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
            nodo.tipoValorNodoHijoDerecho = TipoDato.NADA;
            nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
            nodo.tipoValorNodoHijoIzquierdo = TipoDato.NADA;
            return nodo;

        }
        public NodoArbol NuevoNodoCondicional()
        {
            NodoArbol nodo = new NodoArbol();
            nodo.soyDeTipoNodo = TipoNodoArbol.Condiciona;

            nodo.SoyDeTipoExpresion = tipoExpresion.NADA;
            nodo.soyDeTipoOperacion = tipoOperador.NADA;
            nodo.SoyDeTipoDato = TipoDato.NADA;

            nodo.soySentenciaDeTipo = TipoSentencia.NADA;
            nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
            nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
            nodo.tipoValorNodoHijoDerecho = TipoDato.NADA;
            nodo.tipoValorNodoHijoIzquierdo = TipoDato.NADA;
            return nodo;

        }
        #endregion
    }
}
