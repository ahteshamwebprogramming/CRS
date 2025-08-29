let SelectedServices = [];
$(document).ready(function () {
    // Init tasks
    PackagesTaskView();
    LoadSummaryPartialFromSession();
    generateCaptcha();

    // Email Verification Flow
    $('#emailInput').on('input', function () {
        const email = $(this).val();
        const isValid = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/.test(email);
        $('#sendCodeBtn').toggle(isValid);
    });

    $('#sendCodeBtn').on('click', function () {
        const email = $('#emailInput').val();
        $.post('/Home/SendVerificationCode', { email }, function (response) {
            if (response.success) {
                $('#emailVerificationStatus').text('Verification code sent.');
                $('#emailCodeInput, #verifyCodeBtn').show();
            } else {
                $('#emailVerificationStatus').text('Failed to send code.');
            }
        });
    });

    $('#verifyCodeBtn').on('click', function () {
        const email = $('#emailInput').val();
        const code = $('#emailCodeInput').val();
        $.post('/Home/VerifyEmailCode', { email, code }, function (response) {
            if (response.success) {
                $('#emailVerificationStatus').text('Email verified ✅').css('color', 'green');
                $('#sendCodeBtn, #verifyCodeBtn, #emailCodeInput').hide();
            } else {
                $('#emailVerificationStatus').text('Invalid code ❌').css('color', 'red');
            }
        });
    });

    // Booking form submit with payment
    $('#bookingForm').on('submit', async function (e) {
        e.preventDefault();

        const hcaptchaResponse = hcaptcha.getResponse();
        if (!hcaptchaResponse) {
            $('#hcaptchaError').show();
            return;
        }
        $('#hcaptchaError').hide();

        let isValid = true;
        $('#bookingForm input[required][name*="Age"]').each(function () {
            const age = parseInt($(this).val(), 10);
            if (isNaN(age) || age < 14) {
                $(this).addClass('is-invalid');
                isValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });
        if (!isValid) return;

        const formData = $(this).serializeArray();
        const jsonData = serializeFormToJson(formData);
        jsonData.TotalPrice = parseFloat($('.total .amount').text().replace('₹', '').trim());

        try {
            const response = await fetch('/Home/ProcessPaymentAndSave', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(jsonData)
            });

            const result = await response.json();
            if (result.success) {
                window.location.href = result.redirectUrl || '/Reservation/Confirmation';
            } else {
                alert('Payment failed: ' + result.message);
            }
        } catch (err) {
            console.error(err);
            alert('Server error. Try again.');
        }
    });

    function serializeFormToJson(formData) {
        const data = {};
        formData.forEach(item => {
            const keys = item.name.match(/[^\[\]]+/g);
            if (keys.length === 1) {
                data[keys[0]] = item.value;
            } else {
                if (!data[keys[0]]) data[keys[0]] = [];
                const index = parseInt(keys[1]);
                if (!data[keys[0]][index]) data[keys[0]][index] = {};
                data[keys[0]][index][keys[2]] = item.value;
            }
        });
        return data;
    }
});



function generateCaptcha() {
    const num1 = Math.floor(Math.random() * 10) + 1;
    const num2 = Math.floor(Math.random() * 10) + 1;
    captchaTotal = num1 + num2;
    $('#captchaQuestion').text(`What is ${num1} + ${num2}?`);
}
function isCaptchaValid() {
    const userAnswer = parseInt($('#captchaAnswer').val(), 10);
    return userAnswer === captchaTotal;
}
function LoadSummaryPartialFromSession() {
    $.ajax({
        type: "GET",
        url: '/Home/LoadSummaryFromSession',
        cache: false,
        dataType: "html",
        success: function (data) {
            $('#div_SummaryPartialView').html(data);
        },
        error: function (result) {
            console.error("Error loading summary from session", result.responseText);
        }
    });
}
function PackagesTaskView() {

    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: '/Home/GetTaskPackage',

        cache: false,
        dataType: "html",
        success: function (data, textStatus, jqXHR) {
            $('#div_PackagesPartialView').html(data);
        },
        error: function (result) {

            $erroralert("Transaction Failed!", result.responseText);
        }
    });

}

function toggleService(service, price) {
    debugger;
    const index = SelectedServices.findIndex(s => s.Service === service);

    if (index > -1) {
        SelectedServices.splice(index, 1);
    } else {
        SelectedServices.push({ Service: service, Price: price });
    }

    //updateSummaryDisplay();
    //SummaryPartialView();

    UpdateSummaryInSession().then((d) => {
        LoadSummaryPartialFromSession();
    });



}

function UpdateSummaryInSession() {
    return new Promise((resolve, reject) => {
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
                //$('#div_SummaryPartialView').html(data);
                resolve(data);
            },
            error: function (xhr) {
                //console.error("Error:", xhr.responseText);
                reject(xhr);
            }
        });
    });

}