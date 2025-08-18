<%@ Page Title="Orders" Language="C#" MasterPageFile="~/masterpage.Master" AutoEventWireup="true" CodeBehind="orders.aspx.cs" Inherits="aspnet.orders" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Orders</title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <style>
        .orders-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }
        .loading {
            text-align: center;
            padding: 20px;
            font-style: italic;
        }
        .orders-grid {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        .orders-grid th,
        .orders-grid td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: left;
        }
        .orders-grid th {
            background-color: #f2f2f2;
            font-weight: bold;
        }
        .orders-grid tr:nth-child(even) {
            background-color: #f9f9f9;
        }
        .error {
            color: red;
            padding: 10px;
            background-color: #ffe6e6;
            border: 1px solid #ffcccc;
            margin: 10px 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="orders-container">
        <h1>Orders Management</h1>
        
        <div id="loadingMessage" class="loading">
            Loading orders...
        </div>
        
        <div id="errorMessage" class="error" style="display: none;"></div>
        
        <div id="ordersContent" style="display: none;">
            <table id="ordersTable" class="orders-grid">
                <thead>
                    <tr>
                        <th>Order ID</th>
                        <th>Customer Name</th>
                        <th>Order Date</th>
                        <th>Total Amount</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody id="ordersTableBody">
                </tbody>
            </table>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function() {
            loadOrders();
        });

        function loadOrders() {
            $.ajax({
                url: 'orders.ashx',
                type: 'GET',
                dataType: 'json',
                success: function(data) {
                    $('#loadingMessage').hide();
                    
                    if (data.success) {
                        displayOrders(data.orders);
                    } else {
                        showError(data.message || 'Failed to load orders');
                    }
                },
                error: function(xhr, status, error) {
                    $('#loadingMessage').hide();
                    showError('Error loading orders: ' + error);
                }
            });
        }

        function displayOrders(orders) {
            var tbody = $('#ordersTableBody');
            tbody.empty();
            
            if (orders && orders.length > 0) {
                $.each(orders, function(index, order) {
                    var row = '<tr>' +
                        '<td>' + order.OrderId + '</td>' +
                        '<td>' + order.CustomerName + '</td>' +
                        '<td>' + order.OrderDate + '</td>' +
                        '<td>$' + parseFloat(order.TotalAmount).toFixed(2) + '</td>' +
                        '<td>' + order.Status + '</td>' +
                        '</tr>';
                    tbody.append(row);
                });
                
                $('#ordersContent').show();
            } else {
                showError('No orders found');
            }
        }

        function showError(message) {
            $('#errorMessage').text(message).show();
        }
    </script>
</asp:Content>