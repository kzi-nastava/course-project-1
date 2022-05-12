let getPatientUri = "https://localhost:7195/api/Patient/patientId=";
let userId = sessionStorage.getItem("userId");
let days = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]


currentTime();


let getPatientRequest = new XMLHttpRequest();
getPatientRequest.open('GET', getPatientUri + userId);
getPatientRequest.onreadystatechange = function () {
    if (this.readyState === 4) {
        if (this.status === 200) {
            let user = JSON.parse(getPatientRequest.responseText);
            console.log(user);
            fillLabels(user);
        } else {
            alert("Greska prilikom ucitavanja korisnika.");
        }
    }
}
getPatientRequest.send();


function fillLabels(user)
{
    let smallName = document.getElementById("smallName");
    smallName.innerText = user.name + ' ' + user.surname;

    let bigName = document.getElementById("bigName");
    bigName.innerText = user.name + ' ' + user.surname;

    let welcomeName = document.getElementById("welcome-name");
    welcomeName.innerText = user.name + " !";

    let id = document.getElementById("userId");
    id.innerText = user.id;

    let dateOfBirth = document.getElementById("dateOfBirth");
    dateOfBirth.innerText = user.birthDate.split('T')[0];

    let mail = document.getElementById("mail");
    mail.innerText = user.email;

    let phone = document.getElementById("phone");
    phone.innerText = user.phone;

    let height = document.getElementById("height");
    height.innerText = user.medicalRecord.height;

    let weight = document.getElementById("weight");
    weight.innerText = user.medicalRecord.weight;

    let examinationNumber = document.getElementById("examination-number");
    examinationNumber.innerText = user.examinations.length;
    
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
    document.getElementById("year").innerText = days[date.getDay()]; 
    let t = setTimeout(function(){ currentTime() }, 1000);
  }
  
