using System.ServiceProcess;

namespace CeecDataTransfer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new CeecDataTransfer()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
