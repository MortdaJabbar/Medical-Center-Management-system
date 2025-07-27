let currentTestResult = null;
let DeleteId = 0;
$(document).on('click', '.edit-btn', function () {
     DeleteId = $(this).data('id');
   
    $.ajax({
        url: `https://localhost:7119/api/Tests/${DeleteId}`,
        method: 'GET',
        success: function (data) {
            $('#editTestId').val(data.testID);
            $('#editCost').val(data.cost);
            $('#editStatus').val(data.status);
            $('#editNotes').val(data.notes ?? '');
            currentTestResult = data.testResult;
             
        },
        error: function () {
            alert('Failed to load test data.');
        }
    });
});

$('#editTestForm').submit(function (e) {
    e.preventDefault();

     DeleteId = $('#editTestId').val();
    const cost = $('#editCost').val();
    const newstatus = $('#editStatus').val();
   
    const notes = $('#editNotes').val();
    const fileInput = $('#editTestResult')[0];

    let testResult = currentTestResult;

    if (fileInput.files.length > 0) {
        const fileName = fileInput.files[0].name;
        testResult = `C:\\Files\\${fileName}`;
    }

     
    const updatedData = {
        cost: parseFloat(cost),
        stauts: Number(newstatus),
        notes: notes,
        testResult: testResult
    };
   console.log(updatedData);
    $.ajax({
        url: `https://localhost:7119/api/Tests/update/${DeleteId}`,
        type: 'PUT',
        contentType: 'application/json',
        data: JSON.stringify(updatedData),
        success: function () {
            const modal = bootstrap.Modal.getInstance(document.getElementById('editTestModal'));
            modal.hide();
            loadTests();
        },
        error: function () {
            alert('Failed to update test.');
        }
    });
});

