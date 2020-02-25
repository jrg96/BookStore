var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        'processing': true,
        'paging': true,
        'serverSide': true,
        'serverMethod': 'get',
        "ajax": {
            "url": "/api/products/datatable",
            "dataSrc": "aaData"
        },
        "columns": [
            { "data": "title", "width": "60%" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/CoverType/Upsert/${data}" class="btn btn-success text-white" style="cursor: pointer">
                                    <i class="fa fa-edit"></i>
                                </a>
                                <a onclick=Delete("/api/covertypes/${data}") class="btn btn-danger text-white" style="cursor: pointer">
                                    <i class="fa fa-trash"></i>
                                </a>
                            </div>
                           `;
                },
                "width": "40%"
            }
        ]
    });

}
