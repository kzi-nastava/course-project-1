let doctorsUri = "https://localhost:7195/api/Doctor/search"

currentTime();
let user = JSON.parse(sessionStorage.getItem("user"));
console.log(user);
setNameOnCorner(user);


let doctors;
let searchBtn = document.getElementById("search-btn");
let name = document.getElementById("name-search");
let surname = document.getElementById("surname-search");
let specialization = document.getElementById("specialization-search");
let sortParam = document.getElementById("sort-params");

searchBtn.addEventListener("click", function (e) {
    let params = "?";
    if(name.value != "") params += "Name=" + name.value + "&";
    if(surname.value != "") params += "Surname=" + surname.value + "&";
    if(specialization.value != "") params += "Specialization=" + specialization.value + "&";
    params += "SortParam=" + sortParam.value;
    console.log(doctorsUri + params);
    let getDoctorsRequest = new XMLHttpRequest();
    getDoctorsRequest.open('GET', doctorsUri + params);

    getDoctorsRequest.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                doctors = JSON.parse(getDoctorsRequest.responseText);
                doctorSelect.innerHTML = "";
                doctors.forEach(doctor => {
                    makeDoctorCard(doctor);
                });
            } else {
                alert("Greska prilikom ucitavanja doktora.")
            }
        }
    }

    getDoctorsRequest.send();
});





let doctorSelect = document.getElementById("doctor-select");


function makeDoctorCard(doctor) {
    let card = document.createElement('div');
    card.classList.add('doctor-card');

    card.onclick = function () {
        selectedDoctorId = doctor.id
    }

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