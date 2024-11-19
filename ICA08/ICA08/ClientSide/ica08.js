$(()=>{
//onload
Ajax("http://localhost:5217/retrieve","GET",{},"JSON",successFunc,errorFunc);
});
//end of onload function
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

//function to processs succesful ajax call
function successFunc(returnData, status)
{
    //check if there are any records
    console.log(returnData.studentList);

    //start building table
    let table = "<table><tr><th>Get Students</th><th>Student Id</th><th>First Name</th><th>Last Name</th><th>School Id</th></tr>";

    //iterate through collectin of studentList
    for (let i = 0; i < returnData.studentList.length; i++)
    {
        //build table row with data returned
        table += `<tr><td><button type='button' id='${returnData.studentList[i].studentId}'>Get Class Info</button></td>`;
        table += `<td>${returnData.studentList[i].studentId}</td>`;
        table += `<td>${returnData.studentList[i].firstName}</td>`;
        table += `<td>${returnData.studentList[i].lastName}</td>`;
        table += `<td>${returnData.studentList[i].schoolId}</td></tr>`;
    }
    table += "</table>";

    //update html with the table
    $(".studentTable").html(table);

    //display number of records retrieved
    $("#studentRow").html(`Retrieved: ${returnData.studentList.length} student records`);

    //add event listener to the button
    $(".studentTable").on("click", "button", function(){

        //get the student id from the button
        let studentId = $(this).attr("id");
        console.log(studentId);

        //make ajax call
        Ajax("http://localhost:5217/retrieveClass","POST",{studentId:studentId},"JSON",successClass,errorFunc);
    }
    );
}

//function to process successful ajax call for class info
function successClass(data,status)
{
    console.log(data);

    //check if there was any records returned
    if(data.classList.length == 0)
    {
        $("#classRow").html(`No records found`);
    }
    else
    {
        //start building table
        let table = "<table><tr><th>Class Id</th><th>Class Desc</th><th>Days</th><th>Start Date</th><th>Instructor Id</th><th>Instructor First Name</th><th>Instructor Last Name</th></tr>";
        
        //iterate through collection of classList
        for(let i = 0; i < data.classList.length; i++)
        {
            //build table row with data returned
            table += `<td>${data.classList[i].classId}</td>`;
            table += `<td>${data.classList[i].classDesc}</td>`;
            table += `<td>${data.classList[i].days}</td>`;
            table += `<td>${data.classList[i].startDate}</td>`;
            table += `<td>${data.classList[i].instructorId}</td>`;
            table += `<td>${data.classList[i].instructorFirstName}</td>`;
            table += `<td>${data.classList[i].instructorLastName}</td></tr>`;
        }
        table += "</table>";

        //update html with the table
        $(".classTable").html(table);

        //display number of records retrieved
        $("#classRow").html(`Retrieved: ${data.classList.length} class records`);
    }
    
}