// Vars used in document
var gameTimeH = 0;
var gameTimeM = 0;
var gameTimeSec = 0;

var playerTimeM = 0;
var playerTimeSec = 0;

var playerName = document.getElementById("player").textContent;

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
    [gameTimeH, gameTimeM, gameTimeSec] = [parseInt(gameTimeH), parseInt(gameTimeM), parseInt(gameTimeSec)];
    [playerTimeM, playerTimeSec] = [parseInt(playerTimeM), parseInt(playerTimeSec)];
});