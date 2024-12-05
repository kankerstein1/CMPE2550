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
//function for success
function success(data,status)
{
    console.log(data);

    //create a dropdown for the locations
    let locations = "<select id='location'>";
    
        for (let i = 0; i < data.length; i++)
        {
            locations += `<option value='${data[i].locationid}'>${data[i].locationName}</option>`;
        }
    
    locations += "</select>";

    //append the dropdown to the locationRow
    $("#locationRow").html(locations);

    //event handler for when the location is changed
    $("#location").on("change", function(){

        //get the location id
        $(".orderResponse").html("");
        $(".orderTable").html("");

        //get the customer id
        let customerId = $("#customerNum").val();
        let locationId = $("#location").val();

        console.log(locationId);
        
        //if the customer number or location is empty, alert the user
        if(customerId == "" || locationId == "")
        {
            alert("Please enter a customer number and select a location");
            return;
        }

        //send the data to the server
        let data = {
            "customerId": customerId,
            "locationId": locationId
        }
        Ajax(`http://localhost:5033/customer/${locationId}/${customerId}`,"GET",data,"JSON",successFunc,errorFunc);
    });
}
//function for success order info
function successFunc(data,status)
{
    console.log(data);

    //display the order response
    let ordername = "<h2>" + data.response + "</h2>";
    $(".orderResponse").html(ordername);

    //display the order table
    let table = "<table><tr><th>Order ID</th><th>Order Date</th><th>Payment Method</th><th>Item Name</th><th>Item Price</th><th>Item</th></tr>";
    for(let i = 0; i < data.tableData.length; i++)
    {
        table += `<tr><td>${data.tableData[i].orderId}</td>`;
        table += `<td>${data.tableData[i].orderDate}</td>`;
        table += `<td>${data.tableData[i].paymentMethod}</td>`;
        table += `<td>${data.tableData[i].itemName}</td>`
        table += `<td>${data.tableData[i].itemPrice}</td>`
        table += `<td>${data.tableData[i].itemCount}</td></tr>`;
    }
    table += "</table>";

    //append the table to the orderTable
    $(".orderTable").append(table);
}