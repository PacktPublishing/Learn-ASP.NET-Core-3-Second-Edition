function EnableCheckTurnIsFinished() {
    interval = setInterval(() => {
        CheckTurnIsFinished();
    }, 2000);
}

function CheckTurnIsFinished() {
    var port = document.location.port ? (":" +   document.location.port) : "";
    var url = document.location.protocol + "//" +
        document.location.hostname + port + "/restapi/v1/GetGameSession/" + window.GameSessionId;

    $.get(url, function (data) {
        if (data.turnFinished === true &&
            data.turnNumber >= window.TurnNumber) {
            CheckGameSessionIsFinished();
            ChangeTurn(data);
        }
    });
}

function ChangeTurn(data) {
    var turn = data.turns[data.turnNumber - 1];
    DisplayImageTurn(turn);

    $("#activeUser").text(data.activeUser.email);
    if (data.activeUser.email !== window.EmailPlayer) {
        DisableBoard(data.turnNumber);
    }
    else {
        EnableBoard(data.turnNumber);
    }
}

function DisableBoard(turnNumber) {
    var divBoard = $("#gameBoard");
    divBoard.hide();
    $("#divAlertWaitTurn").show();
    window.TurnNumber = turnNumber;
}

function EnableBoard(turnNumber) {
    var divBoard = $("#gameBoard");
    divBoard.show();
    $("#divAlertWaitTurn").hide();
    window.TurnNumber = turnNumber;
}

function DisplayImageTurn(turn) {
    var c = $("#c_" + turn.y + "_" + turn.x);
    var css;

    if (turn.iconNumber === "1") {
        css = 'glyphicon glyphicon-unchecked';
    }
    else {
        css = 'glyphicon glyphicon-remove-circle';
    }

    c.html('<i class="' + css + '"></i>');
} 