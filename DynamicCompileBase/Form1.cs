using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;

namespace DynamicCompileBase
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //get the code to compile
            string strSourceCode = this.txtSource.Text;

            // 1.Create a new CSharpCodePrivoder instance
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            objCompilerParameters.GenerateInMemory = true;

            // 3.CompilerResults: Complile the code snippet by calling a method from the provider
            CompilerResults cr = objCSharpCodePrivoder.CompileAssemblyFromSource(objCompilerParameters, strSourceCode);

            if (cr.Errors.HasErrors)
            {
                string strErrorMsg = cr.Errors.Count.ToString() + " Errors:";

                for (int x = 0; x < cr.Errors.Count; x++)
                {
                    strErrorMsg = strErrorMsg + "/r/nLine: " +
                                 cr.Errors[x].Line.ToString() + " - " +
                                 cr.Errors[x].ErrorText;
                }

                this.txtResult.Text = strErrorMsg;
                MessageBox.Show("There were build erros, please modify your code.", "Compiling Error");
                return;
            }

            // 4. Invoke the method by using Reflection
            Assembly objAssembly = cr.CompiledAssembly;
            object objClass = objAssembly.CreateInstance("Dynamicly.HelloWorld");

            if (objClass == null)
            {
                this.txtResult.Text = "Error: " + "Couldn't load class.";
                return;
            }

            object[] objCodeParms = new object[1];
            objCodeParms[0] = "Allan.";

            string strResult = (string)objClass.GetType().InvokeMember(
                       "GetTime", BindingFlags.InvokeMethod, null, objClass, objCodeParms);
            this.txtResult.Text = strResult;
        }
    }
}
