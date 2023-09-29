// Vars used in document
var playerTimeM = 0;
var playerTimeSec = 0;

var playerName = document.getElementById("player").textContent;

function displayTime(timeInSec, id) {
    let hours = Math.floor(timeInSec / 3600);
    let minutes = Math.floor((timeInSec / 60) % 60);
    let seconds = Math.floor((timeInSec % 60));
    let formattedTime = id === "gameTime" ? padZero(hours) + ":" : ""
    formattedTime += padZero(minutes) + ":" + padZero(seconds);
    document.getElementById(id).textContent = formattedTime;
}

function padZero(num) {
    return num.toString().padStart(2, "0");
}

function countdown() {
    gameTimeSec--;
    playerTimeSec--;
    displayTime(gameTimeSec, "gameTime");
    displayTime(playerTimeSec, "playerTime");
}

// [Coordinate], zoomlevel
var map = L.map('map').setView([51.066729, 3.630271], 14);

var redIcon = new L.Icon({
    iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-red.png',
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
});

var greenIcon = new L.Icon({
    iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
    shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/0.7.7/images/marker-shadow.png',
    iconSize: [25, 41],
    iconAnchor: [12, 41],
    popupAnchor: [1, -34],
    shadowSize: [41, 41]
});

L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
    attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

var marker = L.marker([51.066729, 3.630271], { icon: redIcon }).addTo(map);
marker.bindPopup("<b>Jager</b>");

var runnerMarker = L.marker([51.0669, 3.614266], { icon: greenIcon }).addTo(map);
runnerMarker.bindPopup("<b>Loper</b>");

// Hub and SignalR functions
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/jachtseizoenHub")
    .withAutomaticReconnect([0, 2000, 10000, 30000, 60000, 120000]) // delay before attempts in ms
    .build()

// Start the connection
connection.start().then(function () {
    connection.invoke("RetrieveTimeData", "jager1");
});

// Uitvoeren als er een "FirstStart" type message komt van de server
connection.on("FirstStart", function (timeData) {
    console.log("Time data retrieved");
    console.log(timeData[1]);
    [gameTimeH, gameTimeM, gameTimeSec] = timeData[0].split(":");
    [playerTimeM, playerTimeSec] = timeData[1].split(":");
    gameTimeSec = parseInt(gameTimeH) * 3600 + parseInt(gameTimeM) *60 + parseInt(gameTimeSec);
    playerTimeSec = parseInt(playerTimeM) * 60 + parseInt(playerTimeSec);
    setInterval(countdown, 1000);
});

// TODO:
// 1. Fix counters
// 2. Player counter = 0 --> Send loc + get new time data
// 3. Update map