Imports EnvDTE
Imports System.Diagnostics

Imports System.Text
Imports System.Text.RegularExpressions


Public Module AspNet
    Function RemoveFontV7()
        '2004.04.13 v1.0 去除 WebForm 中 Visual Studio 自动生成的麻烦的"<FONT ***></FONT>"标记。
        Dim exp = "(\<FONT[^>]*\>)|(\</FONT\>)"

        Dim td As TextDocument = DTE.ActiveDocument.Object

        DTE.UndoContext.Open("去除源代码中的任何<FONT>标记") '用于vs70,vs7.1当中的Html设计器的自动生成代码，在vs8,vs9中设计器已经去掉此功能，一般无需使用。
        td.ReplacePattern(exp, "", vsFindOptions.vsFindOptionsFromStart + vsFindOptions.vsFindOptionsRegularExpression, )
        DTE.UndoContext.Close()


    End Function


    Public Sub RemoveTag()
        '2004.04.13 v1.0 去除 WebForm 中 Visual Studio 自动生成的麻烦的"<FONT ***></FONT>"标记。
        '2008.05.05 TextDocument.ReplacePattern 定义已与以前的版本不同，调用时会导致服务器异常并且不可热修复。
        Dim v As Integer = CInt(DTE.Version.Substring(0, 1))
        If v < 8 Then
            RemoveFontV7()
            Return
            'Else
            '    MsgBox("VS2005及其以上其他版本还存在问题！")
        End If
        'Return
        '用于vs70,vs7.1当中的Html设计器的自动生成代码，在vs8,vs9中设计器已经去掉此功能，一般在升级旧代码时使用。

        Static Tag As String = String.Empty

        If Tag = String.Empty Then
            Tag = InputBox("请输入编译标记：", DefaultResponse:="FONT")
        Else
            Tag = InputBox("请输入编译标记：", DefaultResponse:=Tag)
        End If

        If Tag <> String.Empty Then

            Dim exp = String.Format("(\<{0}[^>]*\>)|(\</{0}\>)", Tag)
            Dim rgx As Regex = New Regex(exp, RegexOptions.IgnoreCase + RegexOptions.Multiline)
            Dim td As TextSelection = DTE.ActiveDocument.Selection
            DTE.UndoContext.Open("久永宏：去除所选代码中的全部<" + Tag + ">标记")
            'td.SelectAll() '一定要全文匹配吗？
            Dim text As String = td.Text
            Dim text1 As String = rgx.Replace(text, String.Empty)
            td.Insert(text1, vsInsertFlags.vsInsertFlagsContainNewText)
            'td.SelectAll()
            'DTE.ActiveDocument.Selection.SmartFormat()
            'td.SmartFormat() '此句带来：
            '---------------------------
            '        错误()
            '---------------------------
            '试图吊销一个未注册的拖放目标 (异常来自 HRESULT:0x80040100 (DRAGDROP_E_NOTREGISTERED))
            '---------------------------
            '        确定()
            '---------------------------

            'td.ReplacePattern(exp, "", CInt(vsFindOptions.vsFindOptionsRegularExpression))
            DTE.UndoContext.Close()

        End If
    End Sub

End Module

