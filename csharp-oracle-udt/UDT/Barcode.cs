using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_oracle_udt.UDT
{
    /// <summary>
    /// Factory
    /// </summary>
    [OracleCustomTypeMapping(Barcode.Name)]
    public class BarcodeFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject() => new Barcode();
    }

    /// <summary>
    /// User type
    /// </summary>
    internal class Barcode : IOracleCustomType, INullable
    {
        public const string Name = "TEST.BARCODE";
        public bool IsNull { get; set; }

        [OracleObjectMapping("DATA")]
        public string Data { get; set; }
        [OracleObjectMapping("TYPE")]
        public string Type { get; set; }
        [OracleObjectMapping("LENGTH")]
        public int Length { get; set; }

        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, "DATA", Data);
            OracleUdt.SetValue(con, pUdt, "TYPE", Type);
            OracleUdt.SetValue(con, pUdt, "LENGTH", Length);
        }

        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            Data = (string)OracleUdt.GetValue(con, pUdt, "DATA");
            Type = (string)OracleUdt.GetValue(con, pUdt, "TYPE");
            Length = (int)OracleUdt.GetValue(con, pUdt, "LENGTH");
        }
    }
}
