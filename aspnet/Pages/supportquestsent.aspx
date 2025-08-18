<%@ Page Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="supportquestsent.aspx.cs" Inherits="aspnet.Pages.supportquestsent" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
    <title>Support Question Sent</title>
    <style>
        .confirmation-container {
            max-width: 800px;
            margin: 0 auto;
            padding: 40px 20px;
            text-align: center;
        }
        .success-message {
            background-color: #d4edda;
            border: 1px solid #c3e6cb;
            color: #155724;
            padding: 20px;
            border-radius: 5px;
            margin-bottom: 30px;
        }
        .success-icon {
            font-size: 48px;
            color: #28a745;
            margin-bottom: 20px;
        }
        .success-title {
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 15px;
            color: #155724;
        }
        .success-description {
            font-size: 16px;
            line-height: 1.5;
            margin-bottom: 20px;
        }
        .next-steps {
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            padding: 20px;
            border-radius: 5px;
            margin-top: 20px;
            text-align: left;
        }
        .next-steps h3 {
            color: #495057;
            margin-bottom: 15px;
        }
        .next-steps ul {
            margin: 0;
            padding-left: 20px;
        }
        .next-steps li {
            margin-bottom: 8px;
            color: #6c757d;
        }
        .back-link {
            margin-top: 30px;
        }
        .back-link a {
            color: #007bff;
            text-decoration: none;
            font-weight: 500;
        }
        .back-link a:hover {
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="confirmation-container">
        <div class="success-message">
            <div class="success-icon">?</div>
            <div class="success-title">Support Question Sent Successfully!</div>
            <div class="success-description">
                Your support question has been sent to our support staff. We appreciate you taking the time to contact us.
            </div>
        </div>

        <div class="next-steps">
            <h3>What happens next?</h3>
            <ul>
                <li>Our support team will review your question</li>
                <li>You will receive a response via email within 1-2 business days</li>
                <li>For urgent matters, please check our FAQ or contact us directly</li>
                <li>Keep an eye on your spam folder in case our response goes there</li>
            </ul>
        </div>

        <div class="back-link">
            <a href="help.aspx">? Back to Help</a> | 
            <a href="home.aspx">Go to Home</a>
        </div>
    </div>
</asp:Content>