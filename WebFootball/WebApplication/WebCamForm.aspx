﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebCamForm.aspx.cs" Inherits="WebApplication.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <script src='<%=ResolveUrl("~/Webcam_Plugin/jquery.Webcam.js") %>' type="text/javascript"></script>
    <script type="text/javascript">
    var pageUrl = '<%=ResolveUrl("~/Default.aspx") %>';
    $(function () {
    jQuery("#Webcam").webcam({
        width: 320,
        height: 240,
        mode: "save",
        swffile: '<%=ResolveUrl("~/Webcam_Plugin/jscam.swf") %>',
        debug: function (type, status) {
            $('#camStatus').append(type + ": " + status + '<br /><br />');
        },
        onSave: function (data) {
            $.ajax({
                type: "POST",
                url: pageUrl + "/GetCapturedImage",
                data: '',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (r) {
                    $("[id*=imgCapture]").css("visibility", "visible");
                    $("[id*=imgCapture]").attr("src", r.d);
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
        },
        onCapture: function () {
            webcam.save(pageUrl);
        }
     });
    });
    function Capture() {
    webcam.capture();
    return false;
    }
    </script>
    <style type="text/css">
        body {
            
            background: url("http://foosballtable.reviews/wp-content/uploads/2016/05/best-foosball-tables-compared.jpg");
            color: white;
            font-family: Helvetica;
            background-size: cover;
            background-position: center center;
            background-repeat: no-repeat;
            background-attachment: fixed;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td class="auto-style1">
                    <u>Live Camera</u>
                </td>
                <td>
                </td>
                <td>
                       <u>Captured Picture</u>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">
                     <div id="webcam">
                     </div>
                </td>
        <td>
            &nbsp;
        </td>
        <td>
            <asp:Image ID="imgCapture" runat="server" Style="visibility: hidden; width: 320px;
                height: 240px" />
        </td>
    </tr>
    </table>
    <br />
    <asp:Button ID="btnCapture" Text="Capture" runat="server" OnClientClick="return Capture();" />
    <br />
    <span id="camStatus"></span>
    </form>
</body>
</html>
