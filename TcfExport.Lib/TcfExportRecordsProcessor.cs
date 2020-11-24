using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace TcfExport.Lib
{
  public class TcfExportRecordsProcessor : List<TcfExportRecord>
  {
    ILogger<TcfExportRecordsProcessor> _logger;

    public TcfExportRecordsProcessor(ILogger<TcfExportRecordsProcessor> logger)
    {
      _logger = logger;
    }
    
    public TcfExportDataList GetDataList()
    {
      TcfExportDataList list = new TcfExportDataList();

      var records = this.Select(r =>
      {
        decimal
          creditDecimal = 0,
          debitDecimal = 0;

        // todo -> inject logger, log warnings of failed parsing
        bool successfulCreditParse = decimal.TryParse(r.Credit, out creditDecimal);
        bool successfulDebitParse = decimal.TryParse(r.Debit, out debitDecimal);

        if (!successfulCreditParse)
          _logger.LogWarning("found value credit value '{0}' that could not be parsed to a decimal; defaulting to 0", r.Credit);
        
        if (!successfulDebitParse)
          _logger.LogWarning("found value debit value '{0}' that could not be parsed to a decimal; defaulting to 0", r.Debit);
        
        return new TcfExportData
        {
          Credit = creditDecimal,
          Debit = debitDecimal,
          Date = r.Date
        };
      });

      list.AddRange(records);
      
      return list;
    }
  }
}