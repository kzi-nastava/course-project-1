let scheduleUrl = "https://localhost:7195/api/Appointment/schedule"

let scheduleRequest = new XMLHttpRequest();
let scheduleTable = document.getElementById("schedule-tbody");

let doctorId = sessionStorage.getItem("userId");


scheduleRequest.onreadystatechange = function (e) {
    if (this.readyState == 4) {
        if (this.status == 200) {
            let appointments = JSON.parse(this.responseText);
            console.log(appointments);
            showSchedule(appointments);
        } else {
            alert("Can't get your schedule at the moment :(")
        }
    }
}

scheduleRequest.open("GET", scheduleUrl.concat("?DoctorId=" + doctorId + "&Date=" + getCurrentDate() + "&ThreeDays=true"));
scheduleRequest.send();

function showSchedule(appointments) {
    showScheduleHeader();

    for (let appointmentId in appointments) {
        let appointment = appointments[appointmentId];

        let row = document.createElement("tr");
        
        let patient = document.createElement("td");
        patient.innerText = appointment.patient;

        let room = document.createElement("td");
        room.innerText = appointment.roomName;

        let startTime = document.createElement("td");
        startTime.innerText = appointment.startTime;

        let duration = document.createElement("td");
        duration.innerText = appointment.duration;

        let type = document.createElement("td");
        type.innerText = appointment.type.toLowerCase();

        row.appendChild(patient);
        row.appendChild(room);
        row.appendChild(startTime);
        row.appendChild(duration);
        row.appendChild(type);
        scheduleTable.appendChild(row);

        if (type.innerText == "examination") {
            let startBtn = document.createElement("button");
            startBtn.innerText = "start";
            startBtn.classList.add("confirm-btn");
            row.appendChild(startBtn);

            startBtn.addEventListener("click", function (e) {
                e.stopPropagation();

                alert(appointment.id);
            });
        } 

    }
}

function showScheduleHeader() {
    let tableHeader = document.getElementById("schedule-header");
    tableHeader.classList.remove("hidden");
}

function getCurrentDate() {
    let today = new Date();

    let dd = today.getDate();
    let mm = today.getMonth() + 1;
    let yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    
    return yyyy + '-' + mm + '-' + dd + "T00:00:00";
}

let getDoctorUri = "https://localhost:7195/api/Doctor/doctorId=";
let userId = sessionStorage.getItem("userId");
let days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]


currentTime();


let getDoctorRequest = new XMLHttpRequest();
getDoctorRequest.open('GET', getDoctorUri + userId);
getDoctorRequest.onreadystatechange = function () {
    if (this.readyState === 4) {
        if (this.status === 200) {
            let user = JSON.parse(getDoctorRequest.responseText);
            console.log(user);
            fillLabels(user);
        } else {
            alert("Error occured while trying to get doctor's data.");
        }
    }
}
getDoctorRequest.send();


function fillLabels(user)
{
    let smallName = document.getElementById("smallName");
    smallName.innerText = user.name + ' ' + user.surname;

    let welcomeName = document.getElementById("welcome-name");
    welcomeName.innerText = "Dr. " + user.name + "!";
}


function currentTime() {
    let date = new Date(); 
    let hh = date.getHours();
    let mm = date.getMinutes();
    let ss = date.getSeconds();
    let session = "AM";
  
    if(hh == 0){
        hh = 12;
    }
    if(hh > 12){
        hh = hh - 12;
        session = "PM";
     }
  
     hh = (hh < 10) ? "0" + hh : hh;
     mm = (mm < 10) ? "0" + mm : mm;
     ss = (ss < 10) ? "0" + ss : ss;
      
     let time = hh + ":" + mm + ":" + ss + " " + session;
  
    document.getElementById("clock").innerText = time; 
    document.getElementById("date").innerText = date.toLocaleDateString();
    document.getElementById("year").innerText = days[date.getDay() - 1]; 
    let t = setTimeout(function(){ currentTime() }, 1000);
  }
