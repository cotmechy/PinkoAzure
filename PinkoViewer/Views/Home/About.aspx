<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">About Us</asp:Content>


<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Pinko Sample Snapshot Formula</h2>
    
    <!-- Request form -->
    <div id="ctrlDivRequestFormula">
        <!-- 
        <form id="Form1" method="post" action="http://localhost:49171/api/PinkoFormProcessor/PostRequestFormula">
        -->
        <form id="ctrlFormRequestFormula">
            Enter Formula: 
            <br />
            <textarea data-bind="value: ExpressionFormula" ></textarea>
            <br/>
            Result: 
            <input data-bind="value: ResultValue" type="text"  />
            <br />
            ResultType:
            <input data-bind="value: ResultType" type="text"  />
            <br />
            LastResultTimeStamp:
            <input data-bind="value: LastResultTimeStamp" type="text" />
            <br />
            ErrorCode:
            <input data-bind="value: ErrorCode" type="text"  />
            <br />
            ErrorMessage:
            <br />
            <textarea data-bind="value: ErrorMessage" /></textarea>
            <br />
            <input id="ctrlBtnSubmit" type="submit" />
        </form>
    </div>


    <!-- window -->
    <!--<button id="ctrlOpenSinpleSample">Formula Selector</button>-->
<!--    <div id="ctlrWndFormualSelector">
        <div id="ctrldictionaryGrid">
            
        </div>
    </div>-->

    <!-- Kendo template -->    
    <script id="tmplFromulaTemplateDefinition" type="text/x-kendo-template">
        <ul>
            <li>#= data[i] #</li>
        <ul>
    </script>

    <script>

        // PinkoCalculateExpression View model
        // http://docs.kendoui.com/getting-started/framework/mvvm/overview
        var vmFormulaResult = kendo.observable({
            ExpressionFormula: "1.5+1.5",
            ResultValue: "---",
            LastResultTimeStamp: "---",
            ResultType: "---",
            MaketEnvId: "MaketEnvId",
            //ClientCtx: "ClientCtx",
            ErrorCode: "",
            ErrorMessage: "",
            //ClientId: "ClientId",
        });


        $(document).ready(function() {
            console.log('Binding Kendo VM...');

            kendo.bind($("#ctrlDivRequestFormula"), vmFormulaResult);
            //kendo.bind($("#ctrlDivRequestFormula"), vmUserContext);

            console.log('Binding Kendo tmplFromulaTemplate...');
            var tmplFromulaTemplate = kendo.template($("#tmplFromulaTemplateDefinition").html());


            // Creating dictionary data source
            // http://docs.kendoui.com/getting-started/framework/datasource/overview
            console.log('CreatingKenod Data source...');
            var dsFormulaDictionary = new kendo.data.DataSource(
                {
                    transport:
                        {
                            read:
                                {
                                    // the remote service url
                                    url: "http://localhost:49171/api/PinkoDictionary",

                                    // JSONP is required for cross-domain AJAX
                                    dataType: "jsonp"
                                }
                        },
                    //schema:
                    //    {
                    //        data: "results"
                    //    }
                });


            // Bind data source to widget
            console.log('Creating Kendo Grid ctrldictionaryGrid...');
            $("#ctrldictionaryGrid").kendoGrid(
                {
                    columns:
                        [
                            {
                                field: "FormulaExp",
                                title: "Expression"
                            }
                            //<EntitytFormulaExpression>
                            //<DateTimeStamp>2012-08-27T00:42:52.1009344Z</DateTimeStamp>
                            //<FormulaExp>=RForm("Symbol", "1000", "Price.Bid", "Reuters")</FormulaExp>
                            //<Id>{10008D0-E0F1-4F00-B8B2-9EE03E92FE9B}</Id>
                            //<LastUpdateStamp>2012-08-27T00:42:52.1009344Z</LastUpdateStamp>
                            //</EntitytFormulaExpression>                
                        ],
                    dataSource: dsFormulaDictionary
                });

            //var vmUserContext = kendo.observable({
            //    ClientCtx: "ClientCtx",
            //    ClientId: "ClientId",
            //    SignalRId: "",
            //    WebRoleId: ""
            //});



            //$("#ctlrWndFormualSelector").kendoWindow({
            //    actions: ["Maximize", "Minimize", "Close"],
            //    //actions: ["Custom", "Refresh", "Maximize", "Minimize", "Close"],
            //    //draggable: false,
            //    //modal: true,
            //    //resizable: false,
            //    title: "Formula Chooser",
            //    //height: "300px",
            //    //width: "500px"
            //});

            //$("#ctrlOpenSinpleSample").click(function ()
            //{
            //    var win = $("#ctlrWndFormualSelector").data("kendoWindow");
            //    //win.center();
            //    win.open();

            //});



            // submit formula to web service
            // http://stackoverflow.com/questions/1960240/jquery-ajax-submit-form
            $("#ctrlFormRequestFormula").submit(function ()
            {
                
                // string expressionFormula, string maketEnvId, string clientCtx, string clientId, string signalRId, string webRoleId
                
                // http://localhost:49171/api/PinkoFormProcessor?callback=jQuery171004215973778627813_1348051384112&ExpressionFormula=1.5%2B1.5&MaketEnvId=MaketEnvId&clientCtx=ClientCtx&ClientId=ClientId&SignalRId=44b09f9e-7e10-422b-b39c-fcfbee2f5455&WebRoleId=6943640d-4206-46b8-af37-8ce764c72367&_=1348051387334
                var urlData = "ExpressionFormula=" + encodeURIComponent(vmFormulaResult.ExpressionFormula) +
                    "&MaketEnvId=" + encodeURIComponent(vmFormulaResult.MaketEnvId) +
                    "&clientCtx=" + encodeURIComponent(vmUserContext.ClientCtx) +
                    "&ClientId=" + encodeURIComponent(vmUserContext.ClientId) +
                    "&SignalRId=" + encodeURIComponent(vmUserContext.SignalRId) +
                    "&WebRoleId=" + encodeURIComponent(vmUserContext.WebRoleId)
                    ;

                
                console.log("posting formula: " + urlData);

                jQuery.ajax({
                    type: "GET",
                    url: "http://localhost:49171/api/PinkoFormProcessor",
                    data: urlData,
                    success: function (value)
                    {
                        console.log("Post response: ctrlFormRequestFormula: " + value);
                    },
                    dataType: "jsonp"
                });

                return false;
            });



            var huBconnection = $.connection;
            var pinkoHub = huBconnection.pinkoSingalHub;

            //// pinkoHub
            ////pinkoHub.notifyClientPinkoRoleHeartbeat = function (datetimeStamp, machineName, signalRId)
            //pinkoHub.expressionResponse = function (expreResult)
            //{
            //    console.log("ExpressionResponse: " + expreResult);
            //    //console.log('notifyClientPinkoRoleHeartbeat: Server called: datetimeStamp: ' + datetimeStamp + ' - machineName: ' + machineName);
            //    //$('#ctrlTimeStamp').text(datetimeStamp);
            //};

            //
            // Expression success
            //
            pinkoHub.expressionResponse = function(clientCtx, resultexpression, resultType, resultValue)
            {
                console.log("from Master ExpressionResponse: " + clientCtx + " :: " + resultType + " :: " + resultValue);
                vmFormulaResult.set("ExpressionFormula", resultexpression);
                vmFormulaResult.set("ResultValue", resultValue);
                vmFormulaResult.set("ResultType", resultType);
                vmFormulaResult.set("ErrorCode", "");
                vmFormulaResult.set("ErrorMessage", "");
                vmFormulaResult.set("LastResultTimeStamp", new Date().toTimeString());
            };


            //
            // Expression Error
            //
            pinkoHub.expressionResponseError = function(clientCtx, resultexpression, resultValue, errorCode, errorDescription)
            {
                console.log("from Master ExpressionResponseError: " + clientCtx + " :: " + resultValue + " :: " + errorCode + " :: " + errorDescription);

                vmFormulaResult.set("ExpressionFormula", resultexpression);
                vmFormulaResult.set("ResultValue", resultValue);
                vmFormulaResult.set("ErrorCode", errorCode);
                vmFormulaResult.set("ErrorMessage", errorDescription);
                vmFormulaResult.set("LastResultTimeStamp", new Date().toTimeString());

            };

        }
        );
    </script>


</asp:Content>