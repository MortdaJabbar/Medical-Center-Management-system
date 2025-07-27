$(document).ready(function () {
    let testTypes = [];

    function loadTestTypes() {
        $.ajax({
            url: 'https://localhost:7119/api/TestTypes/all',
            method: 'GET',
            success: function (data) {
                testTypes = data;

                const select = $('#addTestTypeId');
                select.empty().append('<option disabled selected>Select Test Type</option>');

                data.forEach(test => {
                    select.append(`<option value="${test.testTypeId}" data-cost="${test.cost}">${test.name}</option>`);
                });
            },
            error: function () {
                alert('Failed to load test types.');
            }
        });
    }

    // عند فتح مودال الإضافة، حمّل أنواع الفحوصات
    $('#addTestModal').on('show.bs.modal', function () {
        loadTestTypes();
    });

    // لما يتغير نوع الفحص، غيّر قيمة الكلفة تلقائياً
    $('#addTestTypeId').on('change', function () {
        const selectedOption = $(this).find(':selected');
        const cost = selectedOption.data('cost');
        $('#addCost').val(cost ?? 0);
    });
});
