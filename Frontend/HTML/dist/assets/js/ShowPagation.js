function PagationDataTable(tableSelector, nonOrderableColumns = [], pageLength = 10) {
    if (!$.fn.DataTable.isDataTable(tableSelector)) {
        $(tableSelector).DataTable({
            paging: true,
            searching: true,
            ordering: true,
            
            pageLength: pageLength,
            lengthChange: true,
            lengthMenu: [2,5, 10, 15, 25, 50],
            info: false,
            columnDefs: nonOrderableColumns.map(index => ({
                orderable: false,
                targets: index
            })),
            language: {
                search: "Search:",
                paginate: {
                    next: "Next",
                    previous: "Previous"
                },
                zeroRecords: "No Result"
            },
             
        });

       
    }
}
