using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TcfExport.Lib
{
  public class TcfExportRecords : List<TcfExportRecord>
  {
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