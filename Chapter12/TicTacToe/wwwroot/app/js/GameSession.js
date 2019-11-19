function SetGameSession(gdSessionId, strEmail) {
    window.GameSessionId = gdSessionId;
    window.EmailPlayer = strEmail;
    window.TurnNumber = 0;
}
 
$(document).ready(function () {
    $(".btn-SetPosition").click(function () {
        var intX = $(this).attr("data-X");
        var intY = $(this).attr("data-Y");
        SendPosition(window.GameSessionId, window.EmailPlayer,
            intX, intY);
    });
});

function SendPosition(gdSession, strEmail, intX, intY) {
    var port = document.location.port ? (":" +
        document.location.port) : "";
    var url = document.location.protocol + "//" +
        document.location.hostname + port +
        "/restApi/v1/SetGamePosition/" + gdSession;
    var obj = {
        "Email": strEmail, "x": intX, "y": intY
    };

    var json = JSON.stringify(obj);
    $.ajax({
        'url': url,
        'accepts': "application/json; charset=utf-8",
        'contentType': "application/json",
        'data': json,
        'dataType': "json",
        'type': "POST",
        'success': function (data) {
            alert(data);
        }
    });
}
