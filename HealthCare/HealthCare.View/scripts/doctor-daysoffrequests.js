daysOffURI = "https://localhost:7195/api/DaysOffRequest/byPatient"

let getRoomsRequest = new XMLHttpRequest();
getRoomsRequest.open('GET', roomsUri); 

getRoomsRequest.onreadystatechange = function () {
    if (this.readyState === 4) {
        if (this.status === 200) {
            rooms = JSON.parse(getRoomsRequest.responseText);
            

        } else {
            alert("Greska prilikom ucitavanja soba.")
        }
    }
}
getRoomsRequest.send();