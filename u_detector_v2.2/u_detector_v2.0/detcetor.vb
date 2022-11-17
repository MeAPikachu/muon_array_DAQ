Imports System.IO
Imports System.Data.OleDb
Imports System.Threading
Imports ZedGraph
' ZedGraph is the graphical interface

' Form is the basic class type 
Public Class Form1
    Public baud As String '波特率
    Public data As String  '数据位
    Public check As String    '检验位
    Public data_stop As String   '停止位s
    Public data_read(7) As Byte '读取数据的位数 '7 means the default value

    Public tempure As Double '温度
    Public pressure As Double '气压

    Public u_count As Long
    Public u_count_show As Long
    Public u_count_show_temp As Long
    Public u_count_save As Long
    Public u_sum As Long

    Public year As Integer
    Public month As Integer
    Public day As Integer
    Public old_year As Integer
    Public old_month As Integer
    Public old_day As Integer
    Public hour As Integer
    Public minute As Integer
    Public second As Integer
    Public operation_code As String
    Public operation_data(7) As String

    Dim count As Long
    Dim refresh_flag As Boolean  '图片刷新标志
    Dim zed_show_flag As Boolean '图片显示标志
    Dim old_version As Boolean   '判断是哪个版本的
    Dim show_data As Thread '数据显示进程
    Dim list As New PointPairList() 'zed显示
    Dim show_text As Thread '显示数据进程
    Dim save_text As Thread '显示数据进程
    Dim data_text As String

    Public database_name As String
    Public table_name As String
    Public row_name As String
    Public row_value As String
    Public line_name As String
    Public line_value As String
    Public accessconnectstring As String = "Provider=Microsoft.Jet.OLEDB.4.0;" &
"Data Source=" & database_name & ".mdb;" &
"Persist Security Info=False" & ";jet oledb:database Password ='1234'"

    ' Sub means the functions of the class 
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'zedgraph的初始化
        ZedGraphControl1.GraphPane.Title.Text = "Real-time cosmic ray muon monitoring"
        ZedGraphControl1.GraphPane.Title.FontSpec.Family = "Times New Roman"
        ZedGraphControl1.GraphPane.Title.FontSpec.IsBold = True
        Dim list1 As LineItem = ZedGraphControl1.GraphPane.AddCurve("muon", list, Color.Red, SymbolType.Diamond) '图例
        ZedGraphControl1.GraphPane.XAxis.Title.Text = "Time" '设置X轴名称
        ZedGraphControl1.GraphPane.XAxis.Title.FontSpec.Family = "Times New Roman"
        ZedGraphControl1.GraphPane.XAxis.Title.FontSpec.IsBold = True
        ZedGraphControl1.GraphPane.YAxis.Title.Text = "Muon Flux" '设置Y轴名称
        ZedGraphControl1.GraphPane.YAxis.Title.FontSpec.Family = "Times New Roman"
        ZedGraphControl1.GraphPane.YAxis.Title.FontSpec.IsBold = True
        ZedGraphControl1.GraphPane.YAxis.Scale.MinorUnit = 10
        ZedGraphControl1.GraphPane.YAxis.Scale.Min = 0 'y轴刻度最小值
        ZedGraphControl1.GraphPane.XAxis.Type = AxisType.Date
        ZedGraphControl1.GraphPane.XAxis.Scale.Format = "hh:mm:ss"
        ZedGraphControl1.GraphPane.XAxis.Scale.MinorStep = 1
        'mscomm初始化
        mscomm.InBufferSize = 8192
        mscomm.InBufferCount = 0
        mscomm.OutBufferSize = 1024
        mscomm.OutBufferCount = 0
        mscomm.RThreshold = 7
        mscomm.InputMode = MSCommLib.InputModeConstants.comInputModeBinary
        mscomm.CommPort = 2
        mscomm.Settings = "115200,N,8,1"
        '********************************************************
        ComboBox1.SelectedIndex = 1
        ComboBox2.SelectedIndex = 12
        ComboBox3.SelectedIndex = 0
        ComboBox4.SelectedIndex = 0
        ComboBox5.SelectedIndex = 0
        ComboBox6.SelectedIndex = 0
        ComboBox7.SelectedIndex = 3
        ComboBox8.SelectedIndex = 0
        '*******************************************************
        ComboBox9.SelectedIndex = 0
        ComboBox10.SelectedIndex = 0
        ComboBox11.SelectedIndex = 0
        ComboBox12.SelectedIndex = 0
        ComboBox13.SelectedIndex = 3
        ComboBox14.SelectedIndex = 19
        ComboBox15.SelectedIndex = 1
        Button9.Enabled = Enabled
        Button10.Enabled = Enabled
        '********************************************************
        refresh_flag = False '打开界面禁止刷新图像
        '*******************************************************
        old_version = False '新版本
        count = 0
        u_count = 0
        u_count_save = 0
        u_count_show = 0
        u_sum = 0
        old_year = Now.Year
        old_month = Now.Month
        old_day = Now.Day
        year = Now.Year
        month = Now.Month
        day = Now.Day
        hour = Now.Hour
        minute = Now.Minute
        second = Now.Second
        Dim x As Double = New XDate(year, month, day, hour, minute, second)
        list.Add(x, 0)
        ZedGraphControl1.AxisChange()
        Refresh()
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        TextBox3.Text = "Current Time：" & Now
        year = Now.Year
        month = Now.Month
        day = Now.Day
        hour = Now.Hour
        Minute = Now.Minute
        second = Now.Second
    End Sub

    'ComboBox 1 : Port Number 
    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        On Error GoTo back
        If mscomm.PortOpen = True Then
            mscomm.PortOpen = False
            If ComboBox1.Text = "COM1" Then
                mscomm.CommPort = 1
            End If
            If ComboBox1.Text = "COM2" Then
                mscomm.CommPort = 2
            End If
            If ComboBox1.Text = "COM3" Then
                mscomm.CommPort = 3
            End If
            If ComboBox1.Text = "COM4" Then
                mscomm.CommPort = 4
            End If
            If ComboBox1.Text = "COM5" Then
                mscomm.CommPort = 5
            End If
            If ComboBox1.Text = "COM6" Then
                mscomm.CommPort = 6
            End If
            If ComboBox1.Text = "COM7" Then
                mscomm.CommPort = 7
            End If
            If ComboBox1.Text = "COM8" Then
                mscomm.CommPort = 8
            End If
            If ComboBox1.Text = "COM9" Then
                mscomm.CommPort = 9
            End If
            If ComboBox1.Text = "COM10" Then
                mscomm.CommPort = 10
            End If
            If ComboBox1.Text = "COM11" Then
                mscomm.CommPort = 11
            End If
            If ComboBox1.Text = "COM12" Then
                mscomm.CommPort = 12
            End If
            If ComboBox1.Text = "COM13" Then
                mscomm.CommPort = 13
            End If
            If ComboBox1.Text = "COM14" Then
                mscomm.CommPort = 14
            End If
            If ComboBox1.Text = "COM15" Then
                mscomm.CommPort = 15
            End If
            If ComboBox1.Text = "COM16" Then
                mscomm.CommPort = 16
            End If
            If ComboBox1.Text = "COM17" Then
                mscomm.CommPort = 17
            End If
            If ComboBox1.Text = "COM18" Then
                mscomm.CommPort = 18
            End If
            If ComboBox1.Text = "COM19" Then
                mscomm.CommPort = 19
            End If
            mscomm.PortOpen = True
        Else
            If ComboBox1.Text = "COM1" Then
                mscomm.CommPort = 1
            End If
            If ComboBox1.Text = "COM2" Then
                mscomm.CommPort = 2
            End If
            If ComboBox1.Text = "COM3" Then
                mscomm.CommPort = 3
            End If
            If ComboBox1.Text = "COM4" Then
                mscomm.CommPort = 4
            End If
            If ComboBox1.Text = "COM5" Then
                mscomm.CommPort = 5
            End If
            If ComboBox1.Text = "COM6" Then
                mscomm.CommPort = 6
            End If
            If ComboBox1.Text = "COM7" Then
                mscomm.CommPort = 7
            End If
            If ComboBox1.Text = "COM8" Then
                mscomm.CommPort = 8
            End If
            If ComboBox1.Text = "COM9" Then
                mscomm.CommPort = 9
            End If
            If ComboBox1.Text = "COM10" Then
                mscomm.CommPort = 10
            End If
            If ComboBox1.Text = "COM11" Then
                mscomm.CommPort = 11
            End If
            If ComboBox1.Text = "COM12" Then
                mscomm.CommPort = 12
            End If
            If ComboBox1.Text = "COM13" Then
                mscomm.CommPort = 13
            End If
            If ComboBox1.Text = "COM14" Then
                mscomm.CommPort = 14
            End If
            If ComboBox1.Text = "COM15" Then
                mscomm.CommPort = 15
            End If
            If ComboBox1.Text = "COM16" Then
                mscomm.CommPort = 16
            End If
            If ComboBox1.Text = "COM17" Then
                mscomm.CommPort = 17
            End If
            If ComboBox1.Text = "COM18" Then
                mscomm.CommPort = 18
            End If
            If ComboBox1.Text = "COM19" Then
                mscomm.CommPort = 19
            End If
        End If

        Exit Sub
back:
        MsgBox("The serial port does not exist or is occupied", vbOKOnly, "Notice")
    End Sub

    ' ComboBox 2 baud rate 
    Private Sub ComboBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox2.SelectedIndexChanged
        Select Case ComboBox2.Text
            Case 110
                baud = 110
            Case 300
                baud = 300
            Case 600
                baud = 600
            Case 1200
                baud = 1200
            Case 2400
                baud = 2400
            Case 4800
                baud = 4800
            Case 9600
                baud = 9600
            Case 14400
                baud = 14400
            Case 19200
                baud = 19200
            Case 38400
                baud = 38400
            Case 56000
                baud = 56000
            Case 57600
                baud = 57600
            Case 115200
                baud = 115200
            Case 128000
                baud = 128000
            Case 256000
                baud = 256000
            Case Else
                baud = 115200
        End Select
    End Sub

    'ComboBox 5 : Time interval 
    Private Sub ComboBox5_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox5.SelectedIndexChanged
        Select Case ComboBox5.SelectedIndex
            Case 0
                Timer2.Interval = 1000
            Case 1
                Timer2.Interval = 2000
            Case 2
                Timer2.Interval = 5000
            Case 3
                Timer2.Interval = 10000
            Case 4
                Timer2.Interval = 20000
            Case 5
                Timer2.Interval = 30000
            Case 6
                Timer2.Interval = 60000
            Case 7
                Timer2.Interval = 300000

            Case 8
                Timer2.Interval = 600000
            Case 9
                Timer2.Interval = 1200000
            Case 10
                Timer2.Interval = 1800000
            Case 11
                Timer2.Interval = 3600000

        End Select

    End Sub

    'ComboBox 6 
    Private Sub ComboBox6_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox6.SelectedIndexChanged
        Select Case ComboBox6.SelectedIndex
            Case 0
                Timer3.Interval = 3600000
            Case 1
                Timer3.Interval = 20000
        End Select
    End Sub

    'ComboxBox 7 data length 
    Private Sub ComboBox7_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox7.SelectedIndexChanged
        Select Case ComboBox7.Text
            Case 5
                data = 5
            Case 6
                data = 6
            Case 7
                data = 7
            Case 8
                data = 8
            Case Else
                data = 8
        End Select
    End Sub

    'ComboxBox 8: RTreshold
    Private Sub ComboBox8_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox8.SelectedIndexChanged
        Select Case ComboBox8.SelectedIndex
            Case 0
                mscomm.RThreshold = 7
                old_version = False

            Case 1
                mscomm.RThreshold = 2
                old_version = True

        End Select
    End Sub

    'Button 1 : Open port 
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        On Error GoTo back
        Timer2.Enabled = True
        Timer3.Enabled = True
        refresh_flag = True
        If Button1.Tag = 0 Then
            mscomm.Settings = CStr(baud) + ",N," + CStr(data) + "," + CStr(data_stop) '设置串口参数
            mscomm.PortOpen = True '打开串口
            If mscomm.PortOpen = True Then
                Label10.Text = mscomm.Settings
                Button1.Tag = 1
                Button2.Tag = 0
            End If
            mscomm.InBufferCount = 0
        End If
        If Button1.Tag = 1 Then
            Button1.Text = "Refresh "
            mscomm.Settings = CStr(baud) + ",N," + CStr(data) + "," + CStr(data_stop)
            Label10.Text = mscomm.Settings
            Button2.Tag = 0
            mscomm.InBufferCount = 0
        End If
        If old_version = False Then
            Button9.Enabled = True
            Button10.Enabled = True
        End If
        Exit Sub
back:
        Label10.Text = "The port was not found"
        MsgBox("Serial port opening failed", vbOKOnly, "Notice")
    End Sub

    'Button 2 : Close serial 
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Button2.Tag = 0 Then
            If mscomm.PortOpen = True Then
                mscomm.PortOpen = False  '关闭串口
                Timer2.Enabled = False
                Timer3.Enabled = False
            End If
            If mscomm.PortOpen = False Then
                Label10.Text = "Serial port closed successfully"
                Button1.Text = "Open"
                Button1.Tag = 0
                Button2.Tag = 1
            End If
        Else : Label10.Text = "Serial port is closed"
        End If
        refresh_flag = False
    End Sub

    'Main loop
    Private Sub mscomm_OnComm(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mscomm.OnComm
        Dim byte_data As Object
        Dim i As Object
        If old_version = False Then
            Select Case mscomm.CommEvent
                Case MSCommLib.OnCommConstants.comEvReceive
                    byte_data = mscomm.Input
                    ReDim data_read(UBound(byte_data))
                    For i = 0 To UBound(byte_data)
                        data_read(i) = byte_data(i)
                    Next i
                    If data_read(0) = 0 Then 'u子数据
                        count = count + 1
                        u_count = 3
                        u_sum = u_sum + 3
                        If count <= 300 Then
                            show_text = New Thread(AddressOf text_show)
                            show_text.Start()
                        ElseIf count = 301 Then
                            data_text = TextBox4.Text
                            save_text = New Thread(AddressOf text_save)
                            save_text.Start()
                            TextBox4.Text = CStr(data_read(1)) + "  " + CStr(data_read(2)) + "  " + CStr(data_read(3)) + "  " + CStr(data_read(4)) + "  " + CStr(data_read(5)) + "  " + CStr(data_read(6)) + "  "
                            count = 0
                        End If
                        data_deal(u_count)
                    ElseIf data_read(0) <> 0 Then
                        If data_read(1) = 0 Then
                            tempure = data_read(2) + data_read(3) * 0.1
                        ElseIf data_read(1) <> 0 Then
                            tempure = (data_read(2) + data_read(3) * 0.1) * (-1)
                        End If
                        pressure = data_read(4) * 2 ^ 8 + data_read(5) + data_read(6) * 0.1
                    End If
                    TextBox1.Text = u_sum
                Case MSCommLib.OnCommConstants.comEvCD
                Case MSCommLib.OnCommConstants.comEvCTS
                Case MSCommLib.OnCommConstants.comEvDSR
                Case MSCommLib.OnCommConstants.comEvSend
            End Select

        Else
            Select Case mscomm.CommEvent
                Case MSCommLib.OnCommConstants.comEvReceive
                    byte_data = mscomm.Input
                    ReDim data_read(UBound(byte_data))
                    For i = 0 To UBound(byte_data)
                        data_read(i) = byte_data(i)
                    Next i
                    count = count + 1
                    u_count = 1
                    u_sum = u_sum + 1
                    If count <= 1000 Then
                        show_text = New Thread(AddressOf text_show)
                        show_text.Start()
                    ElseIf count = 1001 Then
                        data_text = TextBox4.Text
                        save_text = New Thread(AddressOf text_save)
                        save_text.Start()
                        TextBox4.Text = CStr(data_read(0)) + "  " + CStr(data_read(1)) + "  "
                        count = 0
                    End If
                    data_deal(u_count)
                    TextBox1.Text = u_sum
                Case MSCommLib.OnCommConstants.comEvCD
                Case MSCommLib.OnCommConstants.comEvCTS
                Case MSCommLib.OnCommConstants.comEvDSR
                Case MSCommLib.OnCommConstants.comEvSend
            End Select

        End If

    End Sub

    'Button 4 :  Refresh 
    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If refresh_flag = True Then
            refresh_flag = False
            Button4.Text = "Refresh"
        Else
            refresh_flag = True
            Button4.Text = "Stop"
        End If
    End Sub

    'Timer2
    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        u_count_show_temp = u_count_show
        u_count_show = 0
        zed_show_flag = True
        If zed_show_flag = True Then
            zed_show_flag = False
            CheckForIllegalCrossThreadCalls = False
            show_data = New Thread(AddressOf zed_show)
            show_data.Start()

        End If
    End Sub

    Private Sub zed_show()
        Dim x As Double = New XDate(year, month, day, hour, minute, second)
        '************************************************************************
        System.IO.File.AppendAllText(TextBox2.Text & "\message.log", vbCrLf)
        System.IO.File.AppendAllText(TextBox2.Text & "\message.log", Now & "      " & year & month & day & " " & hour & ":" & minute & ":" & second)
        '************************************************************************
        If refresh_flag = True Then
            list.Add(x, u_count_show_temp)
        End If
        ZedGraphControl1.AxisChange()
        Me.Refresh()
        show_data.Abort()
    End Sub
    Private Sub text_show()
        If old_version = False Then
            TextBox4.Text = TextBox4.Text + CStr(data_read(1)) + "  " + CStr(data_read(2)) + "  " + CStr(data_read(3)) + "  " + CStr(data_read(4)) + "  " + CStr(data_read(5)) + "  " + CStr(data_read(6)) + "  "
        Else
            TextBox4.Text = TextBox4.Text + CStr(data_read(0)) + "  " + CStr(data_read(1)) + "  "
        End If

        show_text.Abort()
    End Sub
    Private Sub text_save()
        System.IO.File.AppendAllText(TextBox2.Text & "\" & year & "_" & month & "_" & day & "_" & hour & ".log", data_text)
        save_text.Abort()
    End Sub
    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            TextBox4.Text = ""
            u_count = 0
            u_sum = 0
            count = 0
            u_count_show = 0
            TextBox1.Text = "0"
            list.Clear()
            ZedGraphControl1.AxisChange()
            Refresh()
        Catch AccessException As Exception
            MsgBox(AccessException.Message)
        End Try
    End Sub
    Private Sub data_deal(ByVal u_count As Integer)
        u_count_save = u_count_save + u_count
        u_count_show = u_count_show + u_count
    End Sub


    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        If File.Exists(TextBox2.Text) = False Then
            System.IO.Directory.CreateDirectory(TextBox2.Text)
        End If
        TextBox2.ReadOnly = True
        initial_database()
    End Sub
    '****************创建数据库  一个月一个文件
    Sub create_database(ByVal name) '参数文件名  ××年××月 
        Dim Accesscat As ADOX.Catalog
        Accesscat = New ADOX.Catalog
        If File.Exists(name & ".mdb") Then Exit Sub 'File.Delete(data_base_name & ".mdb") 
        Accesscat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & name & ".mdb")
        Accesscat = Nothing
    End Sub
    '*****************************************
    '******************************************创建表
    Sub create_table(ByVal database_name_c)
        Try
            accessconnectstring = "Provider=Microsoft.Jet.OLEDB.4.0;" &
                                   "Data Source=" & database_name_c & ".mdb;" &
                                   "Persist Security Info=False" & ";jet oledb:database Password ='1234'"
            Dim AccessConn As New OleDb.OleDbConnection(accessconnectstring)
            '建立链接
            AccessConn.Open()
            Dim def_table As String = "(日期 string NOT NULL PRIMARY KEY)"
            Dim table_name_c As String = "u子数量"
            Dim AccessString As String = "CREATE TABLE " & table_name_c & def_table '
            Dim AccessCmd As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
            AccessCmd.ExecuteNonQuery()

            table_name_c = "气温"
            AccessString = "CREATE TABLE " & table_name_c & def_table
            Dim AccessCmd2 As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
            AccessCmd2.ExecuteNonQuery()

            table_name_c = "气压"
            AccessString = "CREATE TABLE " & table_name_c & def_table
            Dim AccessCmd3 As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
            AccessCmd3.ExecuteNonQuery()
            AccessConn.Close()
        Catch ex As Exception

        End Try
    End Sub
    '*******************************************  添加列0～23时 +总数
    Sub alter_table(ByVal database_name_c)
        accessconnectstring = "Provider=Microsoft.Jet.OLEDB.4.0;" &
                                  "Data Source=" & database_name_c & ".mdb;" &
                                  "Persist Security Info=False" & ";jet oledb:database Password ='1234'"
        Dim AccessConn As New OleDb.OleDbConnection(accessconnectstring)
        '建立链接
        AccessConn.Open()
        Dim cnt As Integer
        For cnt = 0 To 23 Step 1
            Try
                Dim AccessString As String = "ALTER TABLE " & "u子数量" & " ADD " & Str(cnt) & "时 integer NULL"
                Dim AccessCmd As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
                AccessCmd.ExecuteNonQuery()

            Catch ex As Exception

            End Try

        Next
        Try
            Dim AccessString1 As String = "ALTER TABLE " & "u子数量" & " ADD " & "总数 integer NULL"
            Dim AccessCmd1 As OleDbCommand = New OleDbCommand(AccessString1, AccessConn)
            AccessCmd1.ExecuteNonQuery()
        Catch ex As Exception

        End Try
        Try
            For cnt = 0 To 23 Step 1
                Dim AccessString As String = "ALTER TABLE " & "气温" & " ADD " & Str(cnt) & "时 integer NULL"
                Dim AccessCmd As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
                AccessCmd.ExecuteNonQuery()

            Next
        Catch ex As Exception

        End Try

        Try
            For cnt = 0 To 23 Step 1
                Dim AccessString As String = "ALTER TABLE " & "气压" & " ADD " & Str(cnt) & "时 integer NULL"
                Dim AccessCmd As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
                AccessCmd.ExecuteNonQuery()
            Next
        Catch ex As Exception

        End Try

        Try
            AccessConn.Close()
        Catch ex As Exception

        End Try

    End Sub
    '*******************************************添加行
    Sub create_row(ByVal database_name_c, ByVal table_name_c, ByVal row_name_c, ByVal row_value_c)
        Try
            accessconnectstring = "Provider=Microsoft.Jet.OLEDB.4.0;" &
                "Data Source=" & database_name_c & ".mdb;" &
                "Persist Security Info=False" & ";jet oledb:database Password ='1234'"
            Dim AccessConn As New OleDb.OleDbConnection(accessconnectstring)
            '建立链接
            AccessConn.Open() '打开数据库
            Dim AccessString As String = "INSERT INTO " & table_name_c & "(" & row_name_c & ")" & "VALUES(" & row_value_c & ")"
            Dim AccessCmd As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
            '转为命令
            AccessCmd.ExecuteNonQuery()
        Catch ex As Exception

        End Try

    End Sub
    '**********************************************
    Sub change_value(ByVal database_name_c, ByVal table_name_c, ByVal row_name_c, ByVal line_name_c, ByVal line_value_c)
        Try
            accessconnectstring = "Provider=Microsoft.Jet.OLEDB.4.0;" &
  "Data Source=" & database_name_c & ".mdb;" &
  "Persist Security Info=False" & ";jet oledb:database Password ='1234'"
            Dim AccessConn As New OleDb.OleDbConnection(accessconnectstring)
            '建立链接
            AccessConn.Open()
            '打开数据库
            Dim AccessString As String = "UPDATE " & table_name_c & " SET " & line_name_c & "= " & line_value_c & " WHERE 日期 = " & row_name_c
            '定义命令Str
            Dim AccessCmd As OleDbCommand = New OleDbCommand(AccessString, AccessConn)
            '转为命令
            AccessCmd.ExecuteNonQuery()
            '无返回值的执行
            AccessConn.Close()
            '关闭数据库
        Catch ex As Exception

        End Try
    End Sub
    '××××××××××××××××××××××
    Sub initial_database()
        database_name = TextBox2.Text & "\" & CStr(year) & "年" & CStr(month) & "月"
        row_name = "日期"
        row_value = "'" + CStr(year) + "_" + CStr(month) + "_" + CStr(day) + "'"
        create_database(database_name)
        Try
            Dim AccessConnectionString As String = "Provider=Microsoft.Jet.OLEDB.4.0;" &
                       "Data Source=" & database_name & ".MDB;" &
                       "Persist Security Info=False" & ";jet oledb:database Password ='1234'"
            Dim AccessConn As New OleDb.OleDbConnection(AccessConnectionString)
            Dim dr As OleDbDataReader
            Dim con As New OleDbConnection
            Dim cmd As New OleDbCommand
            con.ConnectionString = AccessConnectionString
            con.Open()
            cmd.Connection = con
            cmd.CommandText = "SELECT 总数 FROM " & "u子数量" & " where 日期= 1"
            dr = cmd.ExecuteReader()
        Catch ex As Exception
            create_table(database_name)
            alter_table(database_name)
            table_name = "u子数量"
            create_row(database_name, table_name, row_name, row_value)
            table_name = "气压"
            create_row(database_name, table_name, row_name, row_value)
            table_name = "气温"
            create_row(database_name, table_name, row_name, row_value)
            table_name = "u子数量"
            line_name = CStr(hour) & "时"
            line_value = "23"
            change_value(database_name, table_name, row_name, line_name, line_value)
        End Try
    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        Dim u_count_save_temp As Long
        u_count_save_temp = u_count_save
        u_count_save = 0
        database_name = TextBox2.Text & "\" & CStr(year) & "年" & CStr(month) & "月"
        row_name = "'" + CStr(year) + "_" + CStr(month) + "_" + CStr(day) + "'" '"日期"
        row_value = "'" + CStr(year) + "_" + CStr(month) + "_" + CStr(day) + "'"
        line_name = CStr(hour) & "时"


        If old_month = month Then
            If old_day = day Then
                table_name = "u子数量"
                line_value = u_count_save_temp
                change_value(database_name, table_name, row_name, line_name, line_value)
                table_name = "气压"
                line_value = pressure
                change_value(database_name, table_name, row_name, line_name, line_value)
                table_name = "气温"
                line_value = tempure
                change_value(database_name, table_name, row_name, line_name, line_value)
                '************************************************************************
                System.IO.File.AppendAllText(TextBox2.Text & "\message.log", vbCrLf)
                System.IO.File.AppendAllText(TextBox2.Text & "\message.log", Now & "old_day=day")
                '************************************************************************
            Else
                old_day = day
                row_name = "日期"
                table_name = "u子数量"
                create_row(database_name, table_name, row_name, row_value)
                table_name = "气压"
                create_row(database_name, table_name, row_name, row_value)
                table_name = "气温"
                create_row(database_name, table_name, row_name, row_value)

                row_name = "'" + CStr(year) + "_" + CStr(month) + "_" + CStr(day) + "'"
                table_name = "u子数量"
                line_value = u_count_save_temp
                change_value(database_name, table_name, row_name, line_name, line_value)
                table_name = "气压"
                line_value = pressure
                change_value(database_name, table_name, row_name, line_name, line_value)
                table_name = "气温"
                line_value = tempure
                change_value(database_name, table_name, row_name, line_name, line_value)

                '************************************************************************
                System.IO.File.AppendAllText(TextBox2.Text & "\message.log", vbCrLf)
                System.IO.File.AppendAllText(TextBox2.Text & "\message.log", Now & "old_day!=day")
                '************************************************************************

            End If
        Else
            old_month = month
            create_table(database_name)
            alter_table(database_name)
            row_name = "日期"
            table_name = "u子数量"
            create_row(database_name, table_name, row_name, row_value)
            table_name = "气压"
            create_row(database_name, table_name, row_name, row_value)
            table_name = "气温"
            create_row(database_name, table_name, row_name, row_value)
            table_name = "u子数量"

            row_name = "'" + CStr(year) + "_" + CStr(month) + "_" + CStr(day) + "'"
            line_value = u_count_save_temp
            change_value(database_name, table_name, row_name, line_name, line_value)
            table_name = "气压"
            line_value = pressure
            change_value(database_name, table_name, row_name, line_name, line_value)
            table_name = "气温"
            line_value = tempure
            change_value(database_name, table_name, row_name, line_name, line_value)
            '************************************************************************
            System.IO.File.AppendAllText(TextBox2.Text & "\message.log", vbCrLf)
            System.IO.File.AppendAllText(TextBox2.Text & "\message.log", Now & "old_month!=month")
            '************************************************************************
        End If

    End Sub

    Private Sub ComboBox9_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox9.SelectedIndexChanged
        operation_data(0) = "0" & CStr(ComboBox9.SelectedIndex)
    End Sub

    Private Sub ComboBox13_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox13.SelectedIndexChanged
        operation_data(1) = "0" & CStr(ComboBox13.SelectedIndex + 1)
    End Sub

    Private Sub ComboBox14_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox14.SelectedIndexChanged
        If 10 + ComboBox14.SelectedIndex < 16 Then
            operation_data(2) = "0" & CStr(Hex(10 + ComboBox14.SelectedIndex))
        Else
            operation_data(2) = CStr(Hex(10 + ComboBox14.SelectedIndex))
        End If

    End Sub

    Private Sub ComboBox15_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox15.SelectedIndexChanged
        operation_data(3) = "0" & CStr(ComboBox15.SelectedIndex)
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        operation_data(4) = "0" & CStr(ComboBox10.SelectedIndex * 1 + ComboBox11.SelectedIndex * 2 + ComboBox12.SelectedIndex * 4)
        Dim sj As String
        Dim bytHex() As Byte
        sj = "80" & operation_data(0) & operation_data(1) & operation_data(2) & operation_data(3) & "c0" & operation_data(4)
        ReDim bytHex((Len(sj) \ 2) - 1)
        Dim i As Integer
        For i = 1 To Len(sj) Step 2
            bytHex((i - 1) / 2) = Val("&H" & Mid(sj, i, 2))
        Next
        mscomm.Output = bytHex
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Dim sj As String
        Dim bytHex() As Byte
        sj = "E0"
        ReDim bytHex((Len(sj) \ 2) - 1)
        Dim i As Integer
        For i = 1 To Len(sj) Step 2
            bytHex((i - 1) / 2) = Val("&H" & Mid(sj, i, 2))
        Next
        mscomm.Output = bytHex
        Label19.Text = "Current temperature: " & tempure & vbCrLf & "Current pressure: " & pressure
    End Sub
End Class
