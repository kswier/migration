<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/masterpage2.Master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="portmgr.login" %>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
<title>Sign in to your EquityStat investment portfolio account</title>
<meta name="Description" content="Sign in to your EquityStat online investment portfolio management tool."/>
<meta name="keywords" content="sign in, investment portfolio, financial portfolio, investment management, financial management, financial tool, stock, bond, ETF, mutual fund"/>
<script src="/portmgr/scripts/checkreq.js"></script>

<script>
    function checkReq() {
        var rslt = jqueryCheckReq("loginform");
        if ($(".mobilecheck").is(":visible"))
            $("#ismobile").val("1");
        return rslt;
    }

</script>
</asp:Content>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="container-fluid maincontent">
  <div class="borderbox content">
    <h1>Sign-In to your account</h1>
    <form id="loginform" method="POST" action="login.aspx" onsubmit="return checkReq();">
       <div class="form-group row">
           <label class="col-form-label offset-sm-1 col-sm-1" for="regEmail">Email</label>
           <div class="col-sm-4">
             <input type="text" id="regEmail" name="email" tabindex="1" class="form-control form-control-md reqfld firstfld" reqmsg="Please enter an email address"/>
             <span class="form-text" id="emailerror"></span>
           </div>
           <div class="col-sm-6"></div>
       </div>
       <div class="form-group row">
           <label class="col-form-label offset-sm-1 col-sm-1" for="regPassword">Password<br /><a href="/portmgr/pages/requestpwd.aspx">(Forgot)</a></label>
           <div class="col-sm-4">
               <input type="password" name="password" id="regPassword" tabindex="2" class="form-control form-control-md reqfld" reqmsg="Please enter a password"/>
               <span class="form-text" id="passworderror"></span>
           </div>
           <div class="col-sm-6"></div>
       </div>
       <div class="form-group row">
           <div class="offset-sm-2 col-sm-10">
               <button type="submit" class="btn btn-default" tabindex="3">Sign In</button>
           </div>
       </div>
       <input type="hidden" value="0" id="ismobile" name="ismobile" />
    </form>
       <div class="row">
       <div class="offset-sm-1 error" id="loginErrMsgFld" runat="server" visible="false"><%=loginErrMsg%></div>			 
    </div>
    </div>
   
    <div class="mobilecheck"></div>
</div>

</asp:Content>