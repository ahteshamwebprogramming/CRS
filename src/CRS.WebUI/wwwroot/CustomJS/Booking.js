let SelectedServices = [];
$(document).ready(function () {
    // Init tasks
    PackagesTaskView();
    LoadSummaryPartialFromSession();
    LoadSummaryPartialFromSession1();
    generateCaptcha();
    loadGenders();
    loadCountries();
    // Email Verification Flow
    $('#emailInput').on('input', function () {
        const email = $(this).val();
        const isValid = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
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
        if (!validateBookingForm()) return;

        const formData = $(this).serializeArray();
        const jsonData = serializeFormToJson(formData);



        jsonData.TotalPrice = parseFloat($('.total .amount').text().replace('$', '').trim());

        try {
            //const response = await fetch('/Home/ProcessPaymentAndSave', {
            const response = await fetch('/Home/SubmitBooking', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(jsonData)
            });
            const result = await response.json();
            if (result.success) {
                window.location.href = result.redirectUrl || '/Home/ReviewPayment';
            } else {
                alert('Payment failed: ' + result.message);
            }
        } catch (err) {
            console.error(err);
            alert('Server error. Try again.');
        }
    });

    function serializeFormToJson1(formData) {
        //const data = {};
        //formData.forEach(item => {
        //    const keys = item.name.match(/[^\[\]]+/g);
        //    if (keys.length === 1) {
        //        data[keys[0]] = item.value;
        //    } else {
        //        if (!data[keys[0]]) data[keys[0]] = [];
        //        const index = parseInt(keys[1]);
        //        if (!data[keys[0]][index]) data[keys[0]][index] = {};
        //        data[keys[0]][index][keys[2]] = item.value;
        //    }
        //});
        //return data;
        const data = {};

        formData.forEach(item => {
            // Remove any leading dots from the name
            const cleanName = item.name.replace(/^\./, '');
            const keys = cleanName.match(/[^\[\]]+/g);

            if (!keys || keys.length === 0) return;

            if (keys.length === 1) {
                data[keys[0]] = item.value;
            } else {
                if (!data[keys[0]]) data[keys[0]] = [];
                const index = parseInt(keys[1]);
                if (!data[keys[0]][index]) data[keys[0]][index] = {};
                data[keys[0]][index][keys[2]] = item.value;
            }
        });
        console.log(data);
        return data;
    }
    function serializeFormToJson(formData) {
        const data = {};

        formData.forEach(item => {
            // Skip items without a name
            if (!item.name) return;

            console.log('Processing:', item.name, '=', item.value);

            // Parse the field name pattern: Guests[0].FirstName
            const match = item.name.match(/^([a-zA-Z_][a-zA-Z0-9_]*)\[(\d+)\]\.([a-zA-Z_][a-zA-Z0-9_]*)$/);

            if (match) {
                // It's a nested object like Guests[0].FirstName
                const [, arrayName, indexStr, propertyName] = match;
                const index = parseInt(indexStr);

                console.log('Nested field:', arrayName, '[', index, '].', propertyName);

                // Initialize the array if it doesn't exist
                if (!data[arrayName]) {
                    data[arrayName] = [];
                }

                // Initialize the object at this index if it doesn't exist
                if (!data[arrayName][index]) {
                    data[arrayName][index] = {};
                }

                // Set the property value (convert Age and GenderId to number)
                if (propertyName === 'Age' || propertyName === 'GenderId') {
                    data[arrayName][index][propertyName] = item.value ? parseInt(item.value) : null;
                } else {
                    data[arrayName][index][propertyName] = item.value;
                }
            } else {
                // It's a simple property like Email, PhoneNumber, etc.
                // Handle simple field names (no brackets)
                const simpleMatch = item.name.match(/^([a-zA-Z_][a-zA-Z0-9_]*)$/);
                if (simpleMatch) {
                    const propertyName = simpleMatch[1];
                    if (propertyName === 'CountryId') {
                        data[propertyName] = item.value ? parseInt(item.value) : null;
                    } else {
                        data[propertyName] = item.value;
                    }
                    console.log('Simple field:', propertyName);
                } else {
                    console.log('Skipping unrecognized field pattern:', item.name);
                }
            }
        });

        console.log('Final data structure:', JSON.stringify(data, null, 2));
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
    //$.ajax({
    //    type: "GET",
    //    url: '/Home/LoadSummaryFromSessionViewOnly',
    //    cache: false,
    //    dataType: "html",
    //    success: function (data) {
    //        $('#div_SummaryPartialView').html(data);
    //    },
    //    error: function (result) {
    //        console.error("Error loading summary from session", result.responseText);
    //    }
    //});
}
function LoadSummaryPartialFromSession1() {
    $.ajax({
        type: "GET",
        url: '/Home/LoadSummaryFromSessionViewOnly1',
        cache: false,
        dataType: "html",
        success: function (data) {
            $('#div_SummaryPartialView1').html(data);
        },
        error: function (result) {
            console.error("Error loading summary from session", result.responseText);
        }
    });
}
function loadGenders() {
    fetch(window.apiBaseUrl + '/api/Gender/GenderList')
        .then(response => response.json())
        .then(data => {
            const options = data.map(g => {
                const text = g.gender;
                const key = g.id;
                return `<option value="${key}">${text}</option>`;
            });
            $('select[id^="guestGender_"]').each(function () {
                const select = $(this);
                const selected = select.val();
                select.empty().append('<option value="" disabled selected>Gender</option>');
                options.forEach(opt => select.append(opt));
                if (selected) {
                    select.val(selected);
                }
                select.on('change', function () {
                    const idx = this.id.split('_')[1];
                    const text = $(this).find('option:selected').text();
                    $('#guestGenderName_' + idx).val(text);
                });
            });
        })
        .catch(err => console.error('Failed to load genders', err));
}
function loadCountries() {
    fetch(window.apiBaseUrl + '/api/Guests/FetchCountryList')
        .then(response => response.json())
        .then(data => {
            const options = data.map(c => {
                const text = c.name;
                const key = c.id;
                return `<option value="${key}">${text}</option>`;
            });
            const select = $('#countryId');
            const selected = select.val();
            select.empty().append('<option value="" disabled selected>Country</option>');
            options.forEach(opt => select.append(opt));
            if (selected) {
                select.val(selected);
            }
            select.on('change', function () {
                const text = $(this).find('option:selected').text();
                $('#countryName').val(text);
            });
        })
        .catch(err => console.error('Failed to load countries', err));
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

function submitBookingForm() {
    $('#bookingForm').trigger('submit');
}


function validateBookingForm() {
    $('#hcaptchaError').hide();
    let isValid = true;
    const hcaptchaResponse = hcaptcha.getResponse();
    if (!hcaptchaResponse) {
        $('#hcaptchaError').show();
        isValid = false;
    }

    const email = $('#emailInput').val().trim();
    if (!email) {
        $('#emailInput').addClass('is-invalid');
        isValid = false;
    } else {
        $('#emailInput').removeClass('is-invalid');
    }

    const phone = $('#phoneNumber').val().trim();
    if (!phone) {
        $('#phoneNumber').addClass('is-invalid');
        isValid = false;
    } else {
        $('#phoneNumber').removeClass('is-invalid');
    }

    const country = $('select[name="CountryId"]').val();
    if (!country) {
        $('select[name="CountryId"]').addClass('is-invalid');
        isValid = false;
    } else {
        $('select[name="CountryId"]').removeClass('is-invalid');
    }

    const zip = $('input[name="ZipCode"]').val().trim();
    if (!zip) {
        $('input[name="ZipCode"]').addClass('is-invalid');
        isValid = false;
    } else {
        $('input[name="ZipCode"]').removeClass('is-invalid');
    }

    $('#bookingForm input[name*="FirstName"]').each(function () {
        if (!$(this).val().trim()) {
            $(this).addClass('is-invalid');
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    //$('#bookingForm input[required][name*="Age"]').each(function () {
    //    const age = parseInt($(this).val(), 10);
    //    if (isNaN(age) || age < 14) {
    //        $(this).addClass('is-invalid');
    //        isValid = false;
    //    } else {
    //        $(this).removeClass('is-invalid');
    //    }
    //});

    return isValid;
}
function openLoginModal() {
    if (!validateBookingForm()) return;
    $('#loginModal').modal('show');
}

$(document).on('submit', '#loginForm', function (e) {
    e.preventDefault();
    $('#loginModal').modal('hide');
    submitBookingForm();
});
$(document).on('submit', '#registerForm', function (e) {
    e.preventDefault();
    $('#loginModal').modal('hide');
    submitBookingForm();
});
$(document).on('click', '#guestCheckout', function () {
    $('#loginModal').modal('hide');
    submitBookingForm();
});