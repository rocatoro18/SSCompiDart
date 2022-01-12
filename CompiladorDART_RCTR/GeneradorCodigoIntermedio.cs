using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiladorDART_RCTR
{
    class GeneradorCodigoIntermedio
    {
        private StreamWriter sw;

        public GeneradorCodigoIntermedio()
        {
            sw = new StreamWriter("C:\\Users\\rocat\\Desktop\\PCODE.txt");

            sw.Close();
        }

        public void Ejecutar(NodoArbol miArbol)
        {
            ObtenerSiguienteCodigoIntermedio(miArbol);
            if (miArbol.hermano != null)
            {
                Ejecutar(miArbol.hermano);
            }
        }

        private void ObtenerSiguienteCodigoIntermedio(NodoArbol miArbol)
        {
            if (miArbol.soyDeTipoNodo == TipoNodoArbol.Sentencia && miArbol.soySentenciaDeTipo == TipoSentencia.ASIGNACION)
            {
                GenerarCodigoIntermedioAsignacion(miArbol);
            }
            else if (miArbol.soyDeTipoNodo == TipoNodoArbol.Sentencia && miArbol.soySentenciaDeTipo == TipoSentencia.FOR)
            {
                GenerarCodigoIntermedioFor(miArbol);
            }
            else if (miArbol.soyDeTipoNodo == TipoNodoArbol.Sentencia && miArbol.soySentenciaDeTipo == TipoSentencia.IF)
            {
                GenerarCodigoIntermedioIF(miArbol);
            }
            else if (miArbol.soyDeTipoNodo == TipoNodoArbol.Sentencia && miArbol.soySentenciaDeTipo == TipoSentencia.ESCRIBIR)
            {
                GenerarCodigoIntermedioEscribir(miArbol);
            }
        }

        public void GenerarCodigoIntermedioExpresion(NodoArbol miArbol)
        {
            if (miArbol.hijoIzquierdo != null) // tiene izquierda?
                GenerarCodigoIntermedioExpresion(miArbol.hijoIzquierdo);

            if (miArbol.hijoDerecho != null)  // tiene derecha?
                GenerarCodigoIntermedioExpresion(miArbol.hijoDerecho);

            // imprime            
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode);
            sw.Close();



        }

        public void GenerarCodigoIntermedioAsignacion(NodoArbol miArbol)
        {
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode);  // lda + lexema
            sw.Close();

            GenerarCodigoIntermedioExpresion(miArbol.hijoIzquierdo); // codep para expresion

            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode1); //sto
            sw.Close();

        }

        public void GenerarCodigoIntermedioIF(NodoArbol miArbol)
        {
            GenerarCodigoIntermedioCondicional(miArbol);
            //sw = File.AppendText("C:\\Users\\julio\\Documents\\PCODE.txt");
            ////sw.WriteLine(miArbol.pCode); //sto
            //sw.Close();
            if (miArbol.hijoCentro != null)
            {
                ObtenerSiguienteCodigoIntermedio(miArbol.hijoCentro);
                if (miArbol.hijoCentro.hermano != null)
                {
                    ObtenerSiguienteCodigoIntermedio(miArbol.hijoCentro.hermano);

                }
            }

            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode2); //sto
            sw.Close();
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode1); //sto
            sw.Close();

            if (miArbol.hijoDerecho != null)
            {
                ObtenerSiguienteCodigoIntermedio(miArbol.hijoDerecho);

            }
            /* if (miArbol.hijoDerecho.hermano != null)
             {
                 ObtenerSiguienteCodigoIntermedio(miArbol.hijoDerecho.hermano);

             }*/
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode3); //sto
            sw.Close();
            /* sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
             sw.WriteLine(miArbol.pCode1); //sto
             sw.Close();
             */
        }
        //ya jala pero genera un salto de linea wr con lo que viene codigo comentado ponia el codigo p alrevez
        public void GenerarCodigoIntermedioEscribir(NodoArbol miArbol)
        {
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode);
            sw.Close();
            if (miArbol.hijoIzquierdo != null) // tiene izquierda?
                GenerarCodigoIntermedioExpresion(miArbol.hijoIzquierdo);

            //if (miArbol.hijoDerecho != null)  // tiene derecha?
            //    GenerarCodigoIntermedioExpresion(miArbol.hijoDerecho);

            // imprime            
            /* sw = File.AppendText("C:\\Users\\peche\\Documents\\PCODE.txt");
             sw.WriteLine(miArbol.pCode);
             sw.Close();*/


        }

        public void GenerarCodigoIntermedioCondicional(NodoArbol miArbol)
        {
            if (miArbol.hijoIzquierdo != null)
            {
                GenerarCodigoIntermedioExpresion(miArbol.hijoIzquierdo);
            }

            //GenerarCodigoIntermedioExpresion(miArbol.hijoDerecho);

            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode);
            sw.Close();

        }
        /*

        public void GenerarCodigoIntermedioRead(NodoArbol miArbol)
        {
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            //sw.WriteLine(miArbol.pCode);
            sw.Close();
            if (miArbol.hijoIzquierdo != null)
            {
                ObtenerSiguienteCodigoIntermedio(miArbol.hijoIzquierdo);
            }
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode);
            sw.Close();
        }
        */
        public void GenerarCodigoIntermedioFor(NodoArbol miArbol)
        {
            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode);
            sw.Close();

            GenerarCodigoIntermedioAsignacion(miArbol.hijoIzquierdo);
            if (miArbol.hijoCentro != null)
            {
                ObtenerSiguienteCodigoIntermedio(miArbol.hijoCentro);
            }

            GenerarCodigoIntermedioCondicional(miArbol.hijoDerecho);

            sw = File.AppendText("C:\\Users\\rocat\\Desktop\\PCODE.txt");
            sw.WriteLine(miArbol.pCode3);
            sw.Close();
        }

    }

}
