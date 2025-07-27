$(document).ready(function () {
    $('#addTestModal').on('show.bs.modal', function () {
        $.ajax({
            url: 'https://localhost:7119/api/Tests/pairs', 
            method: 'GET',
            success: function (data) {
                const patientMap = new Map();
                const doctorMap = new Map();

                data.forEach(entry => {
                    if (!patientMap.has(entry.patientId)) {
                        patientMap.set(entry.patientId, entry.patientFullName);
                    }

                    if (!doctorMap.has(entry.doctorId)) {
                        doctorMap.set(entry.doctorId, entry.doctorFullName);
                    }
                });

                const patientSelect = $('#addPatientId');
                const doctorSelect = $('#addDoctorId');

                patientSelect.empty().append('<option disabled selected>Select Patient</option>');
                doctorSelect.empty().append('<option disabled selected>Select Doctor</option>');

                patientMap.forEach((name, id) => {
                    patientSelect.append(`<option value="${id}">${name}</option>`);
                });

                doctorMap.forEach((name, id) => {
                    doctorSelect.append(`<option value="${id}">${name}</option>`);
                });
            },
            error: function () {
                alert('Failed to load patient-doctor list.');
            }
        });
    });
});
