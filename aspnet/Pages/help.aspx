<%@ Page Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="help.aspx.cs" Inherits="aspnet.Pages.help"%>
<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
<title>Help</title>


<script>
    
    function checkReq() {
        var rslt = jqueryCheckReq("helpform");
        if (rslt) {
            var elem = $("#email");
            if (elem.val().indexOf("@") < 0) {
                $("#emailerror").html("Please enter a valid email address").addClass("error");
                rslt = false;
            }
            if (rslt) {
                $("#jvpost").val("1");
                $("#sendBtn").prop("disabled", true);
                helpform.submit();
            }
                
        }
        return false;
    }

    

</script>

<style>
   

</style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div class="container-fluid maincontent">
      <div class="borderbox content">
         <h1>Ask a Support Question</h1>
         <div class="row">
             <form id="helpform" method="POST" action="help.aspx" onsubmit="return checkReq();">
              <div class="form-group row">
                 <label class="col-form-label offset-sm-1 col-sm-2" for="email">Email</label>
                 <div class="col-sm-9">
                   <input type="email" name="email" id="email" class="form-control form-control-md reqfld firstfld" reqmsg="Please enter an email address" />
                   <span class="form-text" id="emailerror"></span>
                 </div>
             </div>
             <div class="form-group row">
                 <label class="col-form-label offset-sm-1 col-sm-2" for="name">Name</label>
                 <div class="col-sm-9">
                   <input type="text" name="name" id="name" class="form-control form-control-md"/>
                 </div>
             </div>
             <div class="form-group">
                <label class="col-form-label offset-sm-3 col-sm-8" for="question" style="text-align: left">Question - Please be specific as possible</label>
                <div class="col-sm-4"></div>
             </div>
             <div class="form-group row">
                <div class="offset-sm-3 col-sm-9">
                  <textarea name="question" id="question" rows="10" cols="75" class="form-control form-control-md reqfld" reqmsg="Please enter your support question"></textarea>
                  <span class="form-text" id="questionerror"></span>
                </div>
             </div>
             <div class="form-group row">
               <div class="offset-sm-3 col-sm-9">
                  <button type="submit" class="btn btn-default" id="sendBtn">Send</button>
               </div>
             </div>
             <input type="hidden" name="jvpost" value="0" id="jvpost"/>
             </form>
         </div>
      </div>
   </div>
</asp:Content>
