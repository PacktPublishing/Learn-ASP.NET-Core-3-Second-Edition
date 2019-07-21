
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