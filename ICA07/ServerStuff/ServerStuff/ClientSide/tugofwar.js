//globals to update postions
let currentPos = 32;
let previousPos = 32;

$(()=>{
//onload

//populating tables for player 1, 2 and image board
let p1board = "<table><tr>";
let p2board = "<table><tr>";
let imageBoard = ""; 

//space is 65 each so will build each with a for loop
for(let i = 1; i < 66; i++)
{
    p1board += `<td>${i}</td>`;
    p2board += `<td>${i}</td>`;
    imageBoard += `<span id='${i}'></span>`;
}
//finish the tables
p1board += "</tr></table>";
p2board += "</tr></table>";

//append created board
$(".p1Board").append(p1board);
$(".p2Board").append(p2board);
$(".imageBoard").append(imageBoard);

//hide everything until game starts
$(".p1Board").hide();
$(".p2Board").hide();
$(".imageBoard").hide();
$("#pull").hide();

//place image in the center position
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

    //check for valid p1 input
    if(p1 == "")
    {
        status = "Please Enter player 1 name";
        $("#status").html(status);
        return;
    }
    //check for valid p2 input
    else if(p2 == "")
    {
        status = "Please enter player 2 name";
        $("#status").html(status);
        return;
    }
    else
    {
        //player names are good time to make the game
        console.log("new game");

        //data to be sent to server
    let data = {
        action: "NewGame",
        player1: p1,
        player2: p2,
        current: currentPos,
        previous: previousPos
    };
    
    //show the board and pull button
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

//event handler for clicking pull
$("#pull").on("click", ()=>{

    //storing p1 and p2 names
    let p1 = $("#player1").val();
    let p2 = $("#player2").val();

    //clear previous positions
    $(`#${previousPos}`).html("");
    $(`#${currentPos}`).html("");
    console.log("prev:" + previousPos);
    console.log("curr:" + currentPos);

    //build data object
    let data = {
        action: "MakeMove",
        player1: p1,
        player2: p2,
        current: currentPos,
        previous: previousPos
    };

    //send to user
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
    //reset player names
    console.log(returnData);
    $("#player1").val("");
    $("#player2").val("");
    let message = "Please Enter Player Names";
    
    //display to user
    $("#status").html(message);

    //hide board
    $(".p1Board").hide();
    $(".p2Board").hide();

    //rebuild image board
    $(".imageBoard").empty();
    let imageBoard = ""; 
    for(let i = 1; i < 66; i++)
        {
            imageBoard += `<span id='${i}'></span>`;
        }
    //add the image and hide it for next round
    $(".imageBoard").append(imageBoard);
    $("#32").html("<img src='tugofwar.png' alt='Tug of War'>");
    $(".imageBoard").hide();
}

//function to handle a successive move call from the server
function successMove(returnData, status)
{
    //game over nobody wins
    if(returnData.turnsTaken == 30)
    {
        //update user and clear inputs
        let status = "Game Over";
        $("#status").html(status);
        $("#pull").hide();
        $("#player1").val("");
        $("#player2").val("");
        return;
    }

    //if we pass this positon player 2 pulled player 1 far enough and won
    if(returnData.current >= 42)
    {
        //update user
        let status = "Player 2 Wins";
        $("#status").html(status);
        $("#pull").hide();
        $(`#${currentPos}`).html("<img src='tugofwar.png' alt='Tug of War'>");
        return;
    }
    //player 1 pulled p2 far enough and won
    if(returnData.current<= 22)
        {
            //update user
            let status = "Player 1 Wins";
            $("#status").html(status);
            $("#pull").hide();
            $(`#${currentPos}`).html("<img src='tugofwar.png' alt='Tug of War'>");
            return;
        }
    console.log(returnData);
    //p1 turn
    if(returnData.playerTurn)
    {
        //update user with who's turn and new location
        let status = `${returnData.p1}'s turn. Moved to ${returnData.current}`;
        $("#status").html(status);
    }
    //p2 turn
    if(!returnData.playerTurn)
    {
        //update user with who's turn and new location
        let status = `${returnData.p2}'s turn. Moved to ${returnData.current}`;
        $("#status").html(status);
    }
        //set the new player positions
        currentPos = returnData.current;
        previousPos = returnData.previous;
        console.log("new prev:" + previousPos);
        console.log("new curr:" + currentPos);
        //clear old spaces and set the new one
        $(`#${previousPos}`).html("");
        $(`#${currentPos}`).html("");
        $(`#${currentPos}`).html("<img src='tugofwar.png' alt='Tug of War'>");
        console.log("displacement: " + returnData.displacement);
        console.log("turns" +returnData.turnsTaken);
}