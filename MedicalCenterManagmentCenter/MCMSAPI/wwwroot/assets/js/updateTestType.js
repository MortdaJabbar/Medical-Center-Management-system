let id = 0;
$(document).ready(function () {
    // لما يضغط على زر التعديل
    $('#testTypeTable').on('click', '.edit-btn', function () {
         id = $(this).data('id');
        alert(id);
        // جلب البيانات بالتفصيل
        $.ajax({
            url: `https://localhost:7119/api/TestTypes/${id}`,
            method: 'GET',
            success: function (data) {
                $('#editTestTypeId').val(data.testTypeId);
                $('#editTestTypeName').val(data.name);
                $('#editTestTypeDescription').val(data.description || '');
                $('#editTestTypeCost').val(data.cost);
                $('#editTestTypeModal').modal('show');
            },
            error: function () {
                alert('Failed to fetch test type details.');
            }
        });
    });

    // تنفيذ التحديث عند إرسال النموذج
    $('#editTestTypeForm').on('submit', function (e) {
        e.preventDefault();

        const updatedTestType = {
           
            name: $('#editTestTypeName').val(),
            description: $('#editTestTypeDescription').val(),
            cost: parseFloat($('#editTestTypeCost').val())
        };

        $.ajax({
            url: `https://localhost:7119/api/TestTypes/update/${id}`,
            method: 'PUT',
            contentType: 'application/json',
            data: JSON.stringify(updatedTestType),
            success: function () {
                $('#editTestTypeModal').modal('hide');
                getTestTypes();
            },
            error: function () {
                alert('Failed to update test type.');
            }
        });
    });
});
