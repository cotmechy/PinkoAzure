<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    PinkoHeartbeat
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">

        // RFrom View model        
        function PinkoRFormViewModel() {
            /// <summary>
            /// A ViewModel for searching Twitter for a given term.
            /// </summary>
            formResult: ko.observable();
        };

        ko.applyBindings(PinkoRFormViewModel);

        $(function () {
            //
            // SignalR client Object
            // https://github.com/SignalR/SignalR/wiki/SignalR-JS-Client-Hubs
            //
            console.log('PinkoHeartbeata.aspx: var huBconnection = $.connection...');
            var huBconnection = $.connection;

            huBconnection.hub.logging = true;

            var pinkoHub = huBconnection.pinkoSingalHub;
           
            // server calls this methods
            pinkoHub.addMessage = function (value) {
                //console.log('PinkoHeartbeata.aspx: Server called addMessage(' + value + ')');
                $('#ctrlInputIncomingText').val(value);
                
                //PinkoRFormViewModel.
            };

        });
    </script>

<h2>PinkoHeartbeat</h2>

        
Stream Text Incoming:
<br>
<input id="ctrlInputIncomingText" type="text" data-bind="value: PinkoRFormViewModel.formResult" />
<span id="ctrlInputIncomingSpan" data-bind="text: PinkoRFormViewModel.formResult" />

        


</asp:Content>
