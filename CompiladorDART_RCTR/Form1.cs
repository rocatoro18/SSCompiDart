using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace CompiladorDART_RCTR
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ejecutarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var lexico = new Lexico(textBox1.Text);
                lexico.EjecutarLexico();

                var objSintactico = new Sintactico(lexico.listaToken);
                objSintactico.EjecutarSintactico(objSintactico.listaTokens);

            
                List<Error> listaErroresLexico = lexico.listaErrorLexico;
                List<Error> listaErroresSintactico = objSintactico.listaError;

                List<Error> listaErrores = listaErroresLexico.Union(listaErroresSintactico).ToList();


                if (listaErrores != null)
                {
                    var arbolito = new Arbol(lexico.listaToken,objSintactico.devolverClase(),objSintactico.devolverMetodo());  //  <-------- 
                    var arbolito2 = arbolito.CrearArbolSintacticoAbstracto();
                    //CrearArbolSintacticoAbstracto


                    //Llamar a la verificación de tipos
                    //VerificadorTipos verificar = new VerificadorTipos(arbolito2);
                }

                //Tablas que se muestran en el Form
                var Lista = new BindingList<Token>(lexico.listaToken);
                dataGridView1.DataSource = null;
                dataGridView2.DataSource = null;
                dataGridView6.DataSource = null;
                dataGridView1.DataSource = Lista;
                dataGridView3.DataSource = null;
                dataGridView3.DataSource = listaErrores;


                var listaClases = (from x in TablaSimbolos.TablaSimbolosClase
                                   select x.Value).ToList();

                var listademetodos = (from x in TablaSimbolos.ClaseActiva.TSM
                                      select x.Value);

                
                var listadeatributosGRW = (from x in TablaSimbolos.ClaseActiva.TSA
                                           select x.Value);
                
                var listademetodosvariables = (from x in TablaSimbolos.MetodoActivo.TablaSimbolosVariables
                                               select x.Value);

                

                dataGridView2.DataSource = listadeatributosGRW.ToList(); ;   // <- - - - - otra forma.
                dataGridView5.DataSource = listademetodos.ToList();
                dataGridView6.DataSource = listademetodosvariables.ToList();

                dataGridView4.DataSource = listaClases.ToList();

                dataGridView3.DataSource = lexico.listaErrorLexico.Union(objSintactico.listaError.Union(TablaSimbolos.listaErroresSemantico)).ToList();
            } 
            catch
            {
                MessageBox.Show("Tabla de simbolos variables error *TEMPORAL*","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
