let menuList= {};
$(()=>{
    //onload
    let url ="http://localhost:5013/get";
    let welcome = "welcome";
    let data = {
        welcome: welcome
    }
    Ajax(url,"GET",data,"HTML",SuccessWelcome,errorFunc);
    Ajax("http://localhost:5013/locations","POST",data,"JSON",SuccessLocation,errorFunc);
    Ajax("http://localhost:5013/menu","POST",data,"JSON",SuccessMenu,errorFunc);
    Ajax("http://localhost:5013/prices","POST",data,"JSON",SuccessPrice,errorFunc);
    $("#submit").on("click",function()
    {
        let name = $("#name").val();
        let location = $("#locate").val();
        let item = $("#items").val();
        let quantity = $("#amount").val();
        let payment = $("#payment").val();
        console.log(name);
        if(name = "" || location == "" || item == "" || quantity == "" || payment == "")
        {
            alert("Please fill out all fields");
            return;
        }
        if(quantity <= 0)
        {
            alert("Please enter a valid quantity");
            return;
        }

        let url = "http://localhost:5013/order";
        let data = {
            postName: name,
            postLocation: location,
            postItem: item,
            postQuantity: quantity,
            postPayment: payment
        }
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
function SuccessWelcome(returnData, status)
{
    $("#welcome").html(returnData);
}
function SuccessLocation(returnData, status)
{
    console.log(returnData);
    let locations = "<br><select id='locate'>";
    for (let i=0; i<returnData.length; i++)
    {
        locations += "<option value='"+returnData[i] + "'>"+returnData[i] + "</option>";
    }
    locations += "</select>";
    $("#location").append(locations);
}
function SuccessMenu(returnData, status)
{
    let menu = "<ol>"
    for(let item in returnData)
    {
        menuList[item] = returnData[item];
        menu += "<li>"+returnData[item]+"</li>";
    }
    menu += "</ol>";
    $("#menu").append(menu);
}
function SuccessPrice(returnData, status)
{
    console.log(menuList);
    let prices = "";
    for(let i = 0; i < returnData.length; i++)
    {
        prices += "<option value='"+returnData[i] + "'>"+menuList[i] +"</option>";
    }
    //onsole.log(prices);
    $("#items").append(prices);
}
function SuccessOrder(returnData, status)
{
    console.log(returnData);
}
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