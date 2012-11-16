<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Subscription Samples
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Pinko Subscription Formula</h2>
    
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
                    <td>Error</td>
                </tr>
                <tr>
                    <td><input data-bind="value: FormulaId" type="text"/></td>
                    <td><input data-bind="value: ExpressionLabel" type="text"  /></td>
                    <td><textarea data-bind="value: ErrorMessage"></textarea></td>
                </tr>
                <tr>
                    <td colspan="5"><textarea cols="70" data-bind="value: ExpressionFormula"></textarea></td>
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
<%--    <script id="tmplFromulaTemplateDefinition" type="text/x-kendo-template">
        <ul>
            <li>#= data[i] #</li>
        <ul>
    </script>--%>

    <script>

        //// Formula representation
        //function PinkoUserExpressionFormula()
        //{
        //    this.FormulaId = "";
        //    this.ExpressionLabel = "";
        //    this.ExpressionFormula = "";

        //    this.serializeUrl = function ()
        //    {
        //        return this.FormulaId + ":" + this.ExpressionLabel + ":" + this.ExpressionFormula + ";";
        //    };
        //}

        // PinkoCalculateExpression View model
        // http://docs.kendoui.com/getting-started/framework/mvvm/overview
        var vmFormulaResult = kendo.observable({
            FormulaId: "NoId",
            ExpressionLabel: "A",
            ExpressionFormula: "=RForm(\"time\", \"usa\", \"seconds\", \"local\")",
            ResultValue: "---",
            LastResultTimeStamp: "---",
            ResultType: "---",
            MaketEnvId: "MaketEnvId",
            ErrorCode: "",
            ErrorMessage: "",
            SubsId: "<%= Guid.NewGuid().ToString() %>",
            
            serializedFormula: function ()
            {
                return this.get("FormulaId") + ":" + this.get("ExpressionLabel") + ":" + this.get("ExpressionFormula") + ";";
            }
        });


        $(document).ready(function ()
        {
            console.log('Binding Kendo VM...');

            kendo.bind($("#ctrlDivRequestFormula"), vmFormulaResult);

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
                        "&webRoleId=" + encodeURIComponent(vmUserContext.WebRoleId) +
                        "&subscribtionId=" + encodeURIComponent(vmFormulaResult.SubsId);

                var restUrl = "http://localhost:49171/api/PinkoFormProcessorSubscriber";

                console.log("Posting Formula: " + restUrl + urlData);


                jQuery.ajax({
                    type: "GET",
                    url: restUrl,
                    data: urlData,
                    success: function (value)
                    {
                        console.log("Post response: ctrlFormRequestFormula: " + value);
                    },
                    dataType: "jsonp",
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

                //console.log("from Master ExpressionResponse: " + clientCtx + " :: " + resultType + " :: " + formulaId);
                //vmFormulaResult.set("subscribtionId", subscribtionId);
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

            // 
            // Client Ping
            //
            setInterval(function ()
            {
                var urlData =
                        "&maketEnvId=" + encodeURIComponent(vmFormulaResult.MaketEnvId) +
                        "&clientCtx=" + encodeURIComponent(vmUserContext.ClientCtx) +
                        "&clientId=" + encodeURIComponent(vmUserContext.ClientId) +
                        "&signalRId=" + encodeURIComponent(vmUserContext.SignalRId) +
                        "&webRoleId=" + encodeURIComponent(vmUserContext.WebRoleId) +
                        "&subscribtionId=" + encodeURIComponent(vmFormulaResult.SubsId);

                // http://localhost:49171/Api/PinkoClientPing?maketEnvId=MaketEnvId&clientCtx=ClientCtx&clientId=ClientId&signalRId=c9f66445-1dd5-45cc-8547-d6ed32b6e3a6&webRoleId=86ec389f-ee1d-4d0f-beeb-c925f2240ac9&subscribtionId=subscribtionId

                //console.log("callPing...");

                jQuery.ajax({
                    type: "GET",
                    url: "http://localhost:49171/api/PinkoClientPing",
                    data: urlData
                    //success: function (value)
                    //{
                    //    //console.log("Post response: ctrlFormRequestFormula: " + value);
                    //},
                    //dataType: "jsonp"
                });
            }, 5000);


        });  // $(document).ready(function ()
    </script>

</asp:Content>
