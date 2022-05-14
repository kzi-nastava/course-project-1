currentTime();
let user = JSON.parse(sessionStorage.getItem("user"));
console.log(user);
setNameOnCorner(user)