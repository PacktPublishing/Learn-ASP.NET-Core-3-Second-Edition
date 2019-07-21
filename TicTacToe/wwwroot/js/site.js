var interval;
function EmailConfirmation(email) {
    if (window.WebSocket) {
        alert("Websockets are enabled");
        openSocket(email, "Email");
    }
    else {
        alert("Websockets are not enabled");
        interval = setInterval(() => {
            CheckEmailConfirmationStatus(email);
        }, 5000);
    }
} 
function CheckEmailConfirmationStatus(email) {
    $.get("/CheckEmailConfirmationStatus?email=" + email,
        function (data) {
            if (data === "OK") {
                if (interval !== null)
                    clearInterval(interval);
                window.location.href = "/GameInvitation?email=" + email;
            }
        });
} 
var openSocket = function (parameter, strAction) {
    if (interval !== null)
        clearInterval(interval);

    var protocol = location.protocol === "https:" ? "wss:" : "ws:";
    var operation = "";
    var wsUri = "";

    if (strAction === "Email") {
        wsUri = protocol + "//" + window.location.host +
        "/CheckEmailConfirmationStatus";
        operation = "CheckEmailConfirmationStatus";
    }

    var socket = new WebSocket(wsUri);
    socket.onmessage = function (response) {
        console.log(response);
        if (strAction == "Email" && response.data == "OK") {
            window.location.href = "/GameInvitation?email=" +
                parameter;
        }
    };

    socket.onopen = function () {
        var json = JSON.stringify({
            "Operation": operation,
            "Parameters": parameter
        });

        socket.send(json);
    };

    socket.onclose = function (event) {
    };
};