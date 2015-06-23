Namespace Modules.Keywordification
    Public Class BaseEntity

    End Class

    Public Class MiniTabInfo
        Public Sub New(ByVal iTabId As Integer, ByVal dSiteMapPriority As Single, ByVal sTabName As String, ByVal sTitle As String, ByVal sKeyWords As String, ByVal sDescription As String)
            Me.TabID = iTabId
            Me.SiteMapPriority = dSiteMapPriority
            Me.TabName = sTabName
            Me.Title = sTitle
            Me.KeyWords = sKeyWords
            Me.Description = sDescription
        End Sub

        Public TabID As Integer
        Public SiteMapPriority As Single
        Public TabName As String
        Public Title As String
        Public KeyWords As String
        Public Description As String
    End Class

    Public Class PagedResult(Of T As MiniTabInfo)

        Public Sub New()
            Me.rows = New List(Of T)
        End Sub

        Private _rows As List(Of T)
        Private _page As Integer
        Private _total As Integer
        Private _records As Integer

        Public ReadOnly Property page() As Integer
            Get
                Return 1
            End Get
        End Property
        Public ReadOnly Property total() As Integer
            Get
                Return 1
            End Get
        End Property

        Public ReadOnly Property records() As Integer
            Get
                Return _rows.Count
            End Get
        End Property

        Public Property rows() As List(Of T)
            Get
                Return _rows
            End Get
            Set(ByVal value As List(Of T))
                _rows = value
            End Set
        End Property
    End Class

End Namespace