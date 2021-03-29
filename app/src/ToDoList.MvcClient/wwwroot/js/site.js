﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

showPopup = (url, title, button) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (result) {
            $('#form-modal .modal-body').html(result);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');

            if (button) {
                if (button.className.includes('todoItem')) {
                    initMap();
                    initSelect();
                }
            }
        }
    })
}

ajaxPost = form => {
    try {
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (result) {
                $('#view-all').html(result);
                $('#form-modal .modal-body').html("");
                $('#form-modal .modal-title').html("");

                $('#form-modal').modal('hide');
            },
            error: function (err) {
                console.log(err);
            }
        });
    } catch (e) {
        console.log(e)
    }

    return false;
}

ajaxDelete = form => {
    if (confirm("Delete?")) {
        try {
            $.ajax({
                type: "POST",
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (result) {
                    $("#view-all").html(result);
                },
                error: function (err) {
                    console.log(err);
                }
            });
        } catch (e) {
            console.log(e);
        }

        return false;
    }
}

ajaxChangeStatus = checkbox => {
    let self = $(checkbox);

    let id = self.attr("id");
    let isDone = self.prop("checked");

    try {
        $.ajax({
            type: "POST",
            url: self.data("url"),
            data: {
                id: id,
                isDone: isDone
            },
            success: function (result) {
                $("#view-all").html(result);
            },
            error: function (err) {
                console.log(err);
            }
        });
    } catch (e) {
        console.log(e);
    }

    return false;
}



function initSelect () {
    $('#categorySelector').select2({
        theme: 'bootstrap4',
        tags: true
    });
}



let marker;

function placeMarker(location) {
    if (marker) {
        marker.setPosition(location);
    } else {
        marker = new google.maps.Marker({
            position: location,
            map: map
        });
    }
}

function initMap() {
    let location;

    const latitude = document.getElementById("latitude");
    const longitude = document.getElementById("longitude");

    if (latitude.value && longitude.value) {
        location = { lat: parseFloat(latitude.value), lng: parseFloat(longitude.value) };
    }
    else {
        location = { lat: 50.450001, lng: 30.523333 };
    }

    const map = new google.maps.Map(document.getElementById("map"), {
        zoom: 6,
        center: location,
    });

    marker = new google.maps.Marker({
        position: location,
        map: map,
    });

    map.addListener("click", (e) => {
        placeMarker(e.latLng);
        map.panTo(e.latLng);

        let jsonLatLng = JSON.stringify(e.latLng);
        let latLng = JSON.parse(jsonLatLng);

        latitude.value = latLng.lat;
        longitude.value = latLng.lng;
    });
}
