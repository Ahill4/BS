/*******************************************************************************
 * @file
 * @brief The script used by all the floor views
 *
 * *****************************************************************************
 *   Copyright (c) 2020 Koninklijke Philips N.V.
 *   All rights are reserved. Reproduction in whole or in part is
 *   prohibited without the prior written consent of the copyright holder.
 *******************************************************************************/

/*
 * Global variables:
 * 
 * string lastID- a variable that contains the last clicked/searched ID for purposes of restoring its fill 
 *                property on next click
 * currentFloor-when changing the select object or when routed to a new floor from search the current floor 
 *              is set so it knows if it needs to route to a new floor at certain times
 * reD- regular expression that checks if an ID matches the appropriate format of a specific capital letter
 *      followed by 4 integers
 * */
var lastID;
var currentFloor;
var reD = /(D|M|S)[0-9]{4}/

/*
 * document.ready
 * 
 * Jquery function used on page load.
 * currently sets the current floor based on url and adds an on click listener to all elements 
 * with the ID of the format given by reD.
 * Also checks url for query parameters and if it exists calls the necessary functions
 */
$(document).ready(function () {
    const path = window.location.pathname;
    currentFloor = path[path.length - 1];
    if (currentFloor == "/") {
        currentFloor = "1";
    }
    $("*").each(function () {
        if (reD.test(this.id)) {
            $("#" + this.id).on("click", GetController);
        }
    });

    const query = window.location.search;
    const urlParams = new URLSearchParams(query);
    let ID = urlParams.get('ID');
    if (ID) {
        lastID = ID;
        setFill(ID);
        ajaxCall(ID);
    }
});

/*
 * document.addEventListener
 * 
 * gets div containing SVG and attaches even listener that allows for click and drag functionality on map
 */
document.addEventListener('DOMContentLoaded', function () {
    const ele = document.getElementById('map');
    ele.style.cursor = 'grab';

    let pos = { top: 0.5, left: 0.5, x: 0.5, y: 0.5 };

    const mouseDownHandler = function (e) {
        ele.style.cursor = 'grabbing';
        ele.style.userSelect = 'none';

        pos = {
            left: ele.scrollLeft,
            top: ele.scrollTop,
            // Get the current mouse position
            x: e.clientX,
            y: e.clientY,
        };

        document.addEventListener('mousemove', mouseMoveHandler);
        document.addEventListener('mouseup', mouseUpHandler);
    };

    const mouseMoveHandler = function (e) {
        // How far the mouse has been moved
        const dx = e.clientX - pos.x;
        const dy = e.clientY - pos.y;

        // Scroll the element
        ele.scrollTop = pos.top - dy;
        ele.scrollLeft = pos.left - dx;
    };

    const mouseUpHandler = function () {
        ele.style.cursor = 'grab';
        ele.style.removeProperty('user-select');

        document.removeEventListener('mousemove', mouseMoveHandler);
        document.removeEventListener('mouseup', mouseUpHandler);
    };

    // Attach the handler
    ele.addEventListener('mousedown', mouseDownHandler);
});

/*
 * Function GetController
 * 
 * This is the function set  on all the clickable locations that changes fill on click and makes a call to the controller with the id
 */
function GetController() {
    let temp = $(this).attr('id');
    setFill(temp);
    ajaxCall(temp);
}

/*
 * Function setFill
 * 
 * Function is called to restore the last ID to its proper fill and set the fill of the new object to the red shown when clicked
 */
function setFill(currentID) {
    $("#" + lastID).css("fill", "inherit");
    lastID = currentID;
    $("#" + currentID).css("fill", "red");
}

/*
 * Function ajaxCall
 * 
 * Called when making a call to the controller sends an alert if controller responds with Json
 */
function ajaxCall(ID) {
    //checks if default page or regular floor page so that it can route to the right controller method
    let urlPath;
    let path = window.location.pathname;
    if (path == "/") {
        urlPath = 'Home/GetController';
    }
    else {
        urlPath = 'GetController'
    }

    $.ajax({
        type: "GET",
        url: urlPath,
        data: {
            id: ID
        },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            if (result == "True") {
                var add = confirm("Assign someone to this desk?");
                if (add) {
                    var name = prompt("Name");
                    var userId = prompt("Id");
                    var title = prompt("Title");
                    var phone = prompt("Phone");
                    var email = prompt("Email");
                    var manager = prompt("Manager");
                    if (userId) {
                        deskFill(name, ID, userId, title, phone, email, manager);
                    }
                }
            }
            else {
                alert(result);
            }
        },
        error: function (response) {
            alert('error');
        }
    });
}

/*
 * Function setFloor
 * 
 * called from view on selector submit, gets the value from the selector and redirects to the appropriate floor
 */
function setFloor() {
    var floorValue = document.getElementById("floors").value;
    if (floorValue && floorValue != currentFloor) {
        window.location.href = '/Home/Floor' + floorValue;
    }
}

/*
 * Function setZoom
 * 
 * takes the level of zoom given in view and the element in the mapContainer and performs transform to enlarge
 */
function setZoom(zoom, el) {
    transformOrigin = [0, 0];
    el = el || instance.getContainer();
    var p = ["webkit", "moz", "ms", "o"],
        s = "scale(" + zoom + ")",
        oString = (transformOrigin[0] * 100) + "% " + (transformOrigin[1] * 100) + "%";

    for (var i = 0; i < p.length; i++) {
        el.style[p[i] + "Transform"] = s;
        el.style[p[i] + "TransformOrigin"] = oString;
    }

    el.style["transform"] = s;
    el.style["transformOrigin"] = oString;
}

/*
 * Function showVal
 * 
 * function called from view to use the value given for zoom to enlarge div
 */
function showVal(a) {
    var zoomScale = Number(a) / 10;
    setZoom(zoomScale, document.getElementsByClassName('mapContainer')[0])
}

/*
 * Function fillDB
 *
 * function called from view to repopulate Db with all the locations from the current floor to 
 * be used after a floor plan change
 */
function fillDB() {
    //checks if default page or regular floor page so that it can route to the right controller method
    let urlPath;
    let path = window.location.pathname;
    if (path == "/") {
        urlPath = 'Home/refillDB';
    }
    else {
        urlPath = 'refillDB'
    }
    $.ajax({
        type: "GET",
        url: urlPath,
        data: {
            floor: currentFloor
        },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            alert(result)
        },
        error: function (response) {
            alert("error");
        }
    });
}

/*
 * Function deskFill
 *
 * function called when an registered user desires to fill a desk they clicked in view. 
 * uses information provided by user to add a user to DB and assign them to the desk
 */
function deskFill(name, ID, userId, title, phone, email, manager) {
    let urlPath;
    let path = window.location.pathname;
    if (path == "/") {
        urlPath = 'Home/deskFill';
    }
    else {
        urlPath = 'deskFill'
    }

    $.ajax({
        type: "GET",
        url: urlPath,
        data: {
            name: name,
            id: ID,
            userId: userId,
            title: title,
            phone: phone,
            email: email,
            manager: manager
        },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            alert(result)
        },
        error: function (response) {
            alert("error");
        }
    });
}
