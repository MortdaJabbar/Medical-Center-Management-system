$(document).ready(function () {
    $('#addTestModal').on('show.bs.modal', function () {
        $.ajax({
            url: 'https://localhost:7119/api/TestTypes/all', // نفس API مالتك
            method: 'GET',
            success: function (data) {
                const select = $('#addTestTypeId');
                select.empty().append('<option disabled selected>Select Test Type</option>');

                data.forEach(test => {
                    select.append(`<option value="${test.testTypeId}" data-cost="${test.cost}">${test.name}</option>`);
                });
            },
            error: function () {
                alert('Failed to load test types dropdown.');
            }
        });
    });

    // عند تغيير نوع الفحص، غيّر الكلفة
    $('#addTestTypeId').on('change', function () {
        const cost = $(this).find(':selected').data('cost');
        $('#addCost').val(cost ?? 0);
    });
});
