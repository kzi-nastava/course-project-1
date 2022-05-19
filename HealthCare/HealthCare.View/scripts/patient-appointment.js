let doctorsUri = "https://localhost:7195/api/Doctor"
let roomsUri = "https://localhost:7195/api/Room"
let examinationUri = "https://localhost:7195/api/Examination/create"
let examinationDeleteUri = "https://localhost:7195/api/Examination/delete"
let getPatientUri = "https://localhost:7195/api/Patient/patientId=";


currentTime();
let user = JSON.parse(sessionStorage.getItem("user"));
console.log(user);
setNameOnCorner(user);

const yearSelect = document.getElementById("yearSelect");
const monthSelect = document.getElementById("monthSelect");
const daySelect = document.getElementById("daySelect");
const hourSelect = document.getElementById("hourSelect");
const minuteSelect = document.getElementById("minuteSelect");

let appointmentBox = document.getElementById('examination-select');
let doctorSelect = document.getElementById("doctor-select");

const months = ['January', 'February', 'March', 'April', 'May', 'June',
'July', 'August', 'September', 'October', 'November', 'December'];

(function populateMonths(){
    for(let i = 0; i < months.length; i++)
    {
        const option = document.createElement('option');
        option.textContent = months[i];
        option.value = i + 1;
        monthSelect.appendChild(option);
    }
    monthSelect.value = "Month";
})();

function populateDays(month) {
    while(daySelect.firstChild)
    {
        daySelect.removeChild(daySelect.firstChild);
    }
    let dayNum;
    if(month === '1' || month === '3' || month === '5' || 
    month === '7' || month === '8' || month === '10' ||
     month === '12')
    {
        dayNum = 31;
    }   
    else if(month === '4' || month === '6' || month === '9' || 
    month === '11')
    {
        dayNum = 30;
    }   
    else {
        dayNum = 29;
    }
    for(let i = 1; i <= dayNum; i++)
    {
        const option = document.createElement('option');
        option.textContent = i;
        option.value = i;
        daySelect.appendChild(option);
    }
}

function populateYears()
{
    let year = new Date().getFullYear();
    for(let i = 0; i <= 5; i++)
    {
        const option = document.createElement('option');
        option.textContent = year + i;
        option.value = year + i;
        yearSelect.appendChild(option);
    }
}

populateDays(monthSelect.value);
populateYears();

yearSelect.onchange = () => {
    populateDays(monthSelect.value);
}

monthSelect.onchange = () => {
    populateDays(monthSelect.value);
}

function populateHours () 
{
    for(let i = 0; i < 24; i++)
    {
        const option = document.createElement('option');
        option.textContent = i;
        option.value = i;
        hourSelect.appendChild(option);
    }
}

populateHours();

function populateMinutes () 
{
    for(let i = 0; i < 60; i+=15)
    {
        const option = document.createElement('option');
        option.textContent = i;
        option.value = i;
        minuteSelect.appendChild(option);
    }
}

populateMinutes();


let rooms;

let getRoomsRequest = new XMLHttpRequest();
getRoomsRequest.open('GET', roomsUri); 

getRoomsRequest.onreadystatechange = function () {
    if (this.readyState === 4) {
        if (this.status === 200) {
            rooms = JSON.parse(getRoomsRequest.responseText);
            getDoctorsRequest.send();

        } else {
            alert("Greska prilikom ucitavanja soba.")
        }
    }
}
getRoomsRequest.send();

let doctors; 

let getDoctorsRequest = new XMLHttpRequest();
getDoctorsRequest.open('GET', doctorsUri); 

getDoctorsRequest.onreadystatechange = function () {
    if (this.readyState === 4) {
        if (this.status === 200) {
            doctors = JSON.parse(getDoctorsRequest.responseText);
            doctors.forEach(doctor => {
                makeDoctorCard(doctor);
            });
            for(let i = 0; i < user.examinations.length; i++)
            {
                if(user.examinations[i].isDeleted == false)
                    populateAppointments(user.examinations[i]);
            }
        } else {
            alert("Greska prilikom ucitavanja doktora.")
        }
    }
}

function reloadUser()
{
    let getPatientRequest = new XMLHttpRequest();
    getPatientRequest.open('GET', getPatientUri + user.id);
    getPatientRequest.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                let reloadedUser = JSON.parse(getPatientRequest.responseText);
                console.log(reloadedUser);
                sessionStorage.setItem("user", JSON.stringify(reloadedUser));
                user = reloadedUser;
                appointmentBox.innerHTML = "";
                for(let i = 0; i < user.examinations.length; i++)
                {
                    if(user.examinations[i].isDeleted == false)
                        populateAppointments(user.examinations[i]);
                }
            } else {
                alert("Greska prilikom ucitavanja korisnika.");
            }
        }
    }
    getPatientRequest.send();
}

let selectedDoctorId;
let examinationIdForDelete;

function makeDoctorCard(doctor)
{
    let card = document.createElement('div');
    card.classList.add('doctor-card');

    card.onclick = function() {selectedDoctorId = doctor.id}

    let imageDiv = document.createElement('div');
    imageDiv.classList.add('image-doctor');
    let image = document.createElement('img');
    image.src = "../img/profile_pic.png";
    image.alt = "profile_picture";
    image.classList.add("user-nav__box--img");
    imageDiv.appendChild(image);

    let name = document.createElement('div');
    card.classList.add('doctor-name');
    let nameSurname = document.createElement('p');
    nameSurname.classList.add('name-surname');
    nameSurname.innerText = doctor.name + " " + doctor.surname;
    let specialization = document.createElement('p');
    specialization.classList.add('specialization');
    specialization.innerText = doctor.specialization.name;
    let mail = document.createElement('p');
    mail.classList.add('mail');
    mail.innerText = doctor.email;
    name.appendChild(nameSurname);
    name.appendChild(specialization);
    name.appendChild(mail);

    card.appendChild(imageDiv);
    card.appendChild(name);
    card.setAttribute("doctorId", doctor.id);
    card.setAttribute("tabindex", -1);

    doctorSelect.appendChild(card);

}



function populateAppointments(appointment)
{
    let examinationBox = document.createElement("div");
    examinationBox.classList.add("examination-box");
    
    let examination = document.createElement("div");
    examination.classList.add("examination");
    
    let examinationDatetimeBox = document.createElement("div");
    examinationDatetimeBox.classList.add("examination-datetime-box");
    
    let examinationDatetimeIcon = document.createElement("div");
    examinationDatetimeIcon.classList.add("examination-datetime-icon");
    
    let datetimeIcon = document.createElement("i");
    datetimeIcon.classList.add("fa-solid");
    datetimeIcon.classList.add("fa-calendar-days");
    
    
    
    let examinationDatetime = document.createElement("div");
    examinationDatetime.classList.add("examination-datetime");
    
    let date = document.createElement("p");
    date.innerText = appointment.startTime.split("T")[0];
    
    
    let time = document.createElement("p");
    timelist = appointment.startTime.split("T")[1].split("Z")[0].split(":");
    time.innerText = timelist[0] + ":" + timelist[1] + "h";
    
    examinationDatetimeIcon.appendChild(datetimeIcon);
    
    examinationDatetime.appendChild(date);
    examinationDatetime.appendChild(time);
    
    examinationDatetimeBox.appendChild(examinationDatetimeIcon);
    examinationDatetimeBox.appendChild(examinationDatetime);
    
    examination.appendChild(examinationDatetimeBox);
    
    let examinationDoctorBox = document.createElement("div");
    examinationDoctorBox.classList.add("examination-doctor-box");

    let examinationDoctorIcon = document.createElement("div");
    examinationDoctorIcon.classList.add("examination-datetime-icon");
    
    let doctorIcon = document.createElement("i");
    doctorIcon.classList.add("fa-solid");
    doctorIcon.classList.add("fa-user-doctor");
    
    examinationDoctorIcon.appendChild(doctorIcon);
    examinationDoctorBox.appendChild(examinationDoctorIcon);
        
    let examinationDoctorInfo = document.createElement("div");
    examinationDoctorInfo.classList.add("examination-doctor-info");


    let name = document.createElement("p");

    doctors.forEach(doctor => {
        if(doctor.id === appointment.doctorId)
        {
            name.innerText = "Dr " + doctor.name + " " + doctor.surname;
        }
    });

    
    examinationDoctorInfo.appendChild(name);
    
    let room = document.createElement("p");

    rooms.forEach(r => {
        if(r.id === appointment.roomId)
        {
            room.innerText = "Room " + r.roomName;
        }
    });
    
    examinationDoctorInfo.appendChild(room);
    
    examinationDoctorBox.appendChild(examinationDoctorInfo);
    
    examination.appendChild(examinationDoctorBox);
    
    examinationBox.appendChild(examination);
    
    let examinationButtons = document.createElement("div");
    examinationButtons.classList.add("examination-buttons");

    let editBtn = document.createElement("button");
    editBtn.classList.add("editBtn");
    editBtn.innerText += "        Edit";

    let editIcon = document.createElement("i");
    editIcon.classList.add("fa-solid");
    editIcon.classList.add("fa-pencil");
    //editBtn.innerHTML = editIcon;
    
    examinationButtons.appendChild(editBtn);
    
    let deleteBtn = document.createElement("button");
    deleteBtn.classList.add("deleteBtn");
    //deleteBtn.innerHTML = <i class="fa-solid fa-trash"></i>;
    deleteBtn.innerText += "        Delete";

    deleteBtn.onclick = function() {deleteAppointment(appointment.id)};

    examinationButtons.appendChild(deleteBtn);
    
    examinationBox.appendChild(examinationButtons);

    appointmentBox.appendChild(examinationBox);
    
    
    
}

function deleteAppointment(id)
{
    console.log(id);
    let deleteExaminationDTO = {
        examinationId : id,
        patientId : user.id,
        isPatient : true
    }
    let deleteExaminationRequest = new XMLHttpRequest();
    deleteExaminationRequest.open('PUT', examinationDeleteUri); 
    deleteExaminationRequest.setRequestHeader('Content-Type', 'application/json');
    deleteExaminationRequest.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                let examination = JSON.parse(deleteExaminationRequest.responseText);
                console.log(examination)
                user.examinations.push(examination);
                populateAppointments(examination);
                reloadUser();
            } else {
                alert("Greska prilikom brisanja pregleda.")
            }
        }
    }
    deleteExaminationRequest.send(JSON.stringify(deleteExaminationDTO));
}


let submitBtn = document.getElementById("submitBtn");

submitBtn.addEventListener("click", function(e) {
    let day = formatDate(daySelect.value);
    let month = formatDate(monthSelect.value);
    let year = formatDate(yearSelect.value);
    let hours = formatDate(hourSelect.value);
    let minutes = formatDate(minuteSelect.value);
    let date = year + "-" + month + "-" + day + "T" + hours + ":" + minutes + ":00.000Z";
    
    let examination =  {
        "doctorId": selectedDoctorId,
        "examinationId": 0,
        "patientId": user.id,
        "startTime": date,
        "isPatient": true
    }
    console.log(JSON.stringify(examination));

    
    let makeExaminationRequest = new XMLHttpRequest();
    makeExaminationRequest.open('POST', examinationUri); 
    makeExaminationRequest.setRequestHeader('Content-Type', 'application/json');
    makeExaminationRequest.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                let examination = JSON.parse(makeExaminationRequest.responseText);
                reloadUser()
            } else {
                alert("Greska prilikom rezervisanja pregleda.")
            }
        }
    }
    makeExaminationRequest.send(JSON.stringify(examination));
});
     
function formatDate(param)
{
    if(param < 10)
    {
        param = "0" + param;
    }
    return param;
}

