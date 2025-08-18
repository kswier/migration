<%@ Page Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="aboutus.aspx.cs" Inherits="aspnet.Pages.aboutus"%>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
  <title>About Us</title>
  
  <style>
      
      li {
          color: black;
      }
  </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
  <div class="container-fluid maincontent">
   <div class="borderbox content">
    <h1>About Us</h1>
    <div class="contentdetail">
        <p> 
           About Us

        </p>
    </div>
   </div>
  </div>
</asp:Content>