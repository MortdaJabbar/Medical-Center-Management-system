let testId = 0;

$(document).on('click', '.delete-btn', function () {
    testId = $(this).data('id');
    $('#deleteTestId').val(testId); // خزّن ID داخل input hidden
});

$('#confirmDeleteTestBtn').click(function () {
    testId = $('#deleteTestId').val();

    $.ajax({
        url: `https://localhost:7119/api/Tests/${testId}`,  // ← حذف حسب ID
        type: 'DELETE',
        success: function () {
            // إغلاق المودال
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteTestModal'));
            modal.hide();

            // إعادة تحميل الفحوصات
            loadTests();
        },
        error: function () {
            alert('Failed to delete the test.');
        }
    });
});
