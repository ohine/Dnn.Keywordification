<%@ Control language="vb" Inherits="OliverHine.Modules.Keywordification.ViewKeywordification" AutoEventWireup="false" Explicit="True" Codebehind="ViewKeywordification.ascx.vb" %>

<asp:Label ID="lblResponse" runat="server" CssClass="ceResponse"/><asp:Image ID="imgAjax" runat="server" ImageUrl="ajaxanim.gif" CssClass="ceHidden" />

<asp:Panel ID="pnl" runat="server">
    <link rel="stylesheet" type="text/css" media="screen" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7/themes/redmond/jquery-ui.css" />

    <table id="jqgrid" runat="server" class="scroll" cellpadding="0" cellspacing="0"></table>
    <div id="jqpager" runat="server" class="scroll"></div>
</asp:Panel>



