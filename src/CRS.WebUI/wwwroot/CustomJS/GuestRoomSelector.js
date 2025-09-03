let RoomSelections = {};
let PaxData = [];
let tempPax = { Adults: 1, Children: 0 };
let roomCount = 1;
let SelectedServices = [];
let RoomSelectionList = [];
let RoomSelectionDTO1 = {};

$(document).ready(function () {
    //RoomSelectionList.push({ RoomNumber: 1, Adults: 1, Children: 0, SelectedServices: [] });

    loadSelectionsFromSession();
});

// Improved popup toggle function
function togglePopup() {
    const popup = document.getElementById("popup");
    const isVisible = popup.style.display === "block";
    popup.style.display = isVisible ? "none" : "block";

    //// Position popup near the button
    //if (!isVisible) {
    //    const button = document.getElementById("selectionSummary");
    //    const rect = button.getBoundingClientRect();
    //    popup.style.top = (rect.bottom + window.scrollY + 5) + "px";
    //    popup.style.left = (rect.left + window.scrollX) + "px";
    //}
}

// Close popup when clicking outside
document.addEventListener("click", function (event) {
    const popup = document.getElementById("popup");
    const buttons = document.querySelectorAll(".person_filled_button");
    const isButton = Array.from(buttons).some(btn => btn.contains(event.target));
    const isPopup = popup.contains(event.target);

    if (!isButton && !isPopup && popup.style.display === "block") {
        popup.style.display = "none";
    }
});

// Improved counter update function
function updateCounter(button, roomId, type, change) {

    const roomElement = document.querySelector(`.room[data-room-id="${roomId}"]`);
    if (!roomElement) return;
    const countElement = roomElement.querySelector(`.${type}`);
    let currentValue = parseInt(countElement.textContent);
    let newValue = currentValue + change;
    // Validate constraints

    if (type === 'adults') {
        if (roomId === 1 && newValue < 1) return; // First room can't go negative
        if (roomId !== 1 && newValue < 0) return; // Other rooms: can't go negative, removal happens when newValue === 0
        if (newValue > 4) return;
    }
    //if (type === 'adults' && (newValue < 1 || newValue > 4)) return;


    if (type === 'children' && (newValue < 0 || newValue > 2)) return;

    // Check total guests per room (max 4)
    const adults = type === 'adults' ? newValue : parseInt(roomElement.querySelector('.adults').textContent);
    const children = type === 'children' ? newValue : parseInt(roomElement.querySelector('.children').textContent);

    if (adults + children > 4) {
        alert("Maximum 4 guests per room");
        return;
    }

    // ✅ If adults become 0 (for rooms other than 1), remove room from DOM and list
    if (type === 'adults' && roomId !== 1 && newValue === 0) {

        let roomToRemove = RoomSelectionList.find(r => r.RoomNumber === roomId);
        if (roomToRemove) {
            let removedRoomType = roomToRemove.RoomDetails ? roomToRemove.RoomDetails.RoomTypeId : null;

            let sameTypeCount = RoomSelectionList.filter(r => r.RoomDetails && r.RoomDetails.RoomTypeId === removedRoomType).length;

            sameTypeCount = sameTypeCount - 1;
            if (sameTypeCount <= 0) {
                $(`.counter-container [onclick*="${removedRoomType}"]`).closest(".counter-container").hide();
                $(`.add-bed-btn[onclick*="${removedRoomType}"]`).show();
            }
            else {
                $("#RoomCount_" + removedRoomType).text(sameTypeCount);
            }
        }

        // Remove from DOM
        roomElement.remove();

        // Remove from RoomSelectionList



        RoomSelectionList = RoomSelectionList.filter(r => r.RoomNumber !== roomId);

        // Update summary after removal
        //updateSummaryText();
        //SummaryPartialView1();

        //return; // Stop further execution
    }

    increaseDecreasePaxForRoom(roomId, type, change).then(function (status) {
        // Update value
        countElement.textContent = newValue;

        // Update tempPax for this room
        if (roomId === 1) {
            tempPax[type === 'adults' ? 'Adults' : 'Children'] = newValue;
        }

        updateSummaryText();

        applyGuestSelection();
    });

}

// Add new room to popup
function addRoom() {
    if (RoomSelectionList.length >= 4) {
        alert("Maximum 4 rooms allowed");
        return;
    }

    roomCount++;
    const roomsContainer = document.getElementById("rooms");
    const newRoom = document.createElement('div');
    newRoom.className = 'room';
    newRoom.setAttribute('data-room-id', roomCount);
    newRoom.innerHTML = `
            <span>Room ${roomCount}</span>
            <div class="counter">
                <span class="userPerson">Adults</span>
                <button onclick="updateCounter(this, ${roomCount}, 'adults', -1)">−</button>
                <span class="adults">1</span>
                <button onclick="updateCounter(this, ${roomCount}, 'adults', 1)">+</button>
            </div>
            <div class="counter">
                <span class="userPerson">Children</span>
                <button onclick="updateCounter(this, ${roomCount}, 'children', -1)">−</button>
                <span class="children">0</span>
                <button onclick="updateCounter(this, ${roomCount}, 'children', 1)">+</button>
            </div>
        `;

    roomsContainer.appendChild(newRoom);
    //applyGuestSelection();
    //RoomSelectionList.push({ RoomNumber: roomCount, Adults: 1, Children: 0 });
    RoomSelectionList.push({ RoomNumber: roomCount, Adults: 1, Children: 0, SelectedServices: [] });
    SummaryPartialView1();
}

// Render rooms based on current RoomSelectionList
function renderRoomsFromSession() {
    const roomsContainer = document.getElementById("rooms");
    roomsContainer.innerHTML = '';
    roomCount = RoomSelectionList.length;
    RoomSelectionList.forEach(item => {
        const roomId = item.RoomNumber;
        const roomDiv = document.createElement('div');
        roomDiv.className = 'room';
        roomDiv.setAttribute('data-room-id', roomId);
        roomDiv.innerHTML = `
            <span>Room ${roomId}</span>
            <div class="counter">
                <span class="userPerson">Adults</span>
                <button onclick="updateCounter(this, ${roomId}, 'adults', -1)">−</button>
                <span class="adults">${item.Adults}</span>
                <button onclick="updateCounter(this, ${roomId}, 'adults', 1)">+</button>
            </div>
            <div class="counter">
                <span class="userPerson">Children</span>
                <button onclick="updateCounter(this, ${roomId}, 'children', -1)">−</button>
                <span class="children">${item.Children}</span>
                <button onclick="updateCounter(this, ${roomId}, 'children', 1)">+</button>
            </div>
        `;
        roomsContainer.appendChild(roomDiv);
    });
}


// Update the room summary text
function updateRoomSummaryText() {

    let sroomCount = RoomSelectionList.length;
    document.getElementById("roomSummary").textContent = `${sroomCount} Room${sroomCount !== 1 ? 's' : ''}`;
}

// Update the guest summary text
function updateSummaryText() {
    let totalAdults = 0;
    let totalChildren = 0;

    document.querySelectorAll('#rooms .room').forEach(room => {
        totalAdults += parseInt(room.querySelector('.adults').textContent);
        totalChildren += parseInt(room.querySelector('.children').textContent);
    });

    const summaryText = `${totalAdults} Adult${totalAdults !== 1 ? 's' : ''}, ${totalChildren} Child${totalChildren !== 1 ? 'ren' : ''}`;
    document.getElementById("selectionSummary").textContent = summaryText;
}

// Apply guest selection
function applyGuestSelection() {
    PaxData = [];

    document.querySelectorAll('#rooms .room').forEach((room, index) => {
        const roomId = room.getAttribute('data-room-id');
        const adults = parseInt(room.querySelector('.adults').textContent);
        const children = parseInt(room.querySelector('.children').textContent);

        PaxData.push({
            RoomNumber: roomId,
            Adults: adults,
            Children: children
        });
    });

    //togglePopup();
    updateSummaryDisplay();
    SummaryPartialView();
}

// Update summary display with room selections
function updateSummaryDisplay() {
    let totalAdults = 0;
    let totalChildren = 0;

    PaxData.forEach(room => {
        totalAdults += room.Adults;
        totalChildren += room.Children;
    });

    const summaryText = `${totalAdults} Adult${totalAdults !== 1 ? 's' : ''}, ${totalChildren} Child${totalChildren !== 1 ? 'ren' : ''}`;
    document.getElementById("selectionSummary").textContent = summaryText;
    document.getElementById("roomSummary").textContent = `${PaxData.length} Room${PaxData.length !== 1 ? 's' : ''}`;
}

// Toggle service selection
function toggleService(checkbox, service, price) {
    let serviceId = $(checkbox).attr("data-service-id");
    const serviceObj = { Service: service, Price: price, Id: serviceId };
    const wasChecked = checkbox.dataset.checked === 'true';
    if (checkbox.checked) {
        // If already checked, add service only to rooms missing it
        if (wasChecked) {
            RoomSelectionList.forEach(room => {
                if (!room.SelectedServices.some(s => s.Service === service)) {
                    room.SelectedServices.push(serviceObj);
                }
            });
        } else {
            // Newly checked: add service to all rooms
            if (!SelectedServices.some(s => s.Service === service)) {
                SelectedServices.push(serviceObj);
            }
            RoomSelectionList.forEach(room => {
                if (!room.SelectedServices.some(s => s.Service === service)) {
                    room.SelectedServices.push(serviceObj);
                }
            });
        }
    } else {
        SelectedServices.push({ Service: service, Price: price });
        // Unchecked: remove service from all rooms
        SelectedServices = SelectedServices.filter(s => s.Service !== service);
        RoomSelectionList.forEach(room => {
            room.SelectedServices = room.SelectedServices.filter(s => s.Service !== service);
        });
    }
    checkbox.dataset.checked = checkbox.checked;

    updateServicesDisplay();
    SummaryPartialView();
    SummaryPartialView1();
}

// Remove service from a specific room in summary
function removeService(roomNumber, service) {
    let room = RoomSelectionList.find(r => r.RoomNumber === roomNumber);
    if (!room) return;

    room.SelectedServices = room.SelectedServices.filter(s => s.Service !== service);

    // If no room has this service, remove from global list and uncheck checkbox
    const serviceStillSelected = RoomSelectionList.some(r => r.SelectedServices.some(s => s.Service === service));
    if (!serviceStillSelected) {
        SelectedServices = SelectedServices.filter(s => s.Service !== service);
        const chk = document.querySelector(`input[data-service="${service}"]`);
        if (chk) {
            chk.checked = false;
            chk.dataset.checked = 'false';
        }
    }

    SummaryPartialView();
    SummaryPartialView1();
}

// Update services display
function updateServicesDisplay() {
    // You can implement visual feedback for selected services here
    console.log("Selected services:", SelectedServices);
}

// Submit pax data
//function submitPaxDataAjax() {
//    $.ajax({
//        url: '/Home/Pax',
//        type: 'POST',
//        data: JSON.stringify(PaxData),
//        contentType: 'application/json',
//        success: function (response) {
//            if (response.redirectUrl) {
//                window.location.href = "/Home/Booking";
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error:", error);
//            alert("Something went wrong while submitting guest data.");
//        }
//    });
//}

// Room selection functions (to be implemented based on your backend)
//function RoomTypeGridPartialView(checkIn, checkOut) {
//    // Implement your AJAX call to load room types
//    console.log("Loading rooms for:", checkIn, "to", checkOut);

//    // Example implementation:
//    $.ajax({
//        url: '/Home/RoomTypeGridPartialView',
//        type: 'POST',
//        data: {
//            checkInDate: checkIn,
//            checkOutDate: checkOut
//        },
//        success: function (data) {
//            $('#div_RoomTypeGridPartialView').html(data);
//        },
//        error: function () {
//            alert('Error loading room data');
//        }
//    });
//}

//function SummaryPartialView() {
//    // Implement your AJAX call to update summary
//    const selectedDates = $("#dateRangePicker").val().split(" to ");
//    const checkIn = selectedDates[0];
//    const checkOut = selectedDates[1];

//    $.ajax({
//        url: '/Home/SummaryPartialView',
//        type: 'POST',
//        data: {
//            checkInDate: checkIn,
//            checkOutDate: checkOut,
//            paxData: PaxData,
//            selectedServices: SelectedServices
//        },
//        success: function (data) {
//            $('#div_SummaryPartialView').html(data);
//        }
//    });
//}

function PackagesPartialView() {
    // Your existing implementation
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/PackagesPartialView',
        cache: false,
        dataType: "html",
        success: function (data) {
            $('#div_PackagesPartialView').html(data);
        },
        error: function (result) {
            console.error("Transaction Failed!", result.responseText);
        }
    });
}

function increaseDecreasePaxForRoom(roomNumber, opt, change) {
    return new Promise((resolve, reject) => {
        let room = RoomSelectionList.find(r => r.RoomNumber === roomNumber);
        if (room) {
            if (opt == "adults") {
                room.Adults = room.Adults + change;
            }
            else if (opt == "children") {
                room.Children = room.Children + change;
            }
        }

        SummaryPartialView1().then(function (status) {
            if (status) {
                resolve(true);
            } else {
                reject(false);
            }
        });
    });
}




function SummaryPartialView1() {
    return new Promise((resolve) => {
        let currentDateSP = new Date(currentDate.getTime() - currentDate.getTimezoneOffset() * 60000);
        let CheckInDate = currentDateSP.toISOString().split('T')[0];

        let nextDateSP = new Date(nextDate.getTime() - nextDate.getTimezoneOffset() * 60000);
        let CheckOutDate = nextDateSP.toISOString().split('T')[0];

        const inputDTO = {
            CheckInDate: CheckInDate,
            CheckOutDate: CheckOutDate,
            RoomSelectionList: RoomSelectionList,
            SelectedServices: SelectedServices
        };

        $.ajax({
            type: "POST",
            url: '/Home/SummaryPartialView1',
            contentType: "application/json",
            data: JSON.stringify(inputDTO),
            success: function (data) {
                $('#div_SummaryPartialView1').html(data);
                updateRoomSummaryText();
                updateSummaryText();
                resolve(true); // ✅ Return true on success
            },
            error: function (xhr) {
                console.error("Error:", xhr.responseText);
                resolve(false); // ✅ Return false on error
            }
        });
    });
}



function AddRoomFromCard(roomTypeId, opt, price) {
    return new Promise((resolve, reject) => {
        if (opt == "Add") {
            let room = RoomSelectionList.find(r => !r.RoomDetails);
            if (room) {
                room.RoomDetails = {
                    RoomTypeId: roomTypeId,
                    Price: price
                };
            } else {
                // Show SweetAlert if all rooms have RoomDetails
                Swal.fire({
                    icon: 'warning',
                    title: 'No empty room slot!',
                    text: 'Please add a room from the header first.',
                    confirmButtonText: 'OK'
                });
                return false;
            }
        }

        SummaryPartialView1().then(function (status) {
            if (status) {
                resolve(true);
            } else {
                reject(false);
            }
        });
    });
}

function AddRoomFromCard(roomTypeId, opt, price) {
    return new Promise((resolve, reject) => {
        if (opt == "Add") {
            let room = RoomSelectionList.find(r => !r.RoomDetails);
            if (room) {
                room.RoomDetails = {
                    RoomTypeId: roomTypeId,
                    Price: price
                };
            } else {
                // Show SweetAlert if all rooms have RoomDetails
                Swal.fire({
                    icon: 'warning',
                    title: 'No empty room slot!',
                    text: 'Please add a room from the header first.',
                    confirmButtonText: 'OK'
                });
                return false;
            }
        }
        else if (opt === "Remove") {
            // Find the last room with the matching RoomTypeId
            let index = -1;
            for (let i = RoomSelectionList.length - 1; i >= 0; i--) {
                if (RoomSelectionList[i].RoomDetails && RoomSelectionList[i].RoomDetails.RoomTypeId === roomTypeId) {
                    index = i;
                    break;
                }
            }

            if (index !== -1) {
                RoomSelectionList[index].RoomDetails = undefined; // ✅ Remove RoomDetails but keep room slot
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'No room found!',
                    text: 'There is no room of this type to remove.',
                    confirmButtonText: 'OK'
                });
                return resolve(false);
            }
        }


        SummaryPartialView1().then(function (status) {
            if (status) {
                resolve(true);
            } else {
                reject(false);
            }

        });
    });
}





function loadSelectionsFromSession() {
    $.ajax({
        type: 'GET',
        url: '/Home/GetDataForLoadSummaryFromSession1',
        success: function (data) {
            // Restore previously selected dates from session
            if (data) {
                if (data.checkInDate) {
                    currentDate = new Date(data.checkInDate);
                }
                if (data.checkOutDate) {
                    nextDate = new Date(data.checkOutDate);
                }
                if (data.checkInDate && data.checkOutDate) {
                    const pickerEl = document.getElementById('dateRangePicker');
                    const picker = pickerEl && pickerEl._flatpickr;
                    if (picker) {
                        picker.setDate([currentDate, nextDate], false);
                    } else if (pickerEl) {
                        // Fallback in case flatpickr is not initialized yet
                        const options = { year: 'numeric', month: 'short', day: '2-digit' };
                        pickerEl.value = `${currentDate.toLocaleDateString('en-US', options)} to ${nextDate.toLocaleDateString('en-US', options)}`;
                    }
                }
            }

            //if (data && data.roomSelectionList && data.roomSelectionList.length > 0) {
            let hasRoomData = data && data.roomSelectionList && data.roomSelectionList.length > 0;

            if (hasRoomData) {
                RoomSelectionList = data.roomSelectionList.map(function (item) {
                    return {
                        RoomNumber: item.roomNumber,
                        Adults: item.adults,
                        Children: item.children,
                        RoomDetails: item.roomDetails,
                        SelectedServices: item.selectedServices || []
                    };
                });

                SelectedServices = (data.selectedServices || []).map(function (s) {
                    return { Service: s.service, Price: s.price };
                });

                RoomSelections = {};
                PaxData = [];
                RoomSelectionList.forEach(function (item) {
                    var rt = item.RoomDetails && item.RoomDetails.roomTypeId;
                    if (rt != null) {
                        if (!RoomSelections[rt]) {
                            RoomSelections[rt] = {
                                count: 1,
                                price: item.RoomDetails.price,
                                Pax: { Adults: item.Adults, Children: item.Children }
                            };
                        } else {
                            RoomSelections[rt].count++;
                        }
                        PaxData.push({ RoomNumber: item.RoomNumber, Adults: item.Adults, Children: item.Children });
                    }
                });

                roomCount = RoomSelectionList.length;
            }
            else {
                RoomSelectionList = [{ RoomNumber: 1, Adults: 1, Children: 0, SelectedServices: [] }];
                roomCount = RoomSelectionList.length;
            }
            renderRoomsFromSession();
            //const counts = {};
            //RoomSelectionList.forEach(function (item) {
            //    var rt = item.RoomDetails && item.RoomDetails.roomTypeId;
            //    if (rt != null) {
            //        counts[rt] = (counts[rt] || 0) + 1;
            //    }
            //});
            //Object.keys(counts).forEach(function (rt) {
            //    $(`.add-bed-btn[onclick*="${rt}"]`).hide();
            //    const container = $(`.counter-container [onclick*="${rt}"]`).closest('.counter-container');
            //    container.show();
            //    $(`#RoomCount_${rt}`).text(counts[rt]);
            //});

            //SelectedServices.forEach(function (s) {
            //    $(`input[data-service='${s.Service}']`).prop('checked', true);
            //});

            let checkIn = currentDate.toISOString().split('T')[0];
            let checkOut = nextDate.toISOString().split('T')[0];
            RoomTypeGridPartialView(checkIn, checkOut).done(function () {
                if (hasRoomData) {
                    const counts = {};
                    RoomSelectionList.forEach(function (item) {
                        var rt = item.RoomDetails && item.RoomDetails.roomTypeId;
                        if (rt != null) {
                            counts[rt] = (counts[rt] || 0) + 1;
                        }
                    });
                    Object.keys(counts).forEach(function (rt) {
                        $(`.add-bed-btn[onclick*="${rt}"]`).hide();
                        const container = $(`.counter-container [onclick*="${rt}"]`).closest('.counter-container');
                        container.show();
                        $(`#RoomCount_${rt}`).text(counts[rt]);
                    });

                    SelectedServices.forEach(function (s) {
                        $(`input[data-service='${s.Service}']`).prop('checked', true);
                    });

                    $('.crsContainer').show();
                }

                updateRoomSummaryText();
                updateSummaryText();
                SummaryPartialView1();
                //} else {
                //    RoomSelectionList =[{ RoomNumber: 1, Adults: 1, Children: 0, SelectedServices: [] }];
                //    updateRoomSummaryText();
                //    updateSummaryText();
                //    SummaryPartialView1();
                //}
            });
        },
        error: function () {
            RoomSelectionList = [{ RoomNumber: 1, Adults: 1, Children: 0, SelectedServices: [] }];
            renderRoomsFromSession();
            updateRoomSummaryText();
            updateSummaryText();
            SummaryPartialView1();
        }
    });
}

function clearAllSelections() {
    $.post('/Home/ClearRoomSession', function () {
        RoomSelections = {};
        PaxData = [];
        SelectedServices = [];
        RoomSelectionList = [{ RoomNumber: 1, Adults: 1, Children: 0, SelectedServices: [] }];
        roomCount = 1;

        const pickerEl = document.getElementById('dateRangePicker');
        if (pickerEl) {
            if (pickerEl._flatpickr) {
                pickerEl._flatpickr.clear();
            } else {
                pickerEl.value = '';
            }
        }

        $('#div_RoomTypeGridPartialView').html('');
        $('#div_SummaryPartialView1').html('');
        $('#div_SummaryPartialView2').html('');
        $('.crsContainer').hide();

        renderRoomsFromSession();
        updateRoomSummaryText();
        updateSummaryText();
    });
}