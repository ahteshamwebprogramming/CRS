
function RoomTypeGridPartialView(CheckInDate, CheckOutDate) {
    var inputDTO = {};
    inputDTO.CheckInDate = CheckInDate;
    inputDTO.CheckOutDate = CheckOutDate;

    return $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/RoomTypeGridPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {


            $('#div_RoomTypeGridPartialView').html(data);
            initCalenderAvail();
            initAddRoomCounter();

        },
        error: function (result) {

            $erroralert("Transaction Failed!", result.responseText);
        }
    });


}
function PackagesPartialView() {

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/PackagesPartialView',
        //data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PackagesPartialView').html(data);
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });


}

function initCalenderAvail() {
    const calendarToggles = document.querySelectorAll(".availability_calendar");


    calendarToggles.forEach(function (calendarToggle) {
        // Find the closest parent container (e.g., the card) and then find the .date-slider-container within it
        const dateSliderContainer = calendarToggle.closest(".container").querySelector(".date-slider-container");
        let rTypeId = $(calendarToggle.closest(".container")).find("[name='RoomTypeId']").val()
        let recordId = $(calendarToggle.closest(".container")).find("[name='RecordId']").val()
        let packageId = $(calendarToggle.closest(".container")).find("[name='PackageId']").val()
        // Attach a click event listener to the current .availability_calendar
        if (calendarToggle && dateSliderContainer) {
            calendarToggle.addEventListener("click", function () {
                // Toggle the visibility of the corresponding .date-slider-container
                dateSliderContainer.classList.toggle("d-none");
                AvailabilityCalenderPartialView(rTypeId, recordId, packageId);
            });
        }
    });
}

function initAddRoomCounter() {
    document.querySelectorAll(".add-bed-btn").forEach((btn, index) => {
        const counterContainer = document.querySelectorAll(".counter-container")[index];
        const minusBtn = counterContainer.querySelector(".minus-btn");
        const plusBtn = counterContainer.querySelector(".plus-btn");
        const bedCount = counterContainer.querySelector(".bed-count");

        btn.addEventListener("click", function () {
            btn.style.display = "none"; // Hide the button
            counterContainer.style.display = "flex"; // Show the counter
        });

        //minusBtn.addEventListener("click", function () {
        //    let count = parseInt(bedCount.textContent);
        //    if (count > 1) {
        //        bedCount.textContent = count - 1;
        //    } else {
        //        counterContainer.style.display = "none"; // Hide the counter
        //        btn.style.display = "inline-block"; // Show "Add Bed" button again
        //        bedCount.textContent = "1"; // Reset count to 1
        //    }
        //});

        //plusBtn.addEventListener("click", function () {
        //    let count = parseInt(bedCount.textContent);
        //   // bedCount.textContent = count + 1;
        //});
    });
}

function GetDates() {
    let dateRange = $("#dateRangePicker").val();

    let dates = dateRange.split(" to ");

    let fromDate = dates[0];
    let toDate = dates[1];

    return dates;
}


//add
function AddRoom(RoomType, Price) {

    AddRoomFromCard(RoomType, 'Add', Price).then(function (status) {
        if (!RoomSelections[RoomType]) {
            RoomSelections[RoomType] = {
                count: 1,
                price: Price,
                Pax: { Adults: tempPax.Adults, Children: tempPax.Children }
            };
        } else {
            RoomSelections[RoomType].count++;
            RoomSelections[RoomType].price = Price;
        }

        // Update UI
        const roomElement = document.querySelector(`.roominfo_selector input[name="RoomTypeId"][value="${RoomType}"]`)?.closest('.col-md-8');
        if (roomElement) {
            const totalGuests = tempPax.Adults + tempPax.Children;
            const guestText = roomElement.querySelector('.text-muted.mb-1');
            if (guestText) {
                guestText.innerHTML = `<i class="bi bi-person"></i> x ${totalGuests} Guest${totalGuests !== 1 ? 's' : ''}`;
            }
        }

        let roomTypeCount = RoomSelectionList.filter(r => r.RoomDetails && r.RoomDetails.RoomTypeId === RoomType).length;
        $("#RoomCount_" + RoomType).text(roomTypeCount);

        ////updatePaxData();
        ////SummaryPartialView();

        ////addRoom();

        //updatePaxData();
        //updateSummaryDisplay();
        //SummaryPartialView();
    }).error(function (status) {


    });
}
function AddRoombtn(RoomType, Price) {

    AddRoomFromCard(RoomType, 'Add', Price).then(function (status) {
        // Use item.Id as RoomType
        if (!RoomSelections[RoomType]) {
            RoomSelections[RoomType] = {
                count: 1,
                price: Price,
                Pax: { Adults: tempPax.Adults, Children: tempPax.Children }
            };
        } else {
            RoomSelections[RoomType].count = 1; // Reset to 1 to avoid duplicates
            RoomSelections[RoomType].price = Price;
            RoomSelections[RoomType].Pax = { Adults: tempPax.Adults, Children: tempPax.Children };
        }

        // Update UI
        const roomElement = document.querySelector(`.roominfo_selector input[name="RoomTypeId"][value="${RoomType}"]`)?.closest('.col-md-8');
        if (roomElement) {
            const totalGuests = tempPax.Adults + tempPax.Children;
            const guestText = roomElement.querySelector('.text-muted.mb-1');
            if (guestText) {
                guestText.innerHTML = `<i class="bi bi-person"></i> x ${totalGuests} Guest${totalGuests !== 1 ? 's' : ''}`;
            }
        }

        let roomTypeCount = RoomSelectionList.filter(r => r.RoomDetails && r.RoomDetails.RoomTypeId === RoomType).length;
        $(`#RoomCount_${RoomType}`).text(roomTypeCount);
        $(`.counter-container [onclick*="${RoomType}"]`).closest('.counter-container').show();
        $(`.add-bed-btn[onclick*="${RoomType}"]`).hide();

        updatePaxData();
        SummaryPartialView();
    }).error(function (status) {

    });

}
function updateSummary() {
    const summaryText = `${PaxData.Adults} Adult${PaxData.Adults > 1 ? 's' : ''}, ${PaxData.Children} Child${PaxData.Children !== 1 ? 'ren' : ''}`;
    $("#selectionSummary").text(summaryText);
}

function SubRoom(RoomType) {

    AddRoomFromCard(RoomType, 'Remove', 0).then(function (status) {
        if (RoomSelections[RoomType]) {
            RoomSelections[RoomType].count--;

            if (RoomSelections[RoomType].count <= 0) {
                delete RoomSelections[RoomType];
                PaxData = PaxData.filter(p => p.RoomNumber !== parseInt(RoomType));

                $(`.counter-container [onclick*="${RoomType}"]`).closest(".counter-container").hide();
                $(`.add-bed-btn[onclick*="${RoomType}"]`).show();
            } else {

                let roomTypeCount = RoomSelectionList.filter(r => r.RoomDetails && r.RoomDetails.RoomTypeId === RoomType).length;
                $("#RoomCount_" + RoomType).text(roomTypeCount);
            }

            //updatePaxData();
            //updateSummaryDisplay();
            //SummaryPartialView();
        }
    });
}
function SummaryPartialView() {

    let currentDateSP = new Date(currentDate.getTime() - currentDate.getTimezoneOffset() * 60000);
    let CheckInDate = currentDateSP.toISOString().split('T')[0];

    let nextDateSP = new Date(nextDate.getTime() - nextDate.getTimezoneOffset() * 60000);
    let CheckOutDate = nextDateSP.toISOString().split('T')[0];

    const inputDTO = {
        RoomSelections: RoomSelections,
        PaxPerRoom: PaxData,
        SelectedServices: SelectedServices,
        CheckInDate: CheckInDate,
        CheckOutDate: CheckOutDate
    };

    $.ajax({
        type: "POST",
        url: '/Home/SummaryPartialView',
        contentType: "application/json",
        data: JSON.stringify(inputDTO),
        success: function (data) {
            $('#div_SummaryPartialView').html(data);
        },
        error: function (xhr) {
            console.error("Error:", xhr.responseText);
        }
    });
}



function updatePaxData() {
    PaxData = [];
    Object.keys(RoomSelections).forEach(roomType => {
        if (RoomSelections[roomType]) {
            for (let i = 0; i < RoomSelections[roomType].count; i++) {
                PaxData.push({
                    RoomNumber: parseInt(roomType),
                    Adults: RoomSelections[roomType].Pax.Adults,
                    Children: RoomSelections[roomType].Pax.Children
                });
            }
        }
    });
    console.log('Updated PaxData:', PaxData);
}


function PackagesDateWisePartialView() {
    var inputDTO = {};
    inputDTO.CheckInDate = currentDate.toISOString().split('T')[0];
    inputDTO.CheckOutDate = nextDate.toISOString().split('T')[0];

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/PackagesDateWisePartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PackagesDateWisePartialView').html(data);
        },
        error: function (result) {

            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}



function populateDataForView(currOBJ) {

    $currCard = $(currOBJ).closest(".room-card-view");
    let RoomType = $currCard.find(".rtype").contents().first().text().trim();
    let discounttag = $currCard.find(".discounttag").text().trim();
    let txtactualprice = $currCard.find(".txtactualprice").text().trim();
    let txtofferedprice = $currCard.find(".txtofferedprice").text().trim();
    let txtplanname = $currCard.find(".txtplanname").text().trim();
    let txtremarks = $currCard.find(".txtremarks").text().trim();


    $cardtopop = $("#roomDetailsModal");
    $("#roomDetailsModalLabel").text(RoomType);
    $cardtopop.find(".txtdiscount").text(discounttag);
    $cardtopop.find(".txtactualprice").text(txtactualprice);
    $cardtopop.find(".txtofferedprice").text(txtofferedprice);
    $cardtopop.find(".txtplanname").text(txtplanname);
    $cardtopop.find(".txtremarks").text(txtremarks);
}


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