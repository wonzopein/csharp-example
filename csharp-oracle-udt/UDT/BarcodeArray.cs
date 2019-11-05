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
    [OracleCustomTypeMapping(BarcodeArray.Name)]
    public class BarcodeArrayFactory : IOracleCustomTypeFactory, IOracleArrayTypeFactory
    {
        public IOracleCustomType CreateObject()
        {
            return new BarcodeArray();
        }

        public Array CreateArray(int numElems)
        {
            return new Barcode[numElems];
        }

        public Array CreateStatusArray(int numElems)
        {
            return new OracleUdtStatus[numElems];
        }
    }

    /// <summary>
    /// User type
    /// </summary>
    internal class BarcodeArray : IOracleCustomType, INullable
    {
        public const string Name = "TEST.BARCODEARRAY";

        public bool IsNull { get; set; }

        [OracleArrayMapping()]
        public Barcode[] Array;

        public OracleUdtStatus[] StatusArray { get; set; }

        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
            OracleUdt.SetValue(con, pUdt, 0, Array);
        }

        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            object objectStatusArray = null;
            Array = (Barcode[])OracleUdt.GetValue(con, pUdt, 0, out objectStatusArray);
            StatusArray = (OracleUdtStatus[])objectStatusArray;
        }

        public static BarcodeArray Null => new BarcodeArray { IsNull = true };
    }
}
