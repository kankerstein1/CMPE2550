let menuList= {};
$(()=>{
    //onload

    //url for welcome message
    let url ="http://localhost:5013/get";
    let welcome = "welcome";

    //data object
    let data = {
        welcome: welcome
    }

    //ajax calls for onload to populate welcome message and the order location, menu and prices
    Ajax(url,"GET",data,"HTML",SuccessWelcome,errorFunc);
    Ajax("http://localhost:5013/locations","POST",data,"JSON",SuccessLocation,errorFunc);
    Ajax("http://localhost:5013/menu","POST",data,"JSON",SuccessMenu,errorFunc);
    Ajax("http://localhost:5013/prices","POST",data,"JSON",SuccessPrice,errorFunc);

    //event handler for clicking submit button
    $("#submit").on("click",function()
    {
        //returns false in backend?????
        let name = $("#name").val();

        //store order details to populate data object
        let location = $("#locate").val();
        let item = $("#items").val();
        let quantity = $("#amount").val();
        let payment = $("#payment").val();
        console.log(item);

        //checking for valid inputs for form handling
        if(name = "" || location == "" || item == "" || quantity == "" || payment == "")
        {
            alert("Please fill out all fields");
            return;
        }

        //checking for valid quantity
        if(quantity <= 0)
        {
            alert("Please enter a valid quantity");
            return;
        }

        //order url
        let url = "http://localhost:5013/order";

        //order data object
        let data = {
            "postName": `${$("#name").val()}`,
            "postLocation": location,
            "postItem": item,
            "postQuantity": quantity,
            "postPayment": payment
        }

        //make ajax call
        Ajax(url,"POST",data,"JSON",SuccessOrder,errorFunc);
    })
    
})
///end of onload
// errorHandler is function to handle errors
function errorFunc(ajaxReq, ajaxStatus, errorThrown)
{
    console.log("Error");
    console.log(errorThrown);

}

//function to processs welcome ajax call
function SuccessWelcome(returnData, status)
{
    $("#welcome").html(returnData);
}

//function to process succesfull location ajax call
function SuccessLocation(returnData, status)
{
    console.log(returnData);
    //populating select tag with data returned from server
    let locations = "<br><select id='locate'>";
    for (let i=0; i<returnData.length; i++)
    {
        locations += "<option value='"+returnData[i] + "'>"+returnData[i] + "</option>";
    }
    locations += "</select>";

    //append location to the div
    $("#location").append(locations);
}

//function to handle successive menu data returned from server
function SuccessMenu(returnData, status)
{
    //start of ordered list 
    let menu = "<ol>"

    //populate list with returned items
    for(let item in returnData)
    {
        menuList[item] = returnData[item];
        menu += "<li>"+returnData[item]+"</li>";
    }
    menu += "</ol>";

    //append menu to the menu div
    $("#menu").append(menu);
}

//function for populating the prices
function SuccessPrice(returnData, status)
{
    console.log(menuList);
    let prices = "";
    //populate options with the menu items
    for(let i = 0; i < returnData.length; i++)
    {
        prices += "<option value='"+menuList[i] + "'>"+menuList[i] +"</option>";
    }
    //onsole.log(prices);
    //append items to the form
    $("#items").append(prices);
}

//function to post the order sent to the server and update the user with the details
function SuccessOrder(returnData, status)
{
    //building order done string
    let orderDone = "";

    //populating header with name to thank user
    orderDone +=`<h2>Thank You ${returnData.name} For The Order</h2><br>`;

    //building ordered list with details of the order
    orderDone +="<ol>";
    orderDone +=`<li>${returnData.location}</li>`;
    orderDone +=`<li>${returnData.item}</li>`;
    orderDone +=`<li>${returnData.quantity}</li>`;
    orderDone +=`<li>${returnData.payment}</li>`;
    orderDone += "</ol><br>";

    //adding the time it will take order to be picked up
    orderDone += `<p>Your order will be ready for pickup in ${returnData.time} minutes`;
    //append details to the page
    $("#orderDone").html(orderDone);
}

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