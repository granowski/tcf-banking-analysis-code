using System;

namespace TcfExport.Lib
{
  public struct TcfExportData
  {
    public DateTime Date { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }      
  }
}