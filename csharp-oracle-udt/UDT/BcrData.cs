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
    [OracleCustomTypeMapping(BcrData.Name)]
    public class BcrDataFactory : IOracleCustomTypeFactory
    {
        public IOracleCustomType CreateObject() => new BcrData();
    }

    /// <summary>
    /// User type
    /// </summary>
    internal class BcrData : IOracleCustomType, INullable
    {
        public const string Name = "TEST.BCRDATA";
        public bool IsNull { get; set; }

        [OracleObjectMapping("ID")]
        public string Id { get; set; }
        [OracleObjectMapping("COUNT")]
        public int Count { get; set; }
        [OracleObjectMapping("HEIGHT")]
        public int Height { get; set; }

        [OracleObjectMapping("BARCODES")]
        public BarcodeArray Barcodes { get; set; }

        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, "ID", Id);
            OracleUdt.SetValue(con, pUdt, "COUNT", Count);
            OracleUdt.SetValue(con, pUdt, "HEIGHT", Height);
            OracleUdt.SetValue(con, pUdt, "BARCODES", Barcodes);
        }

        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            Id = (string)OracleUdt.GetValue(con, pUdt, "ID");
            Count = (int)OracleUdt.GetValue(con, pUdt, "COUNT");
            Height = (int)OracleUdt.GetValue(con, pUdt, "HEIGHT");
            Barcodes = (BarcodeArray)OracleUdt.GetValue(con, pUdt, "BARCODES");
        }
    }
}
