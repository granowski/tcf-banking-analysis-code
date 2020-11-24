using System;

namespace TcfExport.Lib
{
  public struct TcfExportRecord
  {
    public DateTime Date { get; set; }
    public string Credit { get; set; }
    public string Debit { get; set; }
  }
}