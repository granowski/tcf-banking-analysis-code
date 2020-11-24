using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Configuration;
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

      var trans = data.GetExportDataListInTimeSpan(
        new DateTime(2020, 11, 01), 
        new TimeSpan(5, 0, 0, 0));

      trans.Sort((d1, d2) =>
      {
        if (d1.Date < d2.Date) return -1;
        if (d1.Date > d2.Date) return 1;
        return 0;
      });
      Console.WriteLine("first record -> {0}:{1}:{2}", trans.First().Date, trans.First().Credit, trans.First().Debit);
      Console.WriteLine("last  record -> {0}:{1}:{2}", trans.Last().Date, trans.Last().Credit, trans.Last().Debit);
      
      Console.WriteLine("count of records in time span = {0}", trans.Count);
      
      
      // todo -> lol, apparently I can't math... 3 days of credits: 0, 0, 2xxx from a span of 4 days should be 
      // more like 2xxx/4 not 2xxx/3...
      // well at least we're getting some unique list stuff going on...
      var averageCredit = trans.GetAverageDayCredit();
      var averageDebit = trans.GetAverageDayDebit();

      
      Console.WriteLine("average credit {0} and debit {1}", averageCredit, averageDebit);
      
      //
      // foreach (TcfExportData d in trans)
      // {
      //   Console.WriteLine("{0};{1};{2}", d.Date, d.Credit, d.Debit);
      // }
      //
      
      
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
}