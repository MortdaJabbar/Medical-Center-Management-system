$('#addAppointmentForm').submit(function (e) {
    e.preventDefault();

    const newAppointment = {
        patientId: $('#addPatientId').val(),
        doctorId: $('#addDoctorId').val(),
        appointmentDate: $('#addAppointmentDate').val(),
        appointmentTime: $('#addAppointmentTime').val(),
        reason: $('#addReason').val(),
        status: parseInt($('#addStatus').val()),
        paid: $('#addPaid').val() === 'true',
        notes: $('#addNotes').val()
    };

    $.ajax({
        url: 'https://localhost:7119/api/Appointemnts/add',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(newAppointment),
        success: function () {
            $('#addAppointmentModal').modal('hide');
            alert('Appointment added successfully!');
            // Optionally reload appointments table here
           loadAppointments();
        },
        error: function (xhr) {
            console.error(xhr.responseText);
            alert('Failed to add appointment.');
        }
    });
});
