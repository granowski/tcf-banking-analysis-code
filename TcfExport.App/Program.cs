using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TcfExport.Lib;

namespace TcfExport.App
{
  public partial class ApplicationService : IHostedService {
    ILogger<ApplicationService> _logger;
    IConfiguration _configuration;
    string[] _args;
    TcfExportRecordsProcessor _recordsProcessor;

    public ApplicationService(
        string[] args, 
        ILogger<ApplicationService> logger, 
        IConfiguration configuration,
        TcfExportRecordsProcessor recordsProcessor)
    {
      _logger = logger;
      _configuration = configuration;
      _args = args;
      _recordsProcessor = recordsProcessor;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
      if (_args.Length == 0)
      {
        _logger.LogInformation("no file argument supplied to program, ending...");
        return Task.CompletedTask;
      }
      
      string relativeFilePath = _args[0];

      Console.WriteLine("current working directory -> {0}", Directory.GetCurrentDirectory());
      if (!File.Exists(relativeFilePath))
      {
        _logger.LogInformation($"could not find file '{_args[0]}' to process");
        return Task.CompletedTask;
      }

      var stream = File.OpenRead(relativeFilePath);
      var sr = new StreamReader(stream);

      var csvReader = new CsvReader(sr, CultureInfo.InvariantCulture);

      IEnumerable<TcfExportRecord> records = csvReader.GetRecords<TcfExportRecord>().ToArray();

      _recordsProcessor.AddRange(records);

      var data = _recordsProcessor.GetDataList();
      
      //
      // Console.WriteLine("Date:Credit:Debit");
      //
      // foreach (TcfExportRecord record in records)
      // {
      //   Console.WriteLine("{0};{1};{2}", record.Date, record.Credit, record.Debit);
      // }
      //
      // TcfExportRecords tcfData = new TcfExportRecords();
      // tcfData.AddRange(records);
      //
      // decimal averageDayDebit = tcfData.GetDataList().GetAverageDayDebit();
      // decimal averageDayCredit = tcfData.GetDataList().GetAverageDayCredit();
      //
      // var dataList = tcfData.GetDataList();
      //
      // var eachDay = dataList.GroupBy(data => data.Date);
      //
      // foreach (IGrouping<DateTime,TcfExportData> datas in eachDay)
      // {
      //   Console.WriteLine("{0} -> {1}", datas.Key, datas.Sum(d => d.Credit + d.Debit));
      // }
      //
      // Console.WriteLine("Average Day Debit: {0}", averageDayDebit);
      // Console.WriteLine("Average Day Credit: {0}", averageDayCredit);
      //
      
      
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
  
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
