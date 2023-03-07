document.getElementById("userName").addEventListener("input", function (e) {
    let nameValue = document.getElementById("userName").value;
    if (nameValue === "") {
        document.getElementById("loginBtn").disabled = true;
        document.getElementById("warn").innerHTML = '<p>Username required!</p>';
    }
    else {
        document.getElementById("loginBtn").disabled = false;
        document.getElementById("warn").innerHTML = '';
    }
});

document.getElementById("loginBtn").disabled = true;