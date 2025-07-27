let deleteid =0;
$(document).ready(function () {
    // لما يضغط على زر الحذف
    $('#testTypeTable').on('click', '.delete-btn', function () {
        deleteid = $(this).data('id');
        $('#deleteTestTypeId').val(deleteid);
        $('#deleteTestTypeModal').modal('show');
    });

    // تأكيد الحذف
    $('#confirmDeleteTestTypeBtn').on('click', function () {
         id = $('#deleteTestTypeId').val();

        $.ajax({
            url: `https://localhost:7119/api/TestTypes/${deleteid}`,
            method: 'DELETE',
            success: function () {
                $('#deleteTestTypeModal').modal('hide');
                getTestTypes();
            },
            error: function () {
                alert('Failed to delete test type.');
            }
        });
    });
});
