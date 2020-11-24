using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TcfExport.Lib;

namespace TcfExport.App
{
  class Program
  {
    static void Main(string[] args)
    {
      var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
      {
        services.AddLogging();
        
        // simple clean uninitialized class should be fine for now...
        services.AddSingleton<TcfExportRecordsProcessor>();
        
        services.AddHostedService(serviceProvider => new ApplicationService(
          args,
          serviceProvider.GetService<ILogger<ApplicationService>>(),
          serviceProvider.GetService<IConfiguration>(),
          serviceProvider.GetService<TcfExportRecordsProcessor>()));
      });

      host.Build().Run();
      
      // var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();
      //
      // Console.WriteLine("tcf banking file analyzer version {0}.{1}.{2}", 
      //   entryAssembly.FullName, entryAssembly.ImageRuntimeVersion, entryAssembly.EntryPoint);
      // foreach (string s in args)
      // {
      //   Console.WriteLine("arg: {0}", s);
      //
      //   if (File.Exists(s))
      //   {
      //     var fileData = File.OpenText(s);
      //     CsvHelper.CsvDataReader dataReader = new CsvDataReader(new CsvReader(fileData));          
      //   }
      //   else
      //   {
      //     Console.WriteLine("");
      //   }

      }
    }
  }
