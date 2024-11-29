let eButton = 0;

$(()=>{
//onload
Ajax("http://localhost:5217/retrieve","GET",{},"JSON",successFunc,errorFunc);
Ajax("http://localhost:5217/getClasses","GET",{},"JSON",successClasses,errorFunc);
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
    let table = "<table><tr><th>Get Students</th><th>Student Id</th><th>First Name</th><th>Last Name</th><th>School Id</th><th>Action</th></tr>";

    //iterate through collectin of studentList
    for (let i = 0; i < returnData.studentList.length; i++)
    {
        //build table row with data returned
        table += `<tr><td><button type='button' name='retrieve' id='${returnData.studentList[i].studentId}'>Get Class Info</button></td>`;
        table += `<td>${returnData.studentList[i].studentId}</td>`;
        table += `<td>${returnData.studentList[i].firstName}</td>`;
        table += `<td>${returnData.studentList[i].lastName}</td>`;
        table += `<td>${returnData.studentList[i].schoolId}</td>`;
        table += `<td><button type='button' id='${returnData.studentList[i].studentId}' name='delete'>Delete</button>`;
        table += `<button type='button' id='${returnData.studentList[i].studentId}' name='edit'>Edit</button></td></tr>`;
    }
    table += "</table>";

    //update html with the table
    $(".studentTable").html(table);

    //display number of records retrieved
    $("#studentRow").html(`Retrieved: ${returnData.studentList.length} student records`);

    //add event listener to the button
    $(".studentTable").on("click", "[name=retrieve]", function(){

        //get the student id from the button
        let studentId = $(this).attr("id");
        console.log(studentId);

        //make ajax call
        Ajax("http://localhost:5217/retrieveClass","POST",{studentId:studentId},"JSON",successClass,errorFunc);
    }
    );
    $(".studentTable").on("click", "[name=delete]", function(){
            
            //get the student id from the button
            let studentId = $(this).attr("id");
            console.log(studentId);
    
            //make ajax call
            Ajax(`http://localhost:5217/delete/${studentId}`,"DELETE",{},"JSON",successDelete,errorFunc);
    });
    $(".studentTable").on("click", "[name=edit]", function(){
        
        eButton = this;

        let tablerow = $(eButton).closest('tr').children('td');

        $(".studentTable").off("click", "[name=delete]");
        $(".studentTable").off("click", "[name=edit]");


        let editfName = tablerow[2].innerHTML;
        tablerow[2].innerHTML = "<input type='text' value=''>";
        tablerow[2].children[0].value = editfName;


        let editLName = tablerow[3].innerHTML;
        tablerow[3].innerHTML = "<input type='text' value=''>";
        tablerow[3].children[0].value = editLName;

        let editSchoolId = tablerow[4].innerHTML;
        tablerow[4].innerHTML = "<input type='text' value=''>";
        tablerow[4].children[0].value = editSchoolId;

        let editButtons = table[5].innerHTML;
        tablerow[5].innerHTML = "<button type='button' id='update'>Update</button><button type='button' id='cancel'>Cancel</button>";

        

        $("#update").on("click", function(){
            //get the student id from the button
        let studentId = $(this).parent().prev().prev().prev().prev().text();
        let fName = $(this).parent().prev().prev().prev().children().val();
        let lName = $(this).parent().prev().prev().children().val();
        let schoolId = $(this).parent().prev().children().val();

        console.log(studentId);

        let data = {
            studentId:studentId,
            firstName:fName,
            lastName:lName,
            schoolId:schoolId
        };

        //make ajax call
        Ajax("http://localhost:5217/edit","PUT",data,"JSON",UpdateFunc,errorFunc);
        });
       $("#cancel").on("click", function(){
        Ajax("http://localhost:5217/retrieve","GET",{},"JSON",successFunc,errorFunc);
       });
        
        
});
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
function successClasses(data,status)
{
    console.log(data);
    let form = "<form><label for='fName'>Student First Name:</label><input type='text' id='firstName' name='fName'<br>";
    form += "<label for='lName'>Student Last Name:</label><input type='text' id='lastName' name='lName'<br>";
    form += "<label for='sId'>School Id:</label><input type='text' id='schoolId' name='sId'<br>";
    form += "<label for='cId'>Class Id:</label><select id='classId' name='cId' multiple='multiple'>";
    for(let i = 0; i < data.classList.length; i++)
    {
        form += `<option value='${data.classList[i].classId}' >${data.classList[i].classDesc}</option>`;
    }
    form += "</select><br><button type='button' id='add'>Add Student</button></form>";
    $("#addStudent").html(form);    
    //check if there was any records returned
    $("#addStudent").on("click", "#add", function(){

        
        let fName = $("#firstName").val();
        let lName = $("#lastName").val();
        let sId = $("#schoolId").val();
        let cId = $("#classId").val();
        if(fName == "" || lName == "" || sId == "" || cId == "")
        {
            alert("Please fill out all fields");
            return;
        }
        console.log(fName);
        console.log(lName);
        console.log(sId);
        console.log(cId);
        Ajax("http://localhost:5217/add","POST",{firstName:fName,lastName:lName,schoolId:sId,classId:cId},"JSON",SuccessAdd,errorFunc);
    });
    
}
function SuccessAdd(data,status)
{
    console.log(data);
    $(".studentTable").off("click");
    alert(`Added ${data.studentRow} from Students and ${data.classRow} from Class2Student`);
    Ajax("http://localhost:5217/retrieve","GET",{},"JSON",successFunc,errorFunc);
}
function successDelete(data,status)
{
    console.log(data);
    alert(`Removed ${data.studentRow} from Students and ${data.classRow} from Class2Student`);
    Ajax("http://localhost:5217/retrieve","GET",{},"JSON",successFunc,errorFunc);
}
function UpdateFunc(data,status)
{
    console.log(data);
    alert(`Updated ${data.studentRow} from Students`);
    Ajax("http://localhost:5217/retrieve","GET",{},"JSON",successFunc,errorFunc);
}