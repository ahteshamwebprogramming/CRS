function AvailabilityCalenderPartialView(Id, recordId) {
    let selectedDates = GetDates();

    if (selectedDates[0] != null && selectedDates[0] != undefined && selectedDates[0] != "" && selectedDates[1] != null && selectedDates[1] != undefined && selectedDates[1] != "") {

        selectedDates[0] = moment(selectedDates[0], "DD-MMM-YYYY").toDate().toISOString().split('T')[0];
        selectedDates[1] = moment(selectedDates[1], "DD-MMM-YYYY").toDate().toISOString().split('T')[0];
        

        var inputDTO = {};
        inputDTO.CheckInDate = selectedDates[0];
        inputDTO.CheckOutDate = selectedDates[1];
        inputDTO.Rtype = Id;
        //BlockUI();
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: '/Home/AvailabilityCalenderPartialView',
            data: JSON.stringify(inputDTO),
            cache: false,
            dataType: "html",
            success: function (data, textStatus, jqXHR) {
                //UnblockUI();

                $('#div_AvailabilityCalenderPartialView_' + recordId).html(data);
                //initCalenderAvail();
            },
            error: function (result) {
                //UnblockUI();
                $erroralert("Transaction Failed!", result.responseText);
            }
        });
    }
}

