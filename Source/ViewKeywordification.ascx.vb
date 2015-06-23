
Imports System.Web.UI
Imports System.Collections.Generic
Imports System.Reflection

Imports DotNetNuke
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities

Namespace Modules.Keywordification

    <ControlMethodClass("WinDoH.ViewKeywordification")> _
    Partial Class ViewKeywordification
        Inherits AjaxPortalModuleBase

#Region "AjaxPortalModuleBase Events"
        '-------------------------------------------------------------------------------------------------------------
        '- This is where you add your custom properties that are to be sent down to the client-side object
        '- By default we are passing the naming container ID (ns) and localized messages (msgs) 
        '- If you add your own properties make sure you update your client code with those properties found in EditKeywordification.ascx.js
        '-      get_ImagePath: function() {return this._imagePath;},
        '-      set_ImagePath: function(value) {this._imagePath = value;},
        '-------------------------------------------------------------------------------------------------------------
        Protected Sub AjaxPortalModuleBase_AddScriptComponentDescriptors(ByVal Descriptor As ScriptComponentDescriptor) Handles MyBase.AddScriptComponentDescriptors
            'IMPORTANT!  Enter Client Namespace + ObjectName as Type
            Descriptor.Type = "WinDoH.ViewKeywordification"

            '---------------------------------------------------------------------------------------------------------
            'Add custom properties here
            Descriptor.AddScriptProperty("PortalId", Me.PortalId) 'String.Format("'{0}'", Me.ModulePath & "images/"))
            '---------------------------------------------------------------------------------------------------------
        End Sub

        '-------------------------------------------------------------------------------------------------------------
        '- This is where your client-side javascript that uses the MS AJAX framework needs to be registered
        '- Adding the reference here ensures that the MS AJAX script is run before our script which uses things like Type.registerNamespace run
        '-------------------------------------------------------------------------------------------------------------
        Protected Sub AjaxPortalModuleBase_AddScriptReferences(ByVal References As List(Of ScriptReference)) Handles MyBase.AddScriptReferences
            References.Add(New System.Web.UI.ScriptReference(Me.ControlPath & "jqgrid/js/i18n/grid.locale-en.js"))
            References.Add(New System.Web.UI.ScriptReference(Me.ControlPath & "jqgrid/js/jquery.jqGrid.min.js"))
            References.Add(New System.Web.UI.ScriptReference(Me.ControlPath & "ViewKeywordification.ascx.js"))
        End Sub

        '-------------------------------------------------------------------------------------------------------------
        '- Add any localized text needed on the client
        '-------------------------------------------------------------------------------------------------------------
        Protected Sub AjaxPortalModuleBase_AddLocalizedMessages(ByVal Messages As Dictionary(Of String, String)) Handles MyBase.AddLocalizedMessages
            Messages("bFetchData.Initial") = Localization.GetString("btnFetchData.Initial", LocalResourceFile)
            Messages("bFetchData.Refresh") = Localization.GetString("btnFetchData.Refresh", LocalResourceFile)
            Messages("Column.TabName") = Localization.GetString("Column.TabName", LocalResourceFile)
            Messages("Column.Title") = Localization.GetString("Column.Title", LocalResourceFile)
            Messages("Column.Keywords") = Localization.GetString("Column.Keywords", LocalResourceFile)
            Messages("Column.Description") = Localization.GetString("Column.Description", LocalResourceFile)
            Messages("Column.Priority") = Localization.GetString("Column.Priority", LocalResourceFile)
            Messages("Grid.Title") = Localization.GetString("Grid.Title", LocalResourceFile)
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                DotNetNuke.Framework.jQuery.RequestRegistration()

            Catch exc As Exception        'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Control Methods"
        '-------------------------------------------------------------------------------------------------------------
        '- Exposing methods to the client code is as simple as adding the ControlMethod attribute
        '- IMPORTANT!   ALWAYS SECURE YOUR CONTROL METHODS IF INTERACTING WITH SENSITIVE DATA
        '-------------------------------------------------------------------------------------------------------------

        <ControlMethod()> _
        Public Function GetPortalTabs(ByVal portalId As Integer) As PagedResult(Of MiniTabInfo)
            Try
                Dim arrTabs As List(Of Entities.Tabs.TabInfo) = Entities.Tabs.TabController.GetPortalTabs(portalId, Me.PortalSettings.AdminTabId, False, String.Empty, True, False, False, True, True)
                Dim arrAdminTabs As List(Of Entities.Tabs.TabInfo) = Entities.Tabs.TabController.GetTabsByParent(Me.PortalSettings.AdminTabId, Me.PortalId)

                For Each objTab As Entities.Tabs.TabInfo In arrAdminTabs
                    arrTabs.Remove(objTab)
                Next
                arrTabs.TrimExcess()

                Dim objResult As New PagedResult(Of MiniTabInfo)
                objResult.rows = New List(Of MiniTabInfo)

                For Each obJTab As Entities.Tabs.TabInfo In arrTabs
                    objResult.rows.Add(New MiniTabInfo(obJTab.TabID, obJTab.SiteMapPriority, obJTab.TabName, obJTab.Title, obJTab.KeyWords, obJTab.Description))
                Next

                Return objResult
            Catch ex As Exception
                LogException(ex)
                Return New PagedResult(Of MiniTabInfo)
            End Try
        End Function

        <ControlMethod()> _
        Public Function SaveGridRow(ByVal tabId As Integer, ByVal name As String, ByVal title As String, ByVal description As String, ByVal keywords As String, ByVal priority As Decimal) As Boolean
            Try

                Dim objTabController As New Entities.Tabs.TabController
                Dim objTabInfo As Entities.Tabs.TabInfo = objTabController.GetTab(tabId, PortalId, True)

                With objTabInfo
                    .TabName = name
                    .Title = title
                    .Description = description
                    .KeyWords = keywords
                    .SiteMapPriority = priority
                End With

                objTabController.UpdateTab(objTabInfo)

                Return True

            Catch ex As Exception
                LogException(ex)
                Return False
            End Try
        End Function
#End Region


    End Class

End Namespace
