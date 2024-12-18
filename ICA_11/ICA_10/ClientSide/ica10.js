let locationsId = 0;
let _data;
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
    locations = "";
    let places = "<select id='location'>";
    places += `<option value='default'>Select A Location</option>`;
    
        for (let i = 0; i < data.length; i++)
        {
            locations += `<option value='${data[i].locationid}'>${data[i].locationName}</option>`;
        }
    places += locations;
    places += "</select>";
    
    

    locationsArray = locations;
    //append the dropdown to the locationRow
    $("#locationRow").html(places);

    //get the items from the server
    Ajax("http://localhost:5033/items","GET",{},"JSON",function(data,status){
        console.log(data);

        //build the form for the order
        let add = "<form>";
        add += "<h2>Place An Order</h2>";
        add += "<label for='customerId'>Customer Id:</label>";
        add += "<input type='text' id='customerId' name='customerId' placeholder='Enter Your ID'>";
        add += "<label for='itemOrdered'>Item Ordered:</label>";
        add += "<select id='itemOrdered' name='itemOrdered'>";
        add += "<option value='default'>Select An Item</option>";

        //loop through the items and add them to the dropdown
        for(let i = 0; i < data.length; i++)
        {
            console.log(data[i].itemName);
            add += `<option value='${data[i].itemid}'>${data[i].itemName}</option>`;
        }

        add += "</select>"; 
        add += "<label for='quantity'>How Many:</label>";
        add += "<input type='text' id='quantity' name='quantity' placeholder='0'>";
        add += "<label for='paymentMethod'>Payment Method:</label>";
        add += "<select id='paymentMethod' name='paymentMethod'>";
        add += "<option value='default'>Select A Payment Method</option>";
        add += "<option value='Cash'>Cash</option>";
        add += "<option value='Credit'>Credit</option>";
        add += "<option value='Debit'>Debit</option>";
        add += "</select>";
        add += "<label for='location'>Location:</label>";
        add += "<select id='locationAdd' name='location'>";

        //loop through the locations and add them to the dropdown
        add += locationsArray;

        add += "</select>";
        add += "<button type='button' id='submit'>Place Order</button>";
        add += "</form>";

        //append the form to the addOrder div
        $(".addOrder").html(add);

        //event handler for when the submit button is clicked
        $("#submit").on("click", function(){
            console.log("Submit Clicked");

            //get the values from the form
            let customerId = $("#customerId").val();
            let itemOrdered = $("#itemOrdered").val();
            let quantity = $("#quantity").val();
            let paymentMethod = $("#paymentMethod").val();
            let location = $("#locationAdd").val();
            
            console.log(customerId);
            console.log(itemOrdered);
            console.log(quantity);
            console.log(paymentMethod);
            console.log(location);

            //if any of the fields are empty, alert the user
            if(customerId == "" || itemOrdered == "" || quantity == "" || paymentMethod == "" || location == "")
            {
                alert("Please fill out all fields");
                return;
            }
            //if the quantity is less than 1, alert the user
            if(customerId < 0 || itemOrdered < 0 || quantity < 1)
            {
                alert("Please enter a valid number");
                return;
            }
            if(location == "default" || paymentMethod == "default" || itemOrdered == "default")
                {
                    alert("Please select a location, payment method, and item");
                    return;
                } 
            //send the data to the server
            let data = {
                "customerId": customerId,
                "itemOrdered": itemOrdered,
                "quantity": quantity,
                "paymentMethod": paymentMethod,
                "location": location
            }
            //store the data in a global variable
            _data = data;
            Ajax("http://localhost:5033/add","POST",data,"JSON",successAdd,errorFunc);

        });
    },errorFunc);

    //event handler for when the location is changed
    $("#location").on("change", function(){

        //get the location id
        $(".orderResponse").html("");
        $(".orderTable").html("");

        //get the customer id
        let customerId = $("#customerNum").val();
        let locationId = $("#location").val();
        locationsId = parseInt(locationId);
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
    let table = "<table><tr><th>Order ID</th><th>Order Date</th><th>Payment Method</th><th>Item Name</th><th>Item Price</th><th>Item</th><th>Delete Order</th></tr>";
    for(let i = 0; i < data.tableData.length; i++)
    {
        table += `<tr><td>${data.tableData[i].orderId}</td>`;
        table += `<td>${data.tableData[i].orderDate}</td>`;
        table += `<td>${data.tableData[i].paymentMethod}</td>`;
        table += `<td>${data.tableData[i].itemName}</td>`
        table += `<td>${data.tableData[i].itemPrice}</td>`
        table += `<td>${data.tableData[i].itemCount}</td>`;
        table += `<td><button type=button id='${data.tableData[i].orderId}' name='delete'>Delete</button></td></tr>`;
    }
    table += "</table>";

    //append the table to the orderTable
    $(".orderTable").append(table);

    //event handler for when the delete button is clicked
    $("[name=delete]").on("click", function(){

        //get the order id
        let orderId = $(this).attr("id");
        console.log(orderId);  
        console.log(locationsId);

        //send the data to the server
        Ajax(`http://localhost:5033/delete/${orderId}/${locationsId}`,"DELETE",data,"JSON",deleteSuccess,errorFunc);
    });
    

}

//function for success delete
function deleteSuccess(data,status)
{
    //clear the order table
    $(".orderTable").html("");
    console.log(data);

    //display the order response
    $(".orderResponse").html(data.response);
    //display the order table
    let table = "<table><tr><th>Order ID</th><th>Order Date</th><th>Payment Method</th><th>Item Name</th><th>Item Price</th><th>Item</th><th>Delete Order</th></tr>";
    for(let i = 0; i < data.table.length; i++)
    {
        table += `<tr><td>${data.table[i].orderId}</td>`;
        table += `<td>${data.table[i].orderDate}</td>`;
        table += `<td>${data.table[i].paymentMethod}</td>`;
        table += `<td>${data.table[i].itemName}</td>`
        table += `<td>${data.table[i].itemPrice}</td>`
        table += `<td>${data.table[i].itemCount}</td>`;
        table += `<td><button type=button id='${data.table[i].orderId}' name='delete'>Delete</button></td></tr>`;
    }
    table += "</table>";

    //append the table to the orderTable
    $(".orderTable").append(table);

    // Re-attach the event handler for the delete button
    $("[name=delete]").on("click", function(){

        //get the order id
        let orderId = $(this).attr("id");
        console.log(orderId);  
        console.log(locationsId);

        //send the data to the server
        Ajax(`http://localhost:5033/delete/${orderId}/${locationsId}`,"DELETE",data,"JSON",deleteSuccess,errorFunc);
    });
}
function successAdd(data,status)
{
    //display the response
    $(".addResponse").html(data.response);

    if(data.response == "Customer Not Found")
        {
            return;
        }
    else
        {
            //make ajax to build new form in back end
            Ajax("http://localhost:5033/updateForm","POST",_data,"JSON",updateForm,errorFunc);
        }
        
}

function updateForm(data,status)
{
    console.log(data.response);
    //display the form
    if(data.response == "Customer Not Found")
        {
            return;
        }
    else
    {
        $(".addOrder").html(data.updateForm);
        $("#update").on("click", function(){

            //get the values from the form
            let orderId = $("#orderId").val();
            let customerId = $("#customerId").val();
            let itemOrdered = $("#itemOrdered").val();
            let quantity = $("#quantity").val();
            let paymentMethod = $("#paymentMethod").val();
            let location = $("#location").val();

            //if any of the fields are empty, alert the user
            if(itemOrdered == "" || quantity == "" || paymentMethod == "")
            {
                alert("Please fill out all fields");
                return;
            }
            //if the quantity is less than
            if(quantity < 1)
            {
                alert("Please enter a valid number");
                return;
            }
            //send the data to the server
            let data = {
                "orderId": orderId,
                "customerId": customerId,
                "itemOrdered": itemOrdered,
                "quantity": quantity,
                "paymentMethod": paymentMethod,
                "location": location
            }
            Ajax("http://localhost:5033/updateForm","PUT",data,"JSON",updateOrderSuccess,errorFunc);
        });
    }
    
    //event handler for successful update
    function updateOrderSuccess(data,status)
    {
        //send response to user
        $(".addResponse").html(data.response);

        //disable button
        $("#update").prop("disabled", true);
    }
}