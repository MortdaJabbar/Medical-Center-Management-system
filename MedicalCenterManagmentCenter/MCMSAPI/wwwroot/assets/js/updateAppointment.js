
    let currentAppointmentId = null;
    let PatientId = "";
    let doctorId   = "";
    function openEditAppointmentModal(appointmentId) {
        currentAppointmentId = appointmentId;
    
        $.ajax({
            url: `https://localhost:7119/api/Appointemnts/get/${appointmentId}`,
            method: "GET",
            success: function (appointment) {
                // تعبئة فقط البيانات القابلة للتعديل
                $('#appointmentDate').val(appointment.appointmentDate);
                $('#appointmentTime').val(appointment.appointmentTime);
                $('#reason').val(appointment.reason || '');
                $('#status').val(appointment.status);
                $('#paid').val(appointment.paid.toString());
                $('#notes').val(appointment.notes || '');
                PatientId = appointment.patientId;
                doctorId   = appointment.doctorId ;
                $('#updateAppointmentModal').modal('show');
            },
            error: function () {
                alert("Failed to load appointment data.");
            }
        });

    // 🟢 عند الضغط على زر الحفظ داخل المودال
    $('#updateAppointmentForm').on('submit', function (e) {
        e.preventDefault();

        const updatedAppointment = {
            patientID: PatientId, // ثابت، ما نعدله
            doctorID: doctorId , // ثابت، ما نعدله
            appointmentDate: $('#appointmentDate').val(),
            appointmentTime: $('#appointmentTime').val(),
            paid: $('#paid').val() === "true",
            reason: $('#reason').val(),
            status: parseInt($('#status').val()),
            notes: $('#notes').val()
        };

       

        $.ajax({
            url: `https://localhost:7119/api/Appointemnts/update?id=${currentAppointmentId}`,
            method: "PUT",
            contentType: "application/json",
            data: JSON.stringify(updatedAppointment),
            success: function () {
                $('#updateAppointmentModal').modal('hide');
                showSuccessMessage("Appointment updated successfully!");
                fetchAppointments(); // تحديث الجدول
            },
            error: function (err) {
                switch (err.status) {
                    case 404:
                        showErrorMessage("The requested data could not be found.");
                        break;
                    case 500:
                        showErrorMessage("Something went wrong on our server. Please try again later.");
                    case 0 : showErrorMessage("Cannot connect to server. Please check your internet connection or if the server is running.");
                        break;
                    default:
                        showErrorMessage("Something went wrong.  Please Check Your internet connection or try again later.");
                        
                }  
                
            }
        });
    });

    }



    function showErrorMessage(message) {
        // Remove any existing alert first
        $('#errorAlert').remove();
    
        // Create and append the alert
        $('body').append(`
            <div id="errorAlert" class="alert alert-danger alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 300px;">
                <i class="uil uil-exclamation-circle"></i> <strong>Error:</strong> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `);
    
        window.scrollTo({ top: 0, behavior: 'smooth' });
    
        // Auto close after 5 seconds
        setTimeout(() => { 
            $('#errorAlert').alert('close'); 
        }, 5000);
    }
    function showSuccessMessage(message) {
        // Remove any existing alert first
        $('#successAlert').remove();
    
        // Create and append the alert
        $('body').append(`
            <div id="successAlert" class="alert alert-success alert-dismissible fade show position-fixed top-0 end-0 m-4" role="alert" style="z-index: 9999; min-width: 600px;">
                <i class="uil uil-check-circle"></i> <strong>Success:</strong> ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    
    
        // Auto close after 3 seconds
        setTimeout(() => { 
            $('#successAlert').alert('close'); 
        }, 5000);
    }
    