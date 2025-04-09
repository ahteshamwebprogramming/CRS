let RoomTypeToBook = [];
function RoomTypeGridPartialView(CheckInDate, CheckOutDate) {
    var inputDTO = {};
    inputDTO.CheckInDate = CheckInDate;
    inputDTO.CheckOutDate = CheckOutDate;
    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/RoomTypeGridPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            //UnblockUI();

            $('#div_RoomTypeGridPartialView').html(data);
            initCalenderAvail();
            initAddRoomCounter();

        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });


}
function PackagesPartialView() {
    //var inputDTO = {};
    //inputDTO.CheckInDate = CheckInDate;
    //inputDTO.CheckOutDate = CheckOutDate;
    //BlockUI();
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
    //const dateSliderContainer = document.querySelector(".date-slider-container");

    //if (calendarToggle && dateSliderContainer) {
    //    calendarToggle.addEventListener("click", function () {
    //        dateSliderContainer.classList.toggle("d-none");
    //    });
    //}

    calendarToggles.forEach(function (calendarToggle) {
        // Find the closest parent container (e.g., the card) and then find the .date-slider-container within it
        const dateSliderContainer = calendarToggle.closest(".container").querySelector(".date-slider-container");
        let rTypeId = $(calendarToggle.closest(".container")).find("[name='RoomTypeId']").val()
        let recordId = $(calendarToggle.closest(".container")).find("[name='RecordId']").val()
        // Attach a click event listener to the current .availability_calendar
        if (calendarToggle && dateSliderContainer) {
            calendarToggle.addEventListener("click", function () {
                // Toggle the visibility of the corresponding .date-slider-container
                dateSliderContainer.classList.toggle("d-none");
                AvailabilityCalenderPartialView(rTypeId, recordId);
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

        minusBtn.addEventListener("click", function () {
            let count = parseInt(bedCount.textContent);
            if (count > 1) {
                bedCount.textContent = count - 1;
            } else {
                counterContainer.style.display = "none"; // Hide the counter
                btn.style.display = "inline-block"; // Show "Add Bed" button again
                bedCount.textContent = "1"; // Reset count to 1
            }
        });

        plusBtn.addEventListener("click", function () {
            let count = parseInt(bedCount.textContent);
            bedCount.textContent = count + 1;
        });
    });
}

function GetDates() {
    let dateRange = $("#dateRangePicker").val();

    // Split the date range string into "from" and "to" dates
    let dates = dateRange.split(" to ");

    // Extract the "from" and "to" dates
    let fromDate = dates[0];
    let toDate = dates[1];

    //console.log("From Date:", fromDate);
    //console.log("To Date:", toDate);

    //alert(fromDate + " : " + toDate)
    return dates;
}

function AddRoombtn(RoomType) {

    RoomTypeToBook.push(RoomType);

    let NoOfRooms = $("#RoomCount_" + RoomType).text();
    if (!isNaN) {
        NoOfRooms = 1;
    }
    SummaryPartialView(RoomType, NoOfRooms);
    PackagesDateWisePartialView();
    $('#div_PackagesPartialView').html("");
}
function AddRoom(RoomType) {

    let NoOfRooms = $("#RoomCount_" + RoomType).text();
    if (!isNaN) {
        NoOfRooms = 1;
    }
    NoOfRooms = parseInt(NoOfRooms) + 1;
    SummaryPartialView(RoomType, NoOfRooms);
}
function SubRoom(RoomType) {
    
    let NoOfRooms = $("#RoomCount_" + RoomType).text();
    if (!isNaN) {
        NoOfRooms = 1;
    }
    NoOfRooms = parseInt(NoOfRooms) - 1;
    if (NoOfRooms > 0) {
        SummaryPartialView(RoomType, NoOfRooms);
    }
    else {
        const index = RoomTypeToBook.indexOf(RoomType);
        if (index !== -1) {
            RoomTypeToBook.splice(index, 1);
        }
        PackagesPartialView();
        $('#div_SummaryPartialView').html("");
        $('#div_PackagesDateWisePartialView').html("");
    }

}

function SummaryPartialView(RoomType, NoOfRooms) {
    var inputDTO = {};



    inputDTO.RoomTypes = RoomTypeToBook.toString(",");
    inputDTO.RoomType = RoomType;
    inputDTO.NoOfRooms = NoOfRooms;
    inputDTO.CheckInDate = currentDate.toISOString().split('T')[0];
    inputDTO.CheckOutDate = nextDate.toISOString().split('T')[0];

    console.log(currentDate.toISOString().split('T')[0]);

    //BlockUI();
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/SummaryPartialView',
        data: JSON.stringify(inputDTO),
        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_SummaryPartialView').html(data);
        },
        error: function (result) {
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });


}
function PackagesDateWisePartialView() {
    var inputDTO = {};
    inputDTO.CheckInDate = currentDate.toISOString().split('T')[0];
    inputDTO.CheckOutDate = nextDate.toISOString().split('T')[0];
    //BlockUI();
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
            //UnblockUI();
            $erroralert("Transaction Failed!", result.responseText);
        }
    });
}