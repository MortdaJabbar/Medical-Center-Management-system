// فتح مودال الحذف عند الضغط على الزر

let deleteid = 0;
$(document).on('click', '.delete-appointment-btn', function () {
    deleteid = $(this).data('id');
    $('#deleteAppointmentId').val(deleteid);
    $('#deleteAppointmentModal').modal('show');

     
});

// تنفيذ الحذف عند التأكيد
$('#confirmDeleteAppointmentBtn').click(function () {
    deleteid = $('#deleteAppointmentId').val();

    $.ajax({
        url: `https://localhost:7119/api/Appointemnts/delete/${deleteid}`,
        method: 'DELETE',
        success: function () {
            $('#deleteAppointmentModal').modal('hide');
            alert("Appointment deleted successfully!");
            loadAppointments(); // ← تأكد أنها موجودة لتحديث الجدول بعد الحذف
        },
        error: function () {
            alert("Failed to delete appointment.");
        }
    });
});
