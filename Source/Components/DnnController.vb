Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.Upgrade
Imports DotNetNuke.Entities.Modules

Namespace Modules.Keywordification

    Public Class DnnController
        Implements IUpgradeable

        Public Function UpgradeModule(version As String) As String Implements IUpgradeable.UpgradeModule
            Try
                Select Case version
                    Case "00.02.2375"
                        Dim moduleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Keywordification")
                        If moduleDefinition IsNot Nothing Then
                            'Add Module to Admin Page for all Portals
                            Upgrade.AddAdminPages("Keyword Editor", "Manage the page title, description, and keywords to help review and update seo for the entire site.", String.Empty, String.Empty, True, moduleDefinition.ModuleDefID, "Keyword Editor", String.Empty, True)
                        End If
                        Exit Select
                End Select

                Return "Success"
            Catch exception As Exception
                LogException(exception)

                Return "Failed"
            End Try
        End Function
    End Class

End Namespace

