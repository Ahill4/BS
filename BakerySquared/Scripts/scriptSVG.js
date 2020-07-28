
/*
 * Global variables:
 * 
 * string lastID- a variable that contains the last clicked/searched ID for purposes of restoring its fill property on next click
 * currentFloor-when changing the select object or when routed to a new floor from search the current floor is set so it knows if 
 *              it needs to route to a new floor at certain times
 * reD- regular expression that chechs if an ID matches the appropriate format of a capital letter followed by 4 integers
 * */
var lastID;
var currentFloor;
var reD = /[A-Z][0-9]{4}/


/*
 * document.ready
 * 
 * Jquery function used on page load to do whatever needs done as soon as the page loads.
 * currently sets the current floor based on url and adds an on click listener to all elements 
 * with the ID of the format given by reD.
 * Also checks url for query paraeters and if it exists calls the necessaey functions
 */
$(document).ready(function () {

    const path = window.location.pathname;
    currentFloor = path[path.length - 1];

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
 * Called from document.ready as the onclick function given to the proper objects, calls conttroller and sets fill as needed
 */
function GetController() {
    let temp = $(this).attr('id');
    setFill(temp);

    ajaxCall(temp);

}

/*
 * Function searchBar:
 * 
 * Function is called when the submit button on the search bar is hit. used for routing to new floor if necessary
 * if not necessary to go to new floor it
 */
function searchBar() {
    //get value entered in the search bar
    var searchedVal = document.getElementById("search").value;


    //check it to make sure it is proper format ID
    if (reD.test(searchedVal)) {

        //compare the floor value from the id to current floor and if not equal redirect to proper floor
        if (searchedVal[1] != currentFloor) {
            window.location.href = '/Home/Floor' + searchedVal[1] + "?ID=" + searchedVal;
        }
        else if (searchedVal[1] == currentFloor) {

            setFill(searchedVal);

            ajaxCall(searchedVal);
        }
    }
};

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
    $.ajax({
        type: "GET",
        url: 'GetController',
        data: {
            id: ID
        },
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            alert(result)
        },
        error: function (response) {
            alert('eror');
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

function collectDB() {
    var locations = [];
    $("*").each(function () {
        if (reD.test(this.id)) {
            locations.push(this.id);
        }
    });
    console.log(locations);
    fillDB(locations);
}

function fillDB(locations) {
    $.ajax({
        type: "POST",
        url: 'refillDB',
        data: {
            locationsArr: JSON.stringify(locations)
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