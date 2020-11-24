using System.Collections.Generic;
using System.Linq;

namespace TcfExport.Lib
{
  public class TcfExportDataList : List<TcfExportData>
  {
    public decimal GetAverageDayDebit()
    {
      var groupedByDateData = this.GroupBy(record => record.Date);
      
      var expenditurePerDay = groupedByDateData.Select(grp =>
      {
        return grp.Sum(d => d.Debit);
      }).ToList();

      var avg = expenditurePerDay.Sum();
      var count = expenditurePerDay.Count;

      return avg / count;
    }
    
    public decimal GetAverageDayCredit()
    {
      var groupedByDateData = this.GroupBy(record => record.Date);
      
      var expenditurePerDay = groupedByDateData.Select(grp =>
      {
        return grp.Sum(d => d.Credit);
      }).ToList();

      var avg = expenditurePerDay.Sum();
      var count = expenditurePerDay.Count;

      return avg / count;
    }
  }
}