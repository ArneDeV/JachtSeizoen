// Vars used in document
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

var markers = L.markerClusterGroup();

function displayTime(timeInSec, id) {
    let hours = Math.floor(timeInSec / 3600);
    let minutes = Math.floor((timeInSec / 60) % 60);
    let seconds = Math.floor((timeInSec % 60));
    let formattedTime = id === "gameTime" ? padZero(hours) + ":" : ""
    formattedTime += padZero(minutes) + ":" + padZero(seconds);
    //console.log(formattedTime);
    document.getElementById(id).textContent = formattedTime;
}

function padZero(num) {
    return num.toString().padStart(2, "0");
}

function countdown() {
    gameTimeSec--;
    // Pure visual, no actual function
    if (playerTimeSec > 0) {
        playerTimeSec--;
    }
    displayTime(gameTimeSec, "gameTime");
    displayTime(playerTimeSec, "playerTime");
    if (playerTimeSec === 0) {
        updateLocation();
    }
}

function updateLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            let location = [position.coords.latitude, position.coords.longitude]
            connection.invoke("RetrieveLocation", playerName, location);
        });
    }
}

function updateMarkers(playerInfo, hunterAmount, runnerAmount) {
    playerInfo = JSON.parse(playerInfo);
    
    markers.clearLayers();
    let avg_lat = 0;
    let avg_lon = 0;

    for (let i = 0; i < hunterAmount; i++) {
        let coords = [playerInfo[i].Latitude, playerInfo[i].Longitude]
        avg_lat += playerInfo[i].Latitude;
        avg_lon += playerInfo[i].Longitude;
        createMarker([playerInfo[i].Latitude, playerInfo[i].Longitude], playerInfo[i], "hunter");
    }
    for (let i = 0; i < runnerAmount; i++) {
        createMarker([playerInfo[i+hunterAmount].Latitude, playerInfo[i+hunterAmount].Longitude], playerInfo[i+hunterAmount], "runner");
        avg_lat += playerInfo[i].Latitude;
        avg_lon += playerInfo[i].Longitude;
    }

    let playerAmount = hunterAmount + runnerAmount;
    map.setView([avg_lat/playerAmount, avg_lon/playerAmount], 14);

    map.addLayer(markers)
}

function createMarker(coords, playerInfo, type) {
    if (playerInfo.Name === playerName) {
        var marker = L.marker(coords);
        marker.bindPopup(`<b>${playerInfo.Name}<b>`);
    }
    else if (type === "hunter") {
        var marker = L.marker(coords, {icon: redIcon});
        marker.bindPopup(`<b>${playerInfo.Name}<b>`);
    }
    else if (type === "runner") {
        var marker = L.marker(coords, {icon: greenIcon});
        marker.bindPopup(`<b>${playerInfo.Name}<b>`);
    }
    markers.addLayer(marker);
}

// Hub and SignalR functions
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/jachtseizoenHub")
    .withAutomaticReconnect([0, 2000, 10000, 30000, 60000, 120000]) // delay before attempts in ms
    .build()

// Start the connection
connection.start().then(function () {
    connection.invoke("RetrieveTimeData", playerName);
});

// Uitvoeren als er een "FirstStart" type message komt van de server
// Might need fixing for NextLocTime
connection.on("FirstStart", function (timeData) {
    [gameTimeH, gameTimeM, gameTimeSec] = timeData[0].split(":");
    [playerTimeM, playerTimeSec] = timeData[1].split(":");
    gameTimeSec = parseInt(gameTimeH) * 3600 + parseInt(gameTimeM) *60 + parseInt(gameTimeSec);
    playerTimeSec = parseInt(playerTimeM) * 60 + parseInt(playerTimeSec);
    setInterval(countdown, 1000);
});

connection.on("LocationUpdate", function (playerTimeUpdate, playerInfo, hunterAmount, runnerAmount, updateFrom) {
    // Playertime should come from server side instead of time between !!!
    if (updateFrom === playerName) {
        playerTimeSec = playerTimeUpdate
    }
    displayTime(playerTimeUpdate, "playerTime");
    updateMarkers(playerInfo, hunterAmount, runnerAmount);
})

// TODO:
// 1. Center map around markers