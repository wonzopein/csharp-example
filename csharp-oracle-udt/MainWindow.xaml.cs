using csharp_oracle_udt.UDT;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csharp_oracle_udt
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        string ConnectionString = "Data Source = (DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=0.0.0.0)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=testdb))); User ID=test; Password=1";

        public MainWindow()
        {
            InitializeComponent();

            BcrData bcrData = new BcrData();
            bcrData.Id = "BCR01";
            bcrData.Height = 250;

            BarcodeArray barcodeArray = new BarcodeArray();
            barcodeArray.Array = new Barcode[3];

            Barcode barcode1 = new Barcode();
            barcode1.Data = "barcode1";
            barcode1.Type = "A";
            barcode1.Length = barcode1.Data.Length;

            Barcode barcode2 = new Barcode();
            barcode2.Data = "barcode2";
            barcode2.Type = "B";
            barcode2.Length = barcode2.Data.Length;

            Barcode barcode3 = new Barcode();
            barcode3.Data = "barcode3";
            barcode3.Type = "C";
            barcode3.Length = barcode3.Data.Length;

            barcodeArray.Array[0] = barcode1;
            barcodeArray.Array[1] = barcode2;
            barcodeArray.Array[2] = barcode3;

            bcrData.Barcodes = barcodeArray;
            bcrData.Count = bcrData.Barcodes.Array.Length;


            OracleUdtTest(bcrData);
        }

        private void OracleUdtTest(BcrData bcrData)
        {
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    connection.Open();
                    //
                    //  oracle package
                    OracleCommand command = new OracleCommand("TEST.PKG_DEV_TEST.UDT_TEST", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;
                    //
                    //  only input param
                    OracleParameter i_bcr_data = command.CreateParameter();
                    i_bcr_data.ParameterName = "i_bcr_data";
                    i_bcr_data.Direction = ParameterDirection.Input;
                    i_bcr_data.OracleDbType = OracleDbType.Object;
                    i_bcr_data.UdtTypeName = BcrData.Name;
                    i_bcr_data.Value = bcrData;
                    //
                    //  input and output param
                    OracleParameter io_bcr_data = command.CreateParameter();
                    io_bcr_data.ParameterName = "io_bcr_data";
                    io_bcr_data.Direction = ParameterDirection.InputOutput;
                    io_bcr_data.OracleDbType = OracleDbType.Object;
                    io_bcr_data.UdtTypeName = BcrData.Name;
                    io_bcr_data.Value = bcrData;
                    //
                    //  only output param
                    OracleParameter o_result = command.CreateParameter();
                    o_result.ParameterName = "o_result";
                    o_result.Size = 1000;
                    o_result.DbType = DbType.String;
                    o_result.Direction = ParameterDirection.Output;
                    o_result.Value = "APPEND";
                    //
                    //  append params
                    command.Parameters.Add(i_bcr_data);
                    command.Parameters.Add(io_bcr_data);
                    command.Parameters.Add(o_result);
                    //
                    //  execute
                    command.ExecuteNonQuery();
                    //
                    //  input and output param result
                    BcrData outBcrData = (BcrData) command.Parameters["io_bcr_data"].Value;
                    //
                    //  only output param result
                    string result = command.Parameters["o_result"].Value.ToString();

                    Console.WriteLine($"outBcrData = {outBcrData.Id}");
                    Console.WriteLine($"result = {result}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
