$(()=>{
//start of onload
Ajax("http://localhost:5033/","GET",{},"JSON",success,errorFunc);
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
function success(data,status)
{
    console.log(data);
    let locations = "<select id='location'>";
    if (data.locations) {
        for (let i = 0; i < data.locations.length; i++)
        {
            locations += `<option value='${data.locations[i].locationId}'>${data.locations[i].locationName}</option>`;
        }
    }
    locations += "</select>";
    $("#locationRow").html(locations);

    ("#location").on("change", function(){
        let customerId = $("#customerNum").val();
        let locationId = $("#location").val();
        if(customerId == "" || locationId == "")
        {
            alert("Please enter a customer number and select a location");
            return;
        }
        let data = {
            "customerId": customerId,
            "locationId": locationId
        }
        Ajax("http://localhost:5033/customer","POST",data,"JSON",successFunc,errorFunc);
    });
}
function successFunc(data,status)
{
    console.log(data);
}