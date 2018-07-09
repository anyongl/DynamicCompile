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
using RemoteAccess;

namespace DynamicCompileAppDomain
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // get the code to compile
            string strSourceCode = this.txtSource.Text;

            // 0. Create an addtional AppDomain
            AppDomainSetup objSetup = new AppDomainSetup();
            objSetup.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            AppDomain objAppDomain = AppDomain.CreateDomain("MyAppDomain", null, objSetup);

            // 1.Create a new CSharpCodePrivoder instance
            CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();

            // 2.Sets the runtime compiling parameters by crating a new CompilerParameters instance
            CompilerParameters objCompilerParameters = new CompilerParameters();
            objCompilerParameters.ReferencedAssemblies.Add("System.dll");
            objCompilerParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            // Load the remote loader interface
            objCompilerParameters.ReferencedAssemblies.Add("RemoteAccess.dll");

            // Load the resulting assembly into memory
            objCompilerParameters.GenerateInMemory = false;
            objCompilerParameters.OutputAssembly = "DynamicalCode.dll";

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
            RemoteLoaderFactory factory = (RemoteLoaderFactory)objAppDomain.CreateInstance("RemoteAccess", "RemoteAccess.RemoteLoaderFactory").Unwrap();

            // with help of factory, create a real 'LiveClass' instance
            object objObject = factory.Create("DynamicalCode.dll", "Dynamicly.HelloWorld", null);

            if (objObject == null)
            {
                this.txtResult.Text = "Error: " + "Couldn't load class.";
                return;
            }

            // *** Cast object to remote interface, avoid loading type info
            IRemoteInterface objRemote = (IRemoteInterface)objObject;

            object[] objCodeParms = new object[1];
            objCodeParms[0] = "Allan.";
            string strResult = (string)objRemote.Invoke("GetTime", objCodeParms);
            this.txtResult.Text = strResult;

            //Dispose the objects and unload the generated DLLs.
            objRemote = null;
            AppDomain.Unload(objAppDomain);
            System.IO.File.Delete("DynamicalCode.dll");
        }
    }
}
