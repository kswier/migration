<%@ Page Language="C#" MasterPageFile="~/masterpage2.Master" AutoEventWireup="true" CodeBehind="privacy.aspx.cs" Inherits="aspnet.Pages.privacy"%>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
  <title>Privacy Policy</title>
  
  <style>
      .maincontent div.contentdetail {
          padding-top: 0px;
      }

      li {
          color: black;
      }

      .maincontent h4 {
          color:#336799 
      }
  </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="maincontent container-fluid">
   <div class="borderbox content">
    <h1>Privacy Policy</h1>
    <div class="contentdetail">
        <p> Privacy
        </p>
        <h4>1. Your Privacy is not for sale</h4>
        
    </div>
   </div>
  </div>
</asp:Content>