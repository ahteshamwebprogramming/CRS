
function RoomTypeGridPartialView(CheckInDate, CheckOutDate) {
    var inputDTO = {};
    inputDTO.CheckInDate = CheckInDate;
    inputDTO.CheckOutDate = CheckOutDate;
    
    $.ajax({
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



function AddRoom(RoomType) {

    debugger
    if (!RoomSelections[RoomType]) {
        RoomSelections[RoomType] = 1;
    } else {
        RoomSelections[RoomType]++;
    }

    
    $("#RoomCount_" + RoomType).text(RoomSelections[RoomType]);

    
    SummaryPartialView();
}

function AddRoombtn(RoomType) {
    if (!RoomSelections[RoomType]) {
        RoomSelections[RoomType] = 1;
    }

    
    $(`#RoomCount_${RoomType}`).text(RoomSelections[RoomType]);
    $(`.counter-container [onclick*="${RoomType}"]`).closest('.counter-container').show();
    $(`.add-bed-btn[onclick*="${RoomType}"]`).hide();

    
    updatePaxData();
    SummaryPartialView();
}
function updateSummary() {
    const summaryText = `${PaxData.Adults} Adult${PaxData.Adults > 1 ? 's' : ''}, ${PaxData.Children} Child${PaxData.Children !== 1 ? 'ren' : ''}`;
    $("#selectionSummary").text(summaryText);
}

function SubRoom(RoomType) {
    if (RoomSelections[RoomType]) {
        RoomSelections[RoomType]--;

        if (RoomSelections[RoomType] <= 0) {
            delete RoomSelections[RoomType];
           
            $(".counter-container [onclick*='" + RoomType + "']").closest(".counter-container").hide();
            $(".add-bed-btn[onclick*='" + RoomType + "']").show();
        } else {
            $("#RoomCount_" + RoomType).text(RoomSelections[RoomType]);
        }

        SummaryPartialView();
    }
}

function SummaryPartialView() {
    const inputDTO = {
        RoomSelections: RoomSelections,
        PaxPerRoom: PaxData,
        CheckInDate: currentDate.toISOString().split('T')[0],
        CheckOutDate: nextDate.toISOString().split('T')[0]
    };

    console.log("Sending to server:", JSON.stringify(inputDTO));

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
    document.querySelectorAll("#rooms .room").forEach((room, index) => {
        PaxData.push({
            RoomNumber: index + 1,
            Adults: parseInt(room.querySelector(".adults").innerText),
            Children: parseInt(room.querySelector(".children").innerText)
        });
    });
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