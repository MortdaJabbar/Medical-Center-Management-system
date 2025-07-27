let updatedAppointmentid = 0;
$(document).on('click', '.edit-appointment-btn', function () {
   updatedAppointmentid = $(this).data('id');
    $('#appointmentId').val(updatedAppointmentid);
   
    $.ajax({
        url: `https://localhost:7119/api/Appointemnts/get/${updatedAppointmentid}`,
        type: 'GET',
        success: function (data) {
             

            $('#appointmentId').val(data.appointmentID);
            $('#appointmentDate').val(data.appointmentDate);

            // قطع الثواني من الوقت "09:00:00" → "09:00"
            let time = data.appointmentTime;
            if (time.length === 8) {
                time = time.substring(0, 5);
            }
            $('#appointmentTime').val(time);

            $('#reason').val(data.reason);
            $('#status').val(data.status);
            
            $('#paid').val(data.paid.toString());

            $('#notes').val(data.notes ?? '');

            $('#patientId').val(data.patientID);
            $('#doctorId').val(data.doctorID);
        },
        error: function () {
            alert('Failed to load appointment details.');
        }
    });
});

$('#updateAppointmentForm').submit(function (e) {
    e.preventDefault();

    

    const updatedAppointment = {
        
       appointmentDate: $('#appointmentDate').val(),
       appointmenttime: $('#appointmentTime').val(),
        reason: $('#reason').val(),
        status: parseInt($('#status').val()),
        paid: $('#paid').val() === 'true',
        notes: $('#notes').val(),
        patientId: $('#patientId').val(),
        doctorId: $('#doctorId').val()
    };



    $.ajax({
        url: `https://localhost:7119/api/Appointemnts/update/${updatedAppointmentid}`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(updatedAppointment),
        success: function () {
            $('#updateAppointmentModal').modal('hide');
            alert('Appointment updated successfully!');
            // هنا ممكن تعيد تحميل الجدول مثلاً:
             loadAppointments();
        },
        error: function () {
            alert('Failed to update appointment.');
        }
    });
});

