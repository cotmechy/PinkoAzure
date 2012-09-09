<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">About Us</asp:Content>


<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Sample Formula</h2>
    
    <!-- Request form -->
    <div id="ctrlDivRequestFormula">
        <!-- 
        <form id="Form1" method="post" action="http://localhost:49171/api/PinkoFormProcessor/PostRequestFormula">
        -->
        <form id="ctrlFormRequestFormula">
                Enter Formula: 
            <input id="ExpressionFormula" data-bind="value: ExpressionFormula" type="text" width="200" />
            <br/>
            Result: 
            <input id="ResultValue" data-bind="value: ResultValue" type="text" width="200" />
            <br/>
            <input id="ctrlBtnSubmit" type="submit" />
        </form>
    </div>


    <!-- window -->
    <button id="ctrlOpenSinpleSample">Formula Selector</button>
    <div id="ctlrWndFormualSelector">
        <div id="ctrldictionaryGrid">
            
        </div>
    </div>

    <!-- Kendo template -->    
    <script id="tmplFromulaTemplateDefinition" type="text/x-kendo-template">
        <ul>
            <li>#= data[i] #</li>
        <ul>
    </script>

    <script>
        {
            console.log('Binding Kendo VM...');

            // PinkoCalculateExpression View model
            // http://docs.kendoui.com/getting-started/framework/mvvm/overview
            var vmFormulaResult = kendo.observable({
                ExpressionFormula: "=1+1",
                ResultValue: "---",
                ResultType: "---",
                MaketEnvId: "MaketEnvId",
                ClientCtx: "ClientCtx"
            });
            kendo.bind($("#ctrlDivRequestFormula"), vmFormulaResult);

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


            // submit formula to web service
            // http://stackoverflow.com/questions/1960240/jquery-ajax-submit-form
            $("#ctrlFormRequestFormula").submit(function ()
            {
                var urlData = "ExpressionFormula=" + encodeURIComponent(vmFormulaResult.ExpressionFormula) + "&MaketEnvId=" + encodeURIComponent(vmFormulaResult.MaketEnvId) + "&clientCtx=" + encodeURIComponent(vmFormulaResult.ClientCtx);
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


            $("#ctlrWndFormualSelector").kendoWindow({
                actions: ["Maximize", "Minimize", "Close"],
                //actions: ["Custom", "Refresh", "Maximize", "Minimize", "Close"],
                //draggable: false,
                //modal: true,
                //resizable: false,
                title: "Formula Chooser",
                //height: "300px",
                //width: "500px"
            });

            $("#ctrlOpenSinpleSample").click(function ()
            {
                var win = $("#ctlrWndFormualSelector").data("kendoWindow");
                //win.center();
                win.open();

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
            pinkoHub.expressionResponse = function (clientCtx, resultType, resultValue)
            {
                console.log("from Master ExpressionResponse: " + clientCtx + " :: " + resultType + " :: " + resultValue);
                //vmFormulaResult.ResultValue("resultValue");
            };


            //
            // Expression Error
            //
            pinkoHub.expressionResponseError = function (clientCtx, resultValue, errorCode, errorDescription)
            {
                console.log("from Master ExpressionResponseError: " + clientCtx + " :: " + resultValue + " :: " + errorCode + " :: " + errorDescription);
                //vmFormulaResult.ResultValue("resultValue");
                //$("#ResultValue"). (resultValue);
            };

        };
    </script>


</asp:Content>