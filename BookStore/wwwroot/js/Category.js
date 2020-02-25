var dataTable;

$(document).ready(function() {
    loadDataTable();
});

function loadDataTable() {
    dataTabe = $('#tblData').DataTable({
        "ajax": {
            "url": "/api/categories",
            "dataSrc": ""
        },
        "columns": [
            { "data": "name", "width": "60%" },
            {
                "data": "id", 
                "render": function (data) {
                    return `
                            <div class="text-center">
                                <a href="/Admin/Category/Upsert/${data}" class="btn btn-success text-white" style="cursor: pointer">
                                    <i class="fa fa-edit"></i>
                                </a>
                                <a class="btn btn-danger text-white" style="cursor: pointer">
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