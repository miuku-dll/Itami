using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ItamiAPI;
using System.Threading;
using System.IO;
namespace Itami
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        
       
        static void Main()
        {
            try
            {
                
                AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                {
                    var arr = "";
                    if (File.Exists("logs.txt"))
                    {
                        arr = File.ReadAllText("logs.txt");
                    }
                    File.WriteAllText(arr + "\n[ERROR]:" + args.ExceptionObject,"logs.txt");
                    MessageBox.Show("Application Stopped working,log saved");
                    Environment.Exit(1);
                };
                Clarity clarity = new Clarity();
                clarity.Start();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
