// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect([0, 2000, 10000, 30000, 60000, 120000]) // delay before attempts in ms
    .build()

var pos = ""

// Start connection
connection.start().then(function () {
    sendLoc()
}).catch(function (err) {
    return console.error(err.toString());
});

// function to get Location
function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            pos = `Latitude: ${position.coords.latitude}, longitude: ${position.coords.longitude}`;
        });
    }
    else {
        pos = "Geolocation niet ondersteund door browser.";
    }
    return pos;
}

// Send location to server
function sendLoc() {
    let message = getLocation();
    // Invoke de functie "SendMessage" op de Hub server
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });

    setTimeout(sendLoc, 10000)
}

// Uitvoeren als een "ReceiveMessage" type message komt van de server
connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li)
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you
    // should be aware of possible script injection concerns.
    li.textContent = `Location [ ${message} ]`;
});
