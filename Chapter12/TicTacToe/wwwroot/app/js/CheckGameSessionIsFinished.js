function CheckGameSessionIsFinished() {
    var port = document.location.port ? (":" +  document.location.port) : "";
    var url = document.location.protocol + "//" +
        document.location.hostname + port +
        "/restapi/v1/CheckGameSessionIsFinished/" +  window.GameSessionId;

    $.get(url, function (data) {
        debugger;
        if (data.indexOf("won") > 0 || data == "The game was a draw.") { 
        alert(data);
        window.location.href = document.location.protocol +
            "//" + document.location.hostname + port;
    } 
          }); 
        } 