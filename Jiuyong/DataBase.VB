Imports EnvDTE
Imports System.Diagnostics
Imports System.Text
Imports System.Text.RegularExpressions

Public Module DataBase


    Sub CreateFunction()
        'DTE.Windows.Item(Constants.vsWindowKindServerExplorer).Activate()
        'Dim s As String = DTE.ActiveWindow.Caption
        'MsgBox(DTE.ActiveWindow.Object.GetItem("数据连接").Name)

        '数据集名称
        Dim dsn As String '= "Exercise"
        '关系试图的位置
        Dim sindex As String = InputBox("选择数据关系编号（从上到下，从0开始）", "久永代码机器人", "4")

        If sindex = "" Then Return '点击取消返回

        Dim index As Integer = CInt(sindex)

        Dim root As UIHierarchy = DTE.Windows.Item(Constants.vsWindowKindServerExplorer).Object
        'Dim root As UIHierarchy = DTE.ActiveWindow.Object
        Dim tree As UIHierarchyItem = root.UIHierarchyItems().Item(2).UIHierarchyItems().Item(1).UIHierarchyItems().Item(1).UIHierarchyItems().Item(index)
        dsn = tree.Name
        Dim fmt As String = "" _
     & " " & vbCrLf _
     & "//Dataset{1} 数据集中 {0} 表的操作方法" & vbCrLf _
     & "		public static int Fill{0}()" & vbCrLf _
     & "		{{" & vbCrLf _
     & "//			Database.EnforceConstraints = false;" & vbCrLf _
     & "			int r =  _engine.da{0}.Fill(Database.{0});" & vbCrLf _
     & "//			Database.EnforceConstraints=true;" & vbCrLf _
     & "			return r;" & vbCrLf _
     & "		}}" & vbCrLf _
     & "		public static Dataset{1}.{0}Row[] Select{0}Rows()" & vbCrLf _
     & "		{{" & vbCrLf _
     & "			return (Dataset{1}.{0}Row[])Database.{0}.Select();" & vbCrLf _
     & "		}}" & vbCrLf _
     & "		public static Dataset{1}.{0}Row[] Select{0}Rows(Dataset{1}.{0}Row[] rs,string filterExpression,string sort)" & vbCrLf _
     & "		{{" & vbCrLf _
     & "			Dataset{1} t = new Dataset{1}();" & vbCrLf _
     & "			t.{0}.BeginLoadData();" & vbCrLf _
     & "			t.Merge(rs);" & vbCrLf _
     & "			return (Dataset{1}.{0}Row[])t.{0}.Select(filterExpression,sort);" & vbCrLf _
     & "		}}" & vbCrLf _
     & "		public static int Save{0}(Dataset{1}.{0}Row[] rs)" & vbCrLf _
     & "		{{" & vbCrLf _
     & "			return _engine.da{0}.Update(rs);" & vbCrLf _
     & "		}}" & vbCrLf _
     & "		public static int Save{0}()" & vbCrLf _
     & "		{{" & vbCrLf _
     & "			return _engine.da{0}.Update(Database.{0});" & vbCrLf _
     & "		}}" & vbCrLf _
     & " " & vbCrLf

        Dim fmtEngine As String = vbCrLf _
        & "		private static {1}Database _engine = new {1}Database();" & vbCrLf _
        & "		public static {1}Database Engine" & vbCrLf _
        & "		{{" & vbCrLf _
        & "			get" & vbCrLf _
        & "			{{" & vbCrLf _
        & "				return _engine;" & vbCrLf _
        & "			}}" & vbCrLf _
        & "		}}" & vbCrLf



        Dim CodeStringBuilder As New System.Text.StringBuilder
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("		[Editor(""Microsoft.VSDesigner.Data.SQL.Design.SqlConnectionStringEditor, Microsoft.VSDesigner, Version=7.0.5000.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"", typeof(System.Drawing.Design.UITypeEditor)), RefreshProperties(RefreshProperties.All), RecommendedAsConfigurable(true),System.Data.DataSysDescription(""SqlConnection_ConnectionString""), DefaultValue("""")]")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("		public string ConnectionString")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("		{{")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("			get")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("			{{")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("				return _connection.ConnectionString;")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("			}}")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("			set")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("			{{")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("				if(_connection.ConnectionString==value)return;")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("				_connection.ConnectionString = value;")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("				//				_engine = new ExerciseDatabase();")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("			}}")
        CodeStringBuilder.Append(vbCrLf)
        CodeStringBuilder.AppendFormat("		}}")
        CodeStringBuilder.Append(vbCrLf)

        Dim fmtDatabase As String = vbCrLf _
        & "		public static Dataset{1} Database" & vbCrLf _
        & "		{{" & vbCrLf _
        & "			get" & vbCrLf _
        & "			{{" & vbCrLf _
        & "				return _engine._database;" & vbCrLf _
        & "			}}" & vbCrLf _
        & "		}}" & vbCrLf _
        '& " " & vbCrLf _
        '& " " & vbCrLf

        Dim ts As TextSelection = DTE.ActiveDocument.Selection
        Dim d As String = Date.Now.ToString()
        Dim count As String = (Microsoft.VisualBasic.VBMath.Rnd() * 10000).ToString("0000")
        Dim ct As String = "数据关系[" & dsn & "]的数据操作方法 时间：(" & d & ",ID:" & count & ") (自动生成代码，所有手动都会在更新时清除)" & vbCrLf
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("#region " & ct)

        sb.Append(CodeStringBuilder.ToString())

        sb.AppendFormat(fmtEngine, "", dsn)

        sb.AppendFormat(fmtDatabase, "", dsn)

        For i As Integer = 1 To tree.UIHierarchyItems.Count
            Dim n As String = tree.UIHierarchyItems.Item(i).Name
            n = n.Split(CChar(" "))(0)
            sb.AppendFormat(fmt, n, dsn)
        Next
        sb.Append("#endregion " & ct)


        DTE.UndoContext.Open("公开数据集操作方法：[" & dsn & "]")
        ts.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstColumn)
        ts.Insert(sb.ToString(), vsInsertFlags.vsInsertFlagsContainNewText)
        DTE.UndoContext.Close()
    End Sub

    Sub TableSearch()
        Dim line, originalCode, vars As String

        If DTE.ActiveDocument Is Nothing Then Return

        Dim txt As TextSelection
        txt = CType(DTE.ActiveDocument.Selection, TextSelection)
        originalCode = txt.Text
        Dim variableName As String
        Dim publicName As String
        Dim privateName As String
        Dim varHasTag As String
        Dim lines() As String
        Dim tableName As String
        tableName = InputBox("表的名称（短名）:").Trim
        If (tableName = "") Then Return '无参数直接返回
        lines = Split(originalCode, vbLf)
        Dim r As Regex
        r = New Regex( _
         "r.(?<varName>\w+);", _
         RegexOptions.IgnoreCase Or _
         RegexOptions.ExplicitCapture)

        Dim propertyProcedures As New System.Text.StringBuilder("			string connectTag = """";")
        propertyProcedures.Append(vbCrLf)
        propertyProcedures.Append("			System.Text.StringBuilder sb = new System.Text.StringBuilder();")
        propertyProcedures.Append(vbCrLf)

        vars = String.Empty
        For Each line In lines
            line = line.Trim
            If Not line = String.Empty Then

                'If line.Length = 1 Then
                '    MsgBox("变量名" & line & "太短，建议更改！")
                '    Exit For
                'End If

                Dim mtch As Match
                mtch = r.Match(line)
                If mtch.Success Then
                    variableName = _
                      mtch.Groups("varName").Value.Trim
                    If vars = String.Empty Then
                        vars = variableName
                    Else
                        vars = vars & "," & variableName
                    End If
                    'varHasTag = _
                    '  mtch.Groups("varHasTag").Value.Trim

                    'variableName = IIf(variableName.StartsWith("_"), variableName.Substring(1), variableName)
                    'variableName = IIf(variableName.StartsWith("m_"), variableName.Substring(2), variableName)
                    '当variableName只有一个字母时，发生不必要的计算，产生错误,所以改为以下运算：
                    If (variableName.StartsWith("_")) Then
                        variableName = variableName.Substring(1)
                    End If
                    If (variableName.StartsWith("m_")) Then
                        variableName = variableName.Substring(2)
                    End If

                    publicName = variableName.Substring(0, 1).ToUpper() & variableName.Substring(1)
                    privateName = variableName.Substring(0, 1).ToLower() & variableName.Substring(1)
                    Dim s As String
                    s = vbCrLf _
                    & "			,bool has{0},string {1}" & vbCrLf _
                    & "			if(has{0})" & vbCrLf _
                    & "			{{" & vbCrLf _
                    & "				sb.Append(connectTag);" & vbCrLf _
                    & "				sb.Append(Database." & tableName & ".{0}Column.ColumnName);" & vbCrLf _
                    & "				sb.Append("" LIKE '*"");" & vbCrLf _
                    & "				sb.Append({1});" & vbCrLf _
                    & "				sb.Append(""*' "");" & vbCrLf _
                    & "				connectTag = "" AND "";" & vbCrLf _
                    & "			}}" & vbCrLf

                    propertyProcedures.AppendFormat(s, publicName, privateName)
                End If
            End If
        Next
        propertyProcedures.AppendFormat("			return (DatasetExercise.{0}Row[])Database.{0}.Select(sb.ToString());", tableName)

        DTE.UndoContext.Open("构建查询代码[" & vars & "]")
        DTE.ExecuteCommand("Edit.CommentSelection")
        txt.Insert(propertyProcedures.ToString, _
        vsInsertFlags.vsInsertFlagsContainNewText Or vsInsertFlags.vsInsertFlagsInsertAtEnd)
        'txt.SmartFormat()
        DTE.UndoContext.Close()

    End Sub
End Module


