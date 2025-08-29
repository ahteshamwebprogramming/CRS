//function togglePopup() {
//    const popup = document.getElementById("popup");
//    popup.style.display = (popup.style.display === "block") ? "none" : "block";
//}

//function updateCounter(button, roomType, type, change) {
//    console.log(`updateCounter: roomType=${roomType}, type=${type}, change=${change}`); // Debug

//    // Update tempPax (popup uses roomType=1 or dynamic roomCount, not item.Id)
//    let adults = tempPax.Adults;
//    let children = tempPax.Children;

//    if (type === 'adults') {
//        let newAdults = adults + change;
//        if (newAdults < 1 || newAdults > 3 || (newAdults + children > 3)) {
//            console.warn(`Invalid adult change: newAdults=${newAdults}`);
//            return;
//        }
//        tempPax.Adults = newAdults;
//    } else if (type === 'children') {
//        let newChildren = children + change;
//        if (newChildren < 0 || newChildren > 2 || (adults + newChildren > 3)) {
//            console.warn(`Invalid child change: newChildren=${newChildren}`);
//            return;
//        }
//        tempPax.Children = newChildren;
//    }

//    // Update UI in popup for all rooms
//    document.querySelectorAll('#rooms .room').forEach(room => {
//        const adultSpan = room.querySelector('.adults');
//        const childSpan = room.querySelector('.children');
//        if (adultSpan) adultSpan.textContent = tempPax.Adults;
//        if (childSpan) childSpan.textContent = tempPax.Children;
//    });

//    // Update RoomSelections if room is selected
//    Object.keys(RoomSelections).forEach(id => {
//        RoomSelections[id].Pax.Adults = tempPax.Adults;
//        RoomSelections[id].Pax.Children = tempPax.Children;

//        // Update guest count in room HTML
//        const roomElement = document.querySelector(`.roominfo_selector input[name="RoomTypeId"][value="${id}"]`)?.closest('.col-md-8');
//        if (roomElement) {
//            const totalGuests = tempPax.Adults + tempPax.Children;
//            const guestText = roomElement.querySelector('.text-muted.mb-1');
//            if (guestText) {
//                guestText.innerHTML = `<i class="bi bi-person"></i> x ${totalGuests} Guest${totalGuests !== 1 ? 's' : ''}`;
//            }
//        }
//    });

//    console.log(`Updated tempPax:`, tempPax); // Debug
//    updatePaxData();
//    updateSummaryDisplay();
//    SummaryPartialView();
//}

//function toggleService(service, price) {
//    debugger;
//    const index = SelectedServices.findIndex(s => s.Service === service);

//    if (index > -1) {
//        SelectedServices.splice(index, 1);
//    } else {
//        SelectedServices.push({ Service: service, Price: price });
//    }

//    updateSummaryDisplay();
//    SummaryPartialView();
//}
function updatePaxPerRoom(roomId, adults, children) {

    const existingIndex = PaxPerRoom.findIndex(p => p.RoomNumber == roomId);

    if (existingIndex >= 0) {
        PaxPerRoom[existingIndex].Adults = adults;
        PaxPerRoom[existingIndex].Children = children;
    } else {
        PaxPerRoom.push({
            RoomNumber: roomId,
            Adults: adults,
            Children: children
        });
    }
}

function updateSummaryDisplay() {
    let totalAdults = 0;
    let totalChildren = 0;
    let totalServices = SelectedServices.length ? 'Selected Services: ' + SelectedServices.map(s => s.Service).join(', ') : 'No Services Selected';

    Object.values(RoomSelections).forEach(room => {
        totalAdults += room.Pax.Adults * room.count;
        totalChildren += room.Pax.Children * room.count;
    });

    const summaryText = `${totalAdults} Adult${totalAdults !== 1 ? 's' : ''}, ${totalChildren} Child${totalChildren !== 1 ? 'ren' : ''} | ${totalServices}`;
    $("#selectionSummary").text(summaryText);
}

function updateRoomSelectionsWithPax() {

    PaxData.forEach((pax, index) => {
        const roomType = index + 1;
        if (RoomSelections[roomType]) {
            RoomSelections[roomType].Pax = {
                Adults: pax.Adults,
                Children: pax.Children
            };
        }
    });


    if (Object.keys(RoomSelections).length > 0) {
        SummaryPartialView();
    }
}

//function addRoom() {
//    roomCount++;
//    const roomsContainer = document.getElementById("rooms");
//    const newRoom = document.createElement('div');
//    newRoom.className = 'room';
//    newRoom.setAttribute('data-room-type', roomCount);
//    newRoom.innerHTML = `
//            <span>Room ${roomCount}</span>
//            <div class="counter">
//                <span class="userPerson">Adults</span>
//                <button class="adult-minus">−</button>
//                <span class="adults">${tempPax.Adults}</span>
//                <button class="adult-plus">+</button>
//            </div>
//            <div class="counter">
//                <span class="userPerson">Children</span>
//                <button class="child-minus">−</button>
//                <span class="children">${tempPax.Children}</span>
//                <button class="child-plus">+</button>
//            </div>
//        `;

//    roomsContainer.appendChild(newRoom);

//    // Bind events to counters
//    newRoom.querySelectorAll('.adult-minus, .adult-plus, .child-minus, .child-plus').forEach(btn => {
//        btn.addEventListener('click', () => {
//            const type = btn.classList.contains('adult-minus') || btn.classList.contains('adult-plus') ? 'adults' : 'children';
//            const change = btn.classList.contains('adult-minus') || btn.classList.contains('child-minus') ? -1 : 1;
//            updateCounter(btn, roomCount, type, change);
//        });
//    });
//    console.log(`Added popup room ${roomCount}`); // Debug
//    updateSummaryDisplay();
//}


//function updateSummaryDisplay() {
//    let totalAdults = 0;
//    let totalChildren = 0;
//    let totalServicePrice = SelectedServices.reduce((sum, s) => sum + s.Price, 0);
//    let totalServices = SelectedServices.length ? 'Selected Services: ' + SelectedServices.map(s => s.Service).join(', ') : 'No Services Selected';

//    Object.values(RoomSelections).forEach(room => {
//        totalAdults += room.Pax.Adults * room.count;
//        totalChildren += room.Pax.Children * room.count;
//    });

//    const summaryText = `${totalAdults} Adult${totalAdults !== 1 ? 's' : ''}, ${totalChildren} Child${totalChildren !== 1 ? 'ren' : ''} `;
//    //const summaryText = `${totalAdults} Adult${totalAdults !== 1 ? 's' : ''}, ${totalChildren} Child${totalChildren !== 1 ? 'ren' : ''} | ${totalServices} | $${totalServicePrice.toFixed(2)}`;
//    $("#selectionSummary").text(summaryText);
//}

function submitPaxDataAjax() {
    const rooms = document.querySelectorAll("#rooms .room");
    const paxList = [];

    rooms.forEach((room, index) => {
        const adults = parseInt(room.querySelector(".adults").innerText);
        const children = parseInt(room.querySelector(".children").innerText);
        paxList.push({
            RoomNumber: index + 1,
            Adults: adults,
            Children: children
        });
    });

    $.ajax({
        url: '/Home/Pax',
        type: 'POST',
        data: JSON.stringify(paxList),
        contentType: 'application/json',
        success: function (response) {
            if (response.redirectUrl) {
                window.location.href = "/Home/Booking";
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
            alert("Something went wrong while submitting pax data.");
        }
    });
}