let currentPos = 32;
let previousPos = 32;

$(()=>{
//onload
let p1board = "<table><tr>";
let p2board = "<table><tr>";
let imageBoard = ""; 
for(let i = 1; i < 66; i++)
{
    p1board += `<td>${i}</td>`;
    p2board += `<td>${i}</td>`;
    imageBoard += `<span id='${i}'></span>`;
}
p1board += "</tr></table>";
p2board += "</tr></table>";
$(".p1Board").append(p1board);
$(".p2Board").append(p2board);
$(".imageBoard").append(imageBoard);
$(".p1Board").hide();
$(".p2Board").hide();
$(".imageBoard").hide();
$("#pull").hide();

$("#32").html("<img src='tugofwar.png' alt='Tug of War'>");
//status message for user
let status = "Please Enter Player Names";
console.log("On page load");

//updating status message
$("#status").html(status);

//event handler for new game button
$("#newGame").on("click", ()=>{
    

    //getting player names
    let p1 = $("#player1").val();
    let p2 = $("#player2").val();
    if(p1 == "")
    {
        status = "Please Enter player 1 name";
        $("#status").html(status);
        return;
    }
    else if(p2 == "")
    {
        status = "Please enter player 2 name";
        $("#status").html(status);
        return;
    }
    else
    {
        console.log("new game");

        //data to be sent to server
    let data = {
        action: "NewGame",
        player1: p1,
        player2: p2,
        current: currentPos,
        previous: previousPos
    };
    
    $(".p1Board").show();
    $(".p2Board").show();
    $(".imageBoard").show();
    $("#pull").show();
    //making ajax call with data and success/error functions
    Ajax("http://localhost:5155/game","POST",data,"JSON",successFunc,errorFunc);
    }
    
})

//event handler for quit game button
$("#quitGame").on("click", ()=>{{
    console.log("quit game");
    //getting player names
    let p1 = $("#player1").val();
    let p2 = $("#player2").val();
    //data to be sent to server
    let data = {
        action: "QuitGame",
        player1: p1,
        player2: p2
    };

    //making ajax call with data and success/error functions
    Ajax("http://localhost:5155/game","POST",data,"JSON",QuitFunc,errorFunc);
}})

$("#pull").on("click", ()=>{
    let p1 = $("#player1").val();
    let p2 = $("#player2").val();

    $(`#${previousPos}`).html("");
    $(`#${currentPos}`).html("");
    console.log("prev:" + previousPos);
    console.log("curr:" + currentPos);
    let data = {
        action: "MakeMove",
        player1: p1,
        player2: p2,
        current: currentPos,
        previous: previousPos
    };
    Ajax("http://localhost:5155/game","POST",data,"JSON",successMove,errorFunc);
})

});
//end of onload
//ajax function to send data to the server
function Ajax(url, method, data, dataType, success, errMehtod)
{
    let ajOp={};
    ajOp['url']=url;
    ajOp['method']=method;
    ajOp['data']= JSON.stringify(data);  // New for ASP ajax call
    ajOp['dataType']=dataType;
    ajOp['success']=success;
    ajOp['error']=errMehtod;
    ajOp['contentType']= "application/json"; // New for ASP Part

    console.log(ajOp);

    $.ajax(ajOp);
}
// errorHandler is function to handle errors
function errorFunc(ajaxReq, ajaxStatus, errorThrown)
{
    console.log("Error");
    console.log(errorThrown);
}

//function to processs welcome ajax call
function successFunc(returnData, status)
{
    console.log(returnData);
    $("#status").html(returnData.status);
}
//function to processs welcome ajax call
function QuitFunc(returnData, status)
{
    console.log(returnData);
    $("#player1").val("");
    $("#player2").val("");
    let message = "Please Enter Player Names";
    
    $("#status").html(message);
    $(".p1Board").hide();
    $(".p2Board").hide();
    $(".imageBoard").empty();
    let imageBoard = ""; 
    for(let i = 1; i < 66; i++)
        {
            imageBoard += `<span id='${i}'></span>`;
        }
    $(".imageBoard").append(imageBoard);
    $("#32").html("<img src='tugofwar.png' alt='Tug of War'>");
    $(".imageBoard").hide();
}
function successMove(returnData, status)
{
    if(returnData.turnsTaken == 30)
    {
        let status = "Game Over";
        $("#status").html(status);
        $("#pull").hide();
        $("#player1").val("");
        $("#player2").val("");
        return;
    }
    if(returnData.current >= 40)
    {
        let status = "Player 2 Wins";
        $("#status").html(status);
        $("#pull").hide();
        return;
    }
    if(returnData.current<= 25)
        {
            let status = "Player 1 Wins";
            $("#status").html(status);
            $("#pull").hide();
            return;
        }
    console.log(returnData);
    if(returnData.playerTurn)
    {
        //p1 turn
        let status = `${returnData.p1}'s turn. Moved to ${returnData.current}`;
        $("#status").html(status);
    }
    if(!returnData.playerTurn)
    {
        //p2 turn
        let status = `${returnData.p2}'s turn. Moved to ${returnData.current}`;
        $("#status").html(status);
    }
        currentPos = returnData.current;
        previousPos = returnData.previous;
        console.log("new prev:" + previousPos);
        console.log("new curr:" + currentPos);
        $(`#${previousPos}`).html("");
        $(`#${currentPos}`).html("");
        $(`#${currentPos}`).html("<img src='tugofwar.png' alt='Tug of War'>");
        console.log("displacement: " + returnData.displacement);
        console.log("turns" +returnData.turnsTaken);
}