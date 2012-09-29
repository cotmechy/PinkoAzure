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
            <table>
                <tr>
                    <td>FormulaId</td>
                    <td>User Label</td>
                    <td>Formula</td>
                    <td>Error</td>
                </tr>
                <tr>
                    <td><input data-bind="value: FormulaId" type="text"/></td>
                    <td><input data-bind="value: ExpressionLabel" type="text"  /></td>
                    <td><input data-bind="value: ExpressionFormula" type="text"  /></td>
                    <td><textarea data-bind="value: ErrorMessage" /></textarea></td>
                </tr>
            </table>
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
            <input id="ctrlBtnSubmit" type="submit" />
        </form>
    </div>

    <!-- Kendo template -->    
    <script id="tmplFromulaTemplateDefinition" type="text/x-kendo-template">
        <ul>
            <li>#= data[i] #</li>
        <ul>
    </script>

    <script>

        // Formula representation
        function PinkoUserExpressionFormula()
        {
            this.FormulaId = "";
            this.ExpressionLabel = "";
            this.ExpressionFormula = "";

            this.serializeUrl = function ()
            {
                return this.FormulaId + ":" + this.ExpressionLabel + ":" + this.ExpressionFormula + ";";
            };
        }

        // PinkoCalculateExpression View model
        // http://docs.kendoui.com/getting-started/framework/mvvm/overview
        var vmFormulaResult = kendo.observable({
            FormulaId: "NoId",
            ExpressionLabel: "A",
            ExpressionFormula: "1.2+1.5",
            ResultValue: "---",
            LastResultTimeStamp: "---",
            ResultType: "---",
            MaketEnvId: "MaketEnvId",
            ErrorCode: "",
            ErrorMessage: "",
            
            serializedFormula: function ()
            {
                return this.get("FormulaId") + ":" + this.get("ExpressionLabel") + ":" + this.get("ExpressionFormula") + ";";
            }
        });


        $(document).ready(function() {
            console.log('Binding Kendo VM...');

            kendo.bind($("#ctrlDivRequestFormula"), vmFormulaResult);
            //kendo.bind($("#ctrlDivRequestFormulaId"), vmUserContext);

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
                // http://localhost:49171/api/PinkoFormProcessor?callback=jQuery171004215973778627813_1348051384112&ExpressionFormula=1.5%2B1.5&MaketEnvId=MaketEnvId&clientCtx=ClientCtx&ClientId=ClientId&SignalRId=44b09f9e-7e10-422b-b39c-fcfbee2f5455&WebRoleId=6943640d-4206-46b8-af37-8ce764c72367&_=1348051387334

                var urlData =
                    "expressionFormula=" + encodeURIComponent(vmFormulaResult.serializedFormula()) +
                        "&maketEnvId=" + encodeURIComponent(vmFormulaResult.MaketEnvId) +
                        "&clientCtx=" + encodeURIComponent(vmUserContext.ClientCtx) +
                        "&clientId=" + encodeURIComponent(vmUserContext.ClientId) +
                        "&signalRId=" + encodeURIComponent(vmUserContext.SignalRId) +
                        "&webRoleId=" + encodeURIComponent(vmUserContext.WebRoleId); 
                        //"&subscibtionId=" + encodeURIComponent("SubsId");
                
                
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

            //
            // Expression success: Receives one formula at a time
            //
            pinkoHub.expressionResponse = function (clientCtx, results, resultType, formulaId)
            {
                var pinkoPoint = results[0];
                
                console.log("from Master ExpressionResponse: " + clientCtx + " :: " + resultType + " :: " + formulaId);
                vmFormulaResult.set("subscibtionId", subscibtionId);
                vmFormulaResult.set("ResultValue", pinkoPoint.PointValue);
                vmFormulaResult.set("ResultType", resultType);
                vmFormulaResult.set("ErrorCode", "");
                vmFormulaResult.set("ErrorMessage", "");
                vmFormulaResult.set("LastResultTimeStamp", new Date().toTimeString());
            };


            //
            // Expression Error
            //
            pinkoHub.expressionResponseError = function (clientCtx, subscibtionId, resultValue, errorCode, errorDescription)
            {
                console.log("from Master ExpressionResponseError: " + clientCtx + " :: " + resultValue + " :: " + errorCode + " :: " + errorDescription);

                //vmFormulaResult.set("FormulaId", formulaId);
                vmFormulaResult.set("ResultValue", resultValue);
                vmFormulaResult.set("ErrorCode", errorCode);
                vmFormulaResult.set("ErrorMessage", errorDescription);
                vmFormulaResult.set("LastResultTimeStamp", new Date().toTimeString());

            };

        }
        );
    </script>


</asp:Content>