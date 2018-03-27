<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NTUCalGen.Default" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <!-- Global site tag (gtag.js) - Google Analytics -->
    <script data-cfasync="false" async src="https://www.googletagmanager.com/gtag/js?id=UA-115304762-2"></script>
    <script data-cfasync="false">
        window.dataLayer = window.dataLayer || [];
        function gtag() { dataLayer.push(arguments); }
        gtag('js', new Date());

        gtag('config', 'UA-115304762-2');
    </script>

    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="description" content="">
    <meta name="author" content="">
    <link rel="shortcut icon" href="../../docs-assets/ico/favicon.png">

    <title>NTUCalGen - Generate an iCal calendar from NTU's course timetable</title>

    <!-- Bootstrap core CSS -->
    <link href="assets/css/bootstrap.css" rel="stylesheet">


    <!-- Custom styles for this template -->
    <link href="assets/css/main.css" rel="stylesheet">

    <script src="https://code.jquery.com/jquery-1.10.2.min.js"></script>
    <script src="assets/js/hover.zoom.js"></script>
    <script src="assets/js/hover.zoom.conf.js"></script>

    <!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!--[if lt IE 9]>
      <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
      <script src="https://oss.maxcdn.com/libs/respond.js/1.3.0/respond.min.js"></script>
    <![endif]-->
</head>

<body>

    <!-- Static navbar -->
    <div class="navbar navbar-inverse navbar-static-top">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="Default.aspx">NTUCalGen</a>
            </div>
            <div class="navbar-collapse collapse">
                <%--                <ul class="nav navbar-nav navbar-right">
                    <li><a href="Default.aspx">Generate iCal</a></li>
                </ul>--%>
            </div>
            <!--/.nav-collapse -->
        </div>
    </div>


    <div class="container pt">
        <div class="row mt">
            <div class="col-lg-12">

                <div class="row mt">
                    <div class="col-lg-12 col-lg-offset-0 centered">
                        <h3>Generate an iCal calendar from NTU's course timetable.</h3>
                        <hr>

                        <p>
                            View your NTU Timetable on your smartphone / computer!<br />
                            Works with Android, iPhone, Microsoft Outlook, Google Calendar, etc.<br />
                            Even includes a map of your class location!<br />
                            <br />
                            <img src="images/screenshot1.png" width="200" />
                            <img src="images/screenshot2.png" width="200" />
                        </p>
                    </div>
                </div>
                <div class="row mt">
                    <form id="form1" runat="server">
                        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                Step 1.1: Log into <a href="https://intu.ntu.edu.sg/_layouts/iNTU/Main.aspx?Page=StudentLink">NTU StudentLink</a><br />
                                Step 1.2: Click on "Degree Audit"<br />
                                Step 1.3: Click on "View Course timetable"<br />
                                Step 1.4: Right-click on the timetable and select "View Source"<br />
                                Step 1.5: Select and paste everything into the textbox below<br />
                                <br />
                                <h4>Step 2: If you see the HTML source below, click on "Parse!"</h4>
                                <div class="form-group">

                                    <asp:TextBox ID="txtSrc" runat="server" CssClass="form-control" TextMode="MultiLine" placeholder="HTML Script" Rows="5"></asp:TextBox>

                                </div>
                                <div class="form-group">
                                    <div class="checkbox">
                                        <label>
                                            <asp:CheckBox ID="chkAbbr" runat="server" />
                                            Abbreviate Subject Titles</label>
                                    </div>
                                </div>

                                <asp:HiddenField ID="hidMatric" runat="server" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                        <asp:Button ID="btnParse" runat="server" Text="Parse!" CssClass="btn btn-success" OnClick="btnParse_Click" />

                        <p>
                            <br />
                            <asp:Label ID="lblError" runat="server" Text="iCal will be downloaded once you click on 'Parse!'"></asp:Label>
                            <br />
                            To import calendar to: <a href="https://play.google.com/store/apps/details?id=tk.drlue.icalimportexport&hl=en">Android</a> | <a href="https://support.google.com/calendar/answer/37118?hl=en">Google</a> | <a href="https://www.google.com/search?q=iPhone+import+iCal">iPhone</a> | <a href="https://support.office.com/en-us/article/Import-iCal-or-Address-Book-items-into-Outlook-2a637ac6-f3b5-411d-8a73-016bd90c1094">Outlook</a>

                        </p>

                        <asp:UpdateProgress ID="UpdateProgress1" runat="server">
                            <ProgressTemplate>
                                <div class="modal" style="display: block; background-color: #222222; opacity: 0.4;" aria-hidden="false">
                                </div>
                                <div class="modal" style="display: block;" aria-hidden="false">
                                    <div class="modal-dialog">
                                        <div class="modal-content">
                                            <div class="modal-body">
                                                Loading...
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </ProgressTemplate>
                        </asp:UpdateProgress>
                    </form>

                </div>
            </div>
            <!-- /row -->
        </div>
    </div>
    <!-- /container -->

    <div id="footer">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <p>
                        Developed by <a href="https://www.cczy.io">Calvin Che</a>. For any feedback/bugs, please leave a comment
                            <a href="https://www.cczy.io/tools/ntucalgen/">here</a>. Thanks!
                    </p>
                </div>
                <!-- /col-lg-4 -->

            </div>

        </div>
    </div>


    <!-- Bootstrap core JavaScript
    ================================================== -->
    <!-- Placed at the end of the document so the pages load faster -->
    <script src="assets/js/bootstrap.min.js"></script>

</body>
</html>
