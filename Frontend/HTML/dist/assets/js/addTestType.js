$(document).ready(function () {
    $('#addTestTypeForm').on('submit', function (e) {
        e.preventDefault();

        const newTestType = {
            name: $('#addTestTypeName').val(),
            description: $('#addTestTypeDescription').val(),
            cost: parseFloat($('#addTestTypeCost').val())
        };

        $.ajax({
            url: 'https://localhost:7119/api/TestTypes/add', // عدّل الرابط حسب API مالتك
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(newTestType),
            success: function () {
                $('#addTestTypeModal').modal('hide');
                $('#addTestTypeForm')[0].reset();
                getTestTypes(); // تستدعي تحميل البيانات مجددًا
            },
            error: function () {
                alert('Failed to add test type.');
            }
        });
    });
});
