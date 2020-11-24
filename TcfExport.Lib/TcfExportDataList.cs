using System;
using System.Collections.Generic;
using System.Linq;

namespace TcfExport.Lib
{
  public class TcfExportDataList : List<TcfExportData>
  {
    public decimal GetAverageDayDebit()
    {
      var groupedByDateData = this.GroupBy(record => record.Date);
      
      var averageExpenditurePerDay = groupedByDateData.Select(grp =>
      {
        var count = grp.Count();
        if (count == 0) return 0;
        return grp.Sum(d => d.Debit) / count;
      }).ToList();

      if (averageExpenditurePerDay.Count == 0) return 0;

      var avg = averageExpenditurePerDay.Sum();
      var count = averageExpenditurePerDay.Count;

      return avg / count;
    }
    
    public decimal GetAverageDayCredit()
    {
      var groupedByDateData = this.GroupBy(record => record.Date);
      
      var expenditurePerDay = groupedByDateData.Select(grp =>
      {
        var count = grp.Count();
        if (count == 0) return 0;
        return grp.Sum(d => d.Credit) / count;
      }).ToList();

      if (expenditurePerDay.Count == 0) return 0;

      var avg = expenditurePerDay.Sum();
      var count = expenditurePerDay.Count;

      return avg / count;
    }

    public IEnumerable<TcfExportData> GetTransactionsInTimeSpan(DateTime dateTime, TimeSpan timeSpan)
    {
      IEnumerable<TcfExportData>
        window = this.Where(item => dateTime <= item.Date && item.Date < dateTime.Add(timeSpan));

      return window;
    }
    
    public TcfExportDataList GetExportDataListInTimeSpan(DateTime dateTime, TimeSpan timeSpan)
    {
      IEnumerable<TcfExportData>
        window = this.Where(item => dateTime <= item.Date && item.Date <= dateTime.Add(timeSpan)).ToList();

      var newList = new TcfExportDataList();
      newList.AddRange(window);
      return newList;
    }
  }
}