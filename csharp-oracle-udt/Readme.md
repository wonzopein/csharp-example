# csharp-example

## Oracle UDTs(User-Defied Types) .Net


오라클 프로시저를 호출하는데 기본 데이터 타입 외 Array 형태 또는 특정 Object를 전달하거나 전달 받을 수 있는 기능이 필요했다.
 검색해보니 오라클에서는 UDTs(User-Defined Types)라는 기능을 제공하고 있어 사용방법 및 후기를 남긴다.


프로시저 호출시 한번에 전달하고자 하는 데이터 :
(데이터가 JSON은 아님. 아래는 형태 참고용)
````json
{
    "EQ_ID" : "BCR01",
    "HEIGHT" : 400,
    "COUNT" : 5,
    "BARCODES" : 
        [
            {"Data":"123456", "Type":"CODE128", "Length":6},
            {"Data":"234567", "Type":"CODE93", "Length":6},
            {"Data":"34567", "Type":"CODE93", "Length":5},
            {"Data":"456789", "Type":"CODE49", "Length":6},
            {"Data":"567890", "Type":"CODE128", "Length":6}
        ]
}
````



### 고민했던 방법


1. 데이터를 ROW 형태로 변환 후, 바코드 수 만큼 프로시저를 호출한다.
>- 프로시저 호출이 너무 빈번하게 발생되며, 프로시저에서 모든 데이터를 받은 후 판단해야 하는 경우라면 트랜잭션+퍼포먼스 문제 발생의 여지가 크다고 본다.

2. 구분자를 이용하여 호출하고, 프로시저에서 VARCHAR로 수신 후 Split 등의 거처 진행한다.
>- 단순한 형태의 데이터는 가능하겠지만, 조금만 복잡한 구조가되면 송신/수신 측 모두에게 부담이 될 수 있음. 위 데이터 기준이라면 하고싶지 않을것 같다.

### Oracle UDTs를 활용한 방법
#### 환경
- C#
- WPF
- Oracle 11g, 12c (DLL파일은 github 용량제한으로 제거하고 올림.)
  - oci.dll
  - ociw32.dll
  - Oracle.DataAccess.dll
  - oraociei11.dll
  - OraOps11w.dll


위와 같은 데이터를 프로시저에 효과적으로 전달하고자 몇가지 방법을 고민해봤으나, 이 경우에는 UDTs를 활용하는 방법이 가장 효율적이라고 판단했다.

#### 01. Oracle Type 생성
C# Class와 매핑될 수 있는 오라클 Type을 생성.
````sql
--
--  바코드 데이터
--
CREATE OR REPLACE TYPE TEST.BARCODE AS OBJECT
(
  Data VARCHAR2(20),
  Type VARCHAR2(20),
  Length NUMBER
)
/

--
--  바코드 목록
--
CREATE OR REPLACE TYPE TEST.BARCODEARRAY
AS VARRAY (1000) OF BARCODE
/

--
--  데이터 묶음
--
CREATE OR REPLACE TYPE TEST.BCRDATA AS OBJECT
(
  Id VARCHAR2(20),
  Count NUMBER,
  Height NUMBER,
  Barcodes TEST.BARCODEARRAY
)
/
````

#### 02. C# Class 생성
Oracle Type과 매핑될 수 있는 Class 생성.

Barcode.cs :
````c#
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
public class Barcode : IOracleCustomType, INullable
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
````

BarcodeArray.cs :
````c#
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
````

BcrData.cs :
````c#
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
public class BcrData : IOracleCustomType, INullable
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
````

#### 03. 테스트 프로시저 생성
````sql
CREATE OR REPLACE PACKAGE TEST.PKG_DEV_TEST AS


PROCEDURE UDT_TEST(
    i_bcr_data IN TEST.BCRDATA,
    io_bcr_data IN OUT TEST.BCRDATA,
    o_result    OUT  varchar2
) IS
    v_txt varchar2(1000);
BEGIN
    o_result := o_result || i_bcr_data.Id;
    o_result := o_result || ', ' || i_bcr_data.Height;
    o_result := o_result || ', ' || i_bcr_data.Barcodes.count;
        
    select listagg(Data, ',') within group (order by Data) name  
    into v_txt
    from table(i_bcr_data.Barcodes);
        
    --
    --  IN OUT 파라미터 테스트로 값을 변경해본다.
    io_bcr_data.Id := i_bcr_data.Id || ' from Oracle';
    
    --
    --  OUT 파라미터 테스트
    o_result := o_result || ', ' || v_txt;
END;


END PKG_DEV_TEST;
/
````
#### 04. 테스트
````c#
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
                o_result.Value = "";
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
````
결과 :
````
outBcrData = BCR01 from Oracle
result = BCR01, 250, 3, barcode1,barcode2,barcode3
````

### 사용후기
Oracle Type을 생성하고 이에 매핑될수 있게끔 C# Class(컬럼 매핑 등)를 작성하는것은 매우 번거로운 작업들이었지만,
진행되었던 프로젝트에서 작성된 C# 코드와 프로시저 모두 생각보다 명확한 코드가 작성되었으며 DB부하에도 크게 영향을 미치지 않도록 완료되어 만족한다.

약간 불편한 점으로 UDTs 기능을 사용하기 위해서는 오라클 DLL 5개가 필요한데, Nuget을 통해 받을 수 없으며(파일 복붙해야 한다는 말) DLL파일 5개의 용량이 130MB에 육박한다. 이로인해 어플리케이션 용량이 꽤나 증가된다.

### 참조
- [Oracle, Data Provider for .NET Developers Guide](https://docs.oracle.com/cd/E14435_01/win.111/e10927/featUDTs.htm#CJAGCAID)
